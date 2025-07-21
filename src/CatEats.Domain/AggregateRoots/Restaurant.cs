using CatEats.Domain.Common;
using CatEats.Domain.DomainEvents;
using CatEats.Domain.Entities;
using CatEats.Domain.Enumerations;
using CatEats.Domain.Exceptions;
using CatEats.Domain.ValueObjects;

namespace CatEats.Domain.AggregateRoots;

public record Restaurant : AggregateRoot<RestaurantId>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Email { get; private set; }
    public Address Address { get; private set; }
    public UserId OwnerId { get; private set; }
    public RestaurantStatus Status { get; private set; }
    public TimeSpan OpeningTime { get; private set; }
    public TimeSpan ClosingTime { get; private set; }
    public decimal DeliveryFee { get; private set; }
    public int MinimumOrderAmount { get; private set; }
    public int EstimatedDeliveryTime { get; private set; } // in minutes
    public DateTime CreatedAt { get; private set; }
    
    private readonly List<MenuCategory> _menuCategories = new();
    public IReadOnlyCollection<MenuCategory> MenuCategories => _menuCategories.AsReadOnly();

    private readonly List<string> _cuisineTypes = new();
    public IReadOnlyCollection<string> CuisineTypes => _cuisineTypes.AsReadOnly();

    private Restaurant() { } // EF Constructor

    private Restaurant(RestaurantId id, string name, string description, string phoneNumber,
                      string email, Address address, UserId ownerId, TimeSpan openingTime,
                      TimeSpan closingTime, decimal deliveryFee, int minimumOrderAmount,
                      int estimatedDeliveryTime) : base(id)
    {
        Name = name;
        Description = description;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        OwnerId = ownerId;
        OpeningTime = openingTime;
        ClosingTime = closingTime;
        DeliveryFee = deliveryFee;
        MinimumOrderAmount = minimumOrderAmount;
        EstimatedDeliveryTime = estimatedDeliveryTime;
        Status = RestaurantStatus.PendingApproval;
        CreatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new RestaurantRegisteredEvent(Id, name, ownerId));
    }

    public static Restaurant Create(string name, string description, string phoneNumber,
                                  string email, Address address, UserId ownerId,
                                  TimeSpan openingTime, TimeSpan closingTime,
                                  decimal deliveryFee, int minimumOrderAmount,
                                  int estimatedDeliveryTime)
    {
        ValidateBasicInfo(name, description, phoneNumber, email);
        ValidateBusinessHours(openingTime, closingTime);
        ValidateBusinessRules(deliveryFee, minimumOrderAmount, estimatedDeliveryTime);

        return new Restaurant(RestaurantId.New(), name, description, phoneNumber, email,
                            address, ownerId, openingTime, closingTime, deliveryFee,
                            minimumOrderAmount, estimatedDeliveryTime);
    }

    public void AddMenuCategory(string name, string description, int displayOrder)
    {
        if (_menuCategories.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            throw new BusinessRuleValidationException($"Menu category '{name}' already exists");

        var category = new MenuCategory(name, description, displayOrder);
        _menuCategories.Add(category);
        
        AddDomainEvent(new MenuCategoryAddedEvent(Id, category.Id, name));
    }

    public void AddMenuItem(MenuCategoryId categoryId, string name, string description,
                           decimal price, bool isAvailable = true)
    {
        var category = _menuCategories.FirstOrDefault(c => c.Id == categoryId);
        if (category == null)
            throw new EntityNotFoundException(nameof(MenuCategory), categoryId.Value.ToString());

        category.AddMenuItem(name, description, price, isAvailable);
        
        AddDomainEvent(new MenuItemAddedEvent(Id, categoryId, name, price));
    }

    public void AddCuisineType(string cuisineType)
    {
        if (string.IsNullOrWhiteSpace(cuisineType))
            throw new ArgumentException("Cuisine type cannot be empty", nameof(cuisineType));

        var normalizedCuisine = cuisineType.Trim();
        if (!_cuisineTypes.Contains(normalizedCuisine, StringComparer.OrdinalIgnoreCase))
        {
            _cuisineTypes.Add(normalizedCuisine);
        }
    }

    public void Approve()
    {
        if (Status != RestaurantStatus.PendingApproval)
            throw new BusinessRuleValidationException("Only pending restaurants can be approved");

        Status = RestaurantStatus.Active;
        AddDomainEvent(new RestaurantApprovedEvent(Id));
    }

    public void Suspend()
    {
        if (Status != RestaurantStatus.Active)
            throw new BusinessRuleValidationException("Only active restaurants can be suspended");

        Status = RestaurantStatus.Suspended;
        AddDomainEvent(new RestaurantSuspendedEvent(Id));
    }

    public bool IsOpenAt(DateTime dateTime)
    {
        var time = dateTime.TimeOfDay;
        return Status == RestaurantStatus.Active && 
               time >= OpeningTime && 
               time <= ClosingTime;
    }

    public void UpdateBusinessHours(TimeSpan openingTime, TimeSpan closingTime)
    {
        ValidateBusinessHours(openingTime, closingTime);
        OpeningTime = openingTime;
        ClosingTime = closingTime;
    }

    private static void ValidateBasicInfo(string name, string description, string phoneNumber, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Restaurant name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Restaurant description cannot be empty", nameof(description));
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("Valid email is required", nameof(email));
    }

    private static void ValidateBusinessHours(TimeSpan openingTime, TimeSpan closingTime)
    {
        if (openingTime >= closingTime)
            throw new ArgumentException("Opening time must be before closing time");
    }

    private static void ValidateBusinessRules(decimal deliveryFee, int minimumOrderAmount, int estimatedDeliveryTime)
    {
        if (deliveryFee < 0)
            throw new ArgumentException("Delivery fee cannot be negative", nameof(deliveryFee));
        if (minimumOrderAmount < 0)
            throw new ArgumentException("Minimum order amount cannot be negative", nameof(minimumOrderAmount));
        if (estimatedDeliveryTime <= 0)
            throw new ArgumentException("Estimated delivery time must be positive", nameof(estimatedDeliveryTime));
    }
}