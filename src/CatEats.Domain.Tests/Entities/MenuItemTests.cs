using AwesomeAssertions;
using CatEats.Domain.Entities;

namespace CatEats.Domain.Tests.Entities;

public class MenuItemTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateMenuItemSuccessfully()
    {
        // Act
        var menuItem = new MenuItem("Margherita", "Classic pizza with tomato and mozzarella", 12.99m, true);

        // Assert
        menuItem.Should().NotBeNull();
        menuItem.Id.Should().NotBe(Guid.Empty);
        menuItem.Name.Should().Be("Margherita");
        menuItem.Description.Should().Be("Classic pizza with tomato and mozzarella");
        menuItem.Price.Should().Be(12.99m);
        menuItem.IsAvailable.Should().BeTrue();
        menuItem.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("", "Description", 12.99)]
    [InlineData("   ", "Description", 12.99)]
    [InlineData("Name", "", 12.99)]
    [InlineData("Name", "   ", 12.99)]
    [InlineData("Name", "Description", -1.0)]
    public void Constructor_WithInvalidData_ShouldThrowArgumentException(string name, string description, decimal price)
    {
        // Act & Assert
        var act = () => new MenuItem(name, description, price, true);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdatePrice_WithValidPrice_ShouldUpdatePrice()
    {
        // Arrange
        var menuItem = new MenuItem("Margherita", "Classic pizza", 12.99m, true);

        // Act
        menuItem.UpdatePrice(15.99m);

        // Assert
        menuItem.Price.Should().Be(15.99m);
    }

    [Fact]
    public void UpdatePrice_WithNegativePrice_ShouldThrowArgumentException()
    {
        // Arrange
        var menuItem = new MenuItem("Margherita", "Classic pizza", 12.99m, true);

        // Act & Assert
        var act = () => menuItem.UpdatePrice(-5.99m);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Price cannot be negative*");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SetAvailability_ShouldUpdateAvailability(bool availability)
    {
        // Arrange
        var menuItem = new MenuItem("Margherita", "Classic pizza", 12.99m, !availability);

        // Act
        menuItem.SetAvailability(availability);

        // Assert
        menuItem.IsAvailable.Should().Be(availability);
    }
}