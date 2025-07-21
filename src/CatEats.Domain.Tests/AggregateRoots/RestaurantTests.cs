using AwesomeAssertions;
using CatEats.Domain.AggregateRoots;
using CatEats.Domain.DomainEvents;
using CatEats.Domain.Enumerations;
using CatEats.Domain.Exceptions;
using CatEats.Domain.ValueObjects;

namespace CatEats.Domain.Tests.AggregateRoots;

public class RestaurantTests
{
    private readonly UserId _ownerId = UserId.New();
    private readonly Address _validAddress = new("123 Restaurant St", "Food City", "12345", "USA", 40.7128, -74.0060);

    [Fact]
    public void Create_WithValidData_ShouldCreateRestaurantSuccessfully()
    {
        // Arrange
        var name = "Mario's Pizza";
        var description = "Authentic Italian pizza";
        var phoneNumber = "555-1234";
        var email = "mario@pizza.com";
        var openingTime = new TimeSpan(10, 0, 0); // 10:00 AM
        var closingTime = new TimeSpan(22, 0, 0); // 10:00 PM
        var deliveryFee = 2.99m;
        var minimumOrderAmount = 15;
        var estimatedDeliveryTime = 30;

        // Act
        var restaurant = Restaurant.Create(name, description, phoneNumber, email, _validAddress,
                                         _ownerId, openingTime, closingTime, deliveryFee,
                                         minimumOrderAmount, estimatedDeliveryTime);

        // Assert
        restaurant.Should().NotBeNull();
        restaurant.Id.Should().NotBe(Guid.Empty);
        restaurant.Name.Should().Be(name);
        restaurant.Description.Should().Be(description);
        restaurant.PhoneNumber.Should().Be(phoneNumber);
        restaurant.Email.Should().Be(email);
        restaurant.Address.Should().Be(_validAddress);
        restaurant.OwnerId.Should().Be(_ownerId);
        restaurant.OpeningTime.Should().Be(openingTime);
        restaurant.ClosingTime.Should().Be(closingTime);
        restaurant.DeliveryFee.Should().Be(deliveryFee);
        restaurant.MinimumOrderAmount.Should().Be(minimumOrderAmount);
        restaurant.EstimatedDeliveryTime.Should().Be(estimatedDeliveryTime);
        restaurant.Status.Should().Be(RestaurantStatus.PendingApproval);
        restaurant.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        restaurant.DomainEvents.Should().HaveCount(1);
        restaurant.DomainEvents.First().Should().BeOfType<RestaurantRegisteredEvent>();
    }

    [Theory]
    [InlineData("", "Description", "555-1234", "test@email.com")] // Empty name
    [InlineData("Restaurant", "", "555-1234", "test@email.com")] // Empty description
    [InlineData("Restaurant", "Description", "", "test@email.com")] // Empty phone
    [InlineData("Restaurant", "Description", "555-1234", "")] // Empty email
    [InlineData("Restaurant", "Description", "555-1234", "invalid-email")] // Invalid email
    public void Create_WithInvalidBasicInfo_ShouldThrowArgumentException(
        string name, string description, string phoneNumber, string email)
    {
        // Arrange
        var openingTime = new TimeSpan(10, 0, 0);
        var closingTime = new TimeSpan(22, 0, 0);

        // Act & Assert
        var act = () => Restaurant.Create(name, description, phoneNumber, email, _validAddress,
                                        _ownerId, openingTime, closingTime, 2.99m, 15, 30);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithInvalidBusinessHours_ShouldThrowArgumentException()
    {
        // Arrange
        var openingTime = new TimeSpan(22, 0, 0); // 10:00 PM
        var closingTime = new TimeSpan(10, 0, 0); // 10:00 AM (next day, but not handled)

        // Act & Assert
        var act = () => Restaurant.Create("Restaurant", "Description", "555-1234", "test@email.com",
                                        _validAddress, _ownerId, openingTime, closingTime, 2.99m, 15, 30);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Opening time must be before closing time");
    }

    [Theory]
    [InlineData(-1.0, 15, 30)] // Negative delivery fee
    [InlineData(2.99, -1, 30)] // Negative minimum order amount
    [InlineData(2.99, 15, 0)] // Zero estimated delivery time
    [InlineData(2.99, 15, -1)] // Negative estimated delivery time
    public void Create_WithInvalidBusinessRules_ShouldThrowArgumentException(
        decimal deliveryFee, int minimumOrderAmount, int estimatedDeliveryTime)
    {
        // Arrange
        var openingTime = new TimeSpan(10, 0, 0);
        var closingTime = new TimeSpan(22, 0, 0);

        // Act & Assert
        var act = () => Restaurant.Create("Restaurant", "Description", "555-1234", "test@email.com",
                                        _validAddress, _ownerId, openingTime, closingTime,
                                        deliveryFee, minimumOrderAmount, estimatedDeliveryTime);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddMenuCategory_WithValidData_ShouldAddCategorySuccessfully()
    {
        // Arrange
        var restaurant = CreateValidRestaurant();
        restaurant.ClearDomainEvents();

        // Act
        restaurant.AddMenuCategory("Pizzas", "Delicious pizzas", 1);

        // Assert
        restaurant.MenuCategories.Should().HaveCount(1);
        var category = restaurant.MenuCategories.First();
        category.Name.Should().Be("Pizzas");
        category.Description.Should().Be("Delicious pizzas");
        category.DisplayOrder.Should().Be(1);
        category.IsActive.Should().BeTrue();
        restaurant.DomainEvents.Should().HaveCount(1);
        restaurant.DomainEvents.First().Should().BeOfType<MenuCategoryAddedEvent>();
    }

    [Fact]
    public void AddMenuCategory_WithDuplicateName_ShouldThrowBusinessRuleValidationException()
    {
        // Arrange
        var restaurant = CreateValidRestaurant();
        restaurant.AddMenuCategory("Pizzas", "Delicious pizzas", 1);

        // Act & Assert
        var act = () => restaurant.AddMenuCategory("Pizzas", "More pizzas", 2);
        act.Should().Throw<BusinessRuleValidationException>()
           .WithMessage("Menu category 'Pizzas' already exists");
    }

    [Fact]
    public void AddMenuItem_WithValidData_ShouldAddMenuItemSuccessfully()
    {
        // Arrange
        var restaurant = CreateValidRestaurant();
        restaurant.AddMenuCategory("Pizzas", "Delicious pizzas", 1);
        var categoryId = restaurant.MenuCategories.First().Id;
        restaurant.ClearDomainEvents();

        // Act
        restaurant.AddMenuItem(categoryId, "Margherita", "Classic pizza with tomato and mozzarella", 12.99m);

        // Assert
        var category = restaurant.MenuCategories.First();
        category.MenuItems.Should().HaveCount(1);
        var menuItem = category.MenuItems.First();
        menuItem.Name.Should().Be("Margherita");
        menuItem.Description.Should().Be("Classic pizza with tomato and mozzarella");
        menuItem.Price.Should().Be(12.99m);
        menuItem.IsAvailable.Should().BeTrue();
        restaurant.DomainEvents.Should().HaveCount(1);
        restaurant.DomainEvents.First().Should().BeOfType<MenuItemAddedEvent>();
    }

    [Fact]
    public void AddMenuItem_WithNonExistentCategory_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var restaurant = CreateValidRestaurant();
        var nonExistentCategoryId = MenuCategoryId.New();

        // Act & Assert
        var act = () => restaurant.AddMenuItem(nonExistentCategoryId, "Pizza", "Description", 10.99m);
        act.Should().Throw<EntityNotFoundException>();
    }

    [Fact]
    public void AddCuisineType_WithValidType_ShouldAddCuisineType()
    {
        // Arrange
        var restaurant = CreateValidRestaurant();

        // Act
        restaurant.AddCuisineType("Italian");
        restaurant.AddCuisineType("Pizza");

        // Assert
        restaurant.CuisineTypes.Should().HaveCount(2);
        restaurant.CuisineTypes.Should().Contain("Italian");
        restaurant.CuisineTypes.Should().Contain("Pizza");
    }

    [Fact]
    public void AddCuisineType_WithDuplicateType_ShouldNotAddDuplicate()
    {
        // Arrange
        var restaurant = CreateValidRestaurant();

        // Act
        restaurant.AddCuisineType("Italian");
        restaurant.AddCuisineType("italian"); // Case insensitive

        // Assert
        restaurant.CuisineTypes.Should().HaveCount(1);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void AddCuisineType_WithInvalidType_ShouldThrowArgumentException(string cuisineType)
    {
        // Arrange
        var restaurant = CreateValidRestaurant();

        // Act & Assert
        var act = () => restaurant.AddCuisineType(cuisineType);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Approve_WhenPendingApproval_ShouldApproveRestaurant()
    {
        // Arrange
        var restaurant = CreateValidRestaurant();
        restaurant.Status.Should().Be(RestaurantStatus.PendingApproval);
        restaurant.ClearDomainEvents();

        // Act
        restaurant.Approve();

        // Assert
        restaurant.Status.Should().Be(RestaurantStatus.Active);
        restaurant.DomainEvents.Should().HaveCount(1);
        restaurant.DomainEvents.First().Should().BeOfType<RestaurantApprovedEvent>();
    }

    [Fact]
    public void Approve_WhenNotPendingApproval_ShouldThrowBusinessRuleValidationException()
    {
        // Arrange
        var restaurant = CreateValidRestaurant();
        restaurant.Approve(); // Already approved

        // Act & Assert
        var act = () => restaurant.Approve();
        act.Should().Throw<BusinessRuleValidationException>()
           .WithMessage("Only pending restaurants can be approved");
    }

    [Fact]
    public void Suspend_WhenActive_ShouldSuspendRestaurant()
    {
        // Arrange
        var restaurant = CreateValidRestaurant();
        restaurant.Approve(); // Make it active first
        restaurant.ClearDomainEvents();

        // Act
        restaurant.Suspend();

        // Assert
        restaurant.Status.Should().Be(RestaurantStatus.Suspended);
        restaurant.DomainEvents.Should().HaveCount(1);
        restaurant.DomainEvents.First().Should().BeOfType<RestaurantSuspendedEvent>();
    }

    [Fact]
    public void Suspend_WhenNotActive_ShouldThrowBusinessRuleValidationException()
    {
        // Arrange
        var restaurant = CreateValidRestaurant();
        // Restaurant is PendingApproval by default

        // Act & Assert
        var act = () => restaurant.Suspend();
        act.Should().Throw<BusinessRuleValidationException>()
           .WithMessage("Only active restaurants can be suspended");
    }

    [Fact]
    public void IsOpenAt_WhenActiveAndWithinHours_ShouldReturnTrue()
    {
        // Arrange
        var restaurant = CreateValidRestaurant();
        restaurant.Approve();
        var testTime = DateTime.Today.Add(new TimeSpan(12, 0, 0)); // 12:00 PM (within 10 AM - 10 PM)

        // Act
        var isOpen = restaurant.IsOpenAt(testTime);

        // Assert
        isOpen.Should().BeTrue();
    }

    [Fact]
    public void IsOpenAt_WhenActiveButOutsideHours_ShouldReturnFalse()
    {
        // Arrange
        var restaurant = CreateValidRestaurant();
        restaurant.Approve();
        var testTime = DateTime.Today.Add(new TimeSpan(8, 0, 0)); // 8:00 AM (before 10 AM opening)

        // Act
        var isOpen = restaurant.IsOpenAt(testTime);

        // Assert
        isOpen.Should().BeFalse();
    }

    [Fact]
    public void IsOpenAt_WhenNotActive_ShouldReturnFalse()
    {
        // Arrange
        var restaurant = CreateValidRestaurant();
        // Restaurant is PendingApproval (not active)
        var testTime = DateTime.Today.Add(new TimeSpan(12, 0, 0)); // 12:00 PM (within hours)

        // Act
        var isOpen = restaurant.IsOpenAt(testTime);

        // Assert
        isOpen.Should().BeFalse();
    }

    [Fact]
    public void UpdateBusinessHours_WithValidHours_ShouldUpdateHours()
    {
        // Arrange
        var restaurant = CreateValidRestaurant();
        var newOpeningTime = new TimeSpan(9, 0, 0); // 9:00 AM
        var newClosingTime = new TimeSpan(23, 0, 0); // 11:00 PM

        // Act
        restaurant.UpdateBusinessHours(newOpeningTime, newClosingTime);

        // Assert
        restaurant.OpeningTime.Should().Be(newOpeningTime);
        restaurant.ClosingTime.Should().Be(newClosingTime);
    }

    [Fact]
    public void UpdateBusinessHours_WithInvalidHours_ShouldThrowArgumentException()
    {
        // Arrange
        var restaurant = CreateValidRestaurant();
        var invalidOpeningTime = new TimeSpan(23, 0, 0); // 11:00 PM
        var invalidClosingTime = new TimeSpan(9, 0, 0); // 9:00 AM

        // Act & Assert
        var act = () => restaurant.UpdateBusinessHours(invalidOpeningTime, invalidClosingTime);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Opening time must be before closing time");
    }

    private Restaurant CreateValidRestaurant()
    {
        return Restaurant.Create(
            "Test Restaurant",
            "Test Description",
            "555-1234",
            "test@restaurant.com",
            _validAddress,
            _ownerId,
            new TimeSpan(10, 0, 0), // 10:00 AM
            new TimeSpan(22, 0, 0), // 10:00 PM
            2.99m,
            15,
            30
        );
    }
}