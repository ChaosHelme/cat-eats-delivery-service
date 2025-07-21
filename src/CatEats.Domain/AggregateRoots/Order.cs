using CatEats.Domain.Common;
using CatEats.Domain.DomainEvents;
using CatEats.Domain.Entities;
using CatEats.Domain.Enumerations;
using CatEats.Domain.Exceptions;
using CatEats.Domain.ValueObjects;

namespace CatEats.Domain.AggregateRoots;

public sealed record Order : AggregateRoot<OrderId>
{
    public UserId CustomerId { get; private set; }
    public RestaurantId RestaurantId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime? EstimatedDeliveryTime { get; private set; }
    public DateTime? ActualDeliveryTime { get; private set; }
    public Address DeliveryAddress { get; private set; }
    public string? SpecialInstructions { get; private set; }
    public UserId? AssignedRiderId { get; private set; }
    
    // Pricing
    public decimal SubTotal { get; private set; }
    public decimal DeliveryFee { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    
    private readonly List<OrderItem> _orderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    private Order() { } // EF Constructor

    private Order(OrderId id, UserId customerId, RestaurantId restaurantId, Address deliveryAddress,
                 string? specialInstructions = null) : base(id)
    {
        CustomerId = customerId;
        RestaurantId = restaurantId;
        DeliveryAddress = deliveryAddress;
        SpecialInstructions = specialInstructions?.Trim();
        Status = OrderStatus.Created;
        OrderDate = DateTime.UtcNow;
        
        AddDomainEvent(new OrderCreatedEvent(Id, customerId, restaurantId));
    }

    public static Order Create(UserId customerId, RestaurantId restaurantId, Address deliveryAddress,
                              string? specialInstructions = null)
    {
        return new Order(OrderId.New(), customerId, restaurantId, deliveryAddress, specialInstructions);
    }

    public void AddItem(MenuItemId menuItemId, string itemName, decimal unitPrice, int quantity,
                       string? specialRequests = null)
    {
        if (Status != OrderStatus.Created)
            throw new BusinessRuleValidationException("Cannot modify order items after order has been placed");

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

        if (string.IsNullOrWhiteSpace(itemName))
            throw new ArgumentException("Item name cannot be empty", nameof(itemName));

        // Check if item already exists, if so, update quantity
        var existingItem = _orderItems.FirstOrDefault(item => item.MenuItemId == menuItemId);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var orderItem = new OrderItem(menuItemId, itemName, unitPrice, quantity, specialRequests);
            _orderItems.Add(orderItem);
        }

        RecalculateAmounts();
        AddDomainEvent(new OrderItemAddedEvent(Id, menuItemId, quantity));
    }

    public void RemoveItem(MenuItemId menuItemId)
    {
        if (Status != OrderStatus.Created)
            throw new BusinessRuleValidationException("Cannot modify order items after order has been placed");

        var item = _orderItems.FirstOrDefault(i => i.MenuItemId == menuItemId);
        if (item == null)
            throw new EntityNotFoundException(nameof(OrderItem), menuItemId.Value.ToString());

        _orderItems.Remove(item);
        RecalculateAmounts();
        AddDomainEvent(new OrderItemRemovedEvent(Id, menuItemId));
    }

    public void UpdateItemQuantity(MenuItemId menuItemId, int newQuantity)
    {
        if (Status != OrderStatus.Created)
            throw new BusinessRuleValidationException("Cannot modify order items after order has been placed");

        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(newQuantity));

        var item = _orderItems.FirstOrDefault(i => i.MenuItemId == menuItemId);
        if (item == null)
            throw new EntityNotFoundException(nameof(OrderItem), menuItemId.Value.ToString());

        item.UpdateQuantity(newQuantity);
        RecalculateAmounts();
        AddDomainEvent(new OrderItemQuantityUpdatedEvent(Id, menuItemId, newQuantity));
    }

    public void PlaceOrder(decimal deliveryFee, decimal taxRate = 0.08m)
    {
        if (Status != OrderStatus.Created)
            throw new BusinessRuleValidationException("Order has already been placed");

        if (!_orderItems.Any())
            throw new BusinessRuleValidationException("Cannot place order without items");

        if (deliveryFee < 0)
            throw new ArgumentException("Delivery fee cannot be negative", nameof(deliveryFee));

        if (taxRate < 0 || taxRate > 1)
            throw new ArgumentException("Tax rate must be between 0 and 1", nameof(taxRate));

        DeliveryFee = deliveryFee;
        TaxAmount = SubTotal * taxRate;
        TotalAmount = SubTotal + DeliveryFee + TaxAmount;
        Status = OrderStatus.Placed;
        EstimatedDeliveryTime = DateTime.UtcNow.AddMinutes(45); // Default estimation

        AddDomainEvent(new OrderPlacedEvent(Id, CustomerId, RestaurantId, TotalAmount));
    }

    public void ConfirmByRestaurant(int estimatedPreparationMinutes)
    {
        if (Status != OrderStatus.Placed)
            throw new BusinessRuleValidationException("Only placed orders can be confirmed by restaurant");

        if (estimatedPreparationMinutes <= 0)
            throw new ArgumentException("Estimated preparation time must be positive", nameof(estimatedPreparationMinutes));

        Status = OrderStatus.Confirmed;
        EstimatedDeliveryTime = DateTime.UtcNow.AddMinutes(estimatedPreparationMinutes);

        AddDomainEvent(new OrderConfirmedEvent(Id, EstimatedDeliveryTime.Value));
    }

    public void StartPreparation()
    {
        if (Status != OrderStatus.Confirmed)
            throw new BusinessRuleValidationException("Only confirmed orders can start preparation");

        Status = OrderStatus.InPreparation;
        AddDomainEvent(new OrderPreparationStartedEvent(Id));
    }

    public void CompletePreparation()
    {
        if (Status != OrderStatus.InPreparation)
            throw new BusinessRuleValidationException("Only orders in preparation can be completed");

        Status = OrderStatus.ReadyForPickup;
        AddDomainEvent(new OrderReadyForPickupEvent(Id));
    }

    public void AssignToRider(UserId riderId)
    {
        if (Status != OrderStatus.ReadyForPickup)
            throw new BusinessRuleValidationException("Only orders ready for pickup can be assigned to riders");

        AssignedRiderId = riderId;
        Status = OrderStatus.OutForDelivery;
        AddDomainEvent(new OrderAssignedToRiderEvent(Id, riderId));
    }

    public void CompleteDelivery()
    {
        if (Status != OrderStatus.OutForDelivery)
            throw new BusinessRuleValidationException("Only orders out for delivery can be completed");

        Status = OrderStatus.Delivered;
        ActualDeliveryTime = DateTime.UtcNow;
        AddDomainEvent(new OrderDeliveredEvent(Id, ActualDeliveryTime.Value));
    }

    public void Cancel(string reason)
    {
        if (Status is OrderStatus.Delivered or OrderStatus.Cancelled)
            throw new BusinessRuleValidationException("Cannot cancel delivered or already cancelled orders");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Cancellation reason is required", nameof(reason));

        Status = OrderStatus.Cancelled;
        AddDomainEvent(new OrderCancelledEvent(Id, reason.Trim()));
    }

    public bool CanBeModified() => Status == OrderStatus.Created;

    private void RecalculateAmounts()
    {
        SubTotal = _orderItems.Sum(item => item.TotalPrice);
    }
}