using AwesomeAssertions;
using CatEats.Domain.Entities;
using CatEats.Domain.Exceptions;

namespace CatEats.Domain.Tests.Entities;

public class MenuCategoryTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateCategorySuccessfully()
    {
        // Act
        var category = new MenuCategory("Pizzas", "Delicious pizzas", 1);

        // Assert
        category.Should().NotBeNull();
        category.Id.Should().NotBe(Guid.Empty);
        category.Name.Should().Be("Pizzas");
        category.Description.Should().Be("Delicious pizzas");
        category.DisplayOrder.Should().Be(1);
        category.IsActive.Should().BeTrue();
        category.MenuItems.Should().BeEmpty();
    }

    [Theory]
    [InlineData("", "Description")]
    [InlineData("   ", "Description")]
    [InlineData("Name", "")]
    [InlineData("Name", "   ")]
    public void Constructor_WithInvalidData_ShouldThrowArgumentException(string name, string description)
    {
        // Act & Assert
        var act = () => new MenuCategory(name, description, 1);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddMenuItem_WithValidData_ShouldAddMenuItemSuccessfully()
    {
        // Arrange
        var category = new MenuCategory("Pizzas", "Delicious pizzas", 1);

        // Act
        category.AddMenuItem("Margherita", "Classic pizza", 12.99m, true);

        // Assert
        category.MenuItems.Should().HaveCount(1);
        var menuItem = category.MenuItems.First();
        menuItem.Name.Should().Be("Margherita");
        menuItem.Description.Should().Be("Classic pizza");
        menuItem.Price.Should().Be(12.99m);
        menuItem.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public void AddMenuItem_WithDuplicateName_ShouldThrowBusinessRuleValidationException()
    {
        // Arrange
        var category = new MenuCategory("Pizzas", "Delicious pizzas", 1);
        category.AddMenuItem("Margherita", "Classic pizza", 12.99m, true);

        // Act & Assert
        var act = () => category.AddMenuItem("Margherita", "Another pizza", 15.99m, true);
        act.Should().Throw<BusinessRuleValidationException>()
           .WithMessage("Menu item 'Margherita' already exists in this category");
    }

    [Fact]
    public void UpdateDisplayOrder_WithValidOrder_ShouldUpdateOrder()
    {
        // Arrange
        var category = new MenuCategory("Pizzas", "Delicious pizzas", 1);

        // Act
        category.UpdateDisplayOrder(5);

        // Assert
        category.DisplayOrder.Should().Be(5);
    }

    [Fact]
    public void UpdateDisplayOrder_WithNegativeOrder_ShouldThrowArgumentException()
    {
        // Arrange
        var category = new MenuCategory("Pizzas", "Delicious pizzas", 1);

        // Act & Assert
        var act = () => category.UpdateDisplayOrder(-1);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Display order cannot be negative*");
    }
}