using CatEats.Domain.Common;
using CatEats.Domain.DomainEvents;
using CatEats.Domain.Enumerations;
using CatEats.Domain.Exceptions;
using CatEats.Domain.ValueObjects;

namespace CatEats.Domain.AggregateRoots;

public record Delivery : AggregateRoot<DeliveryId>
{
    public OrderId OrderId { get; private set; }
    public UserId RiderId { get; private set; }
    public DeliveryStatus Status { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public DateTime? PickedUpAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public Address PickupAddress { get; private set; }
    public Address DeliveryAddress { get; private set; }
    public decimal DeliveryFee { get; private set; }
    public int EstimatedDurationMinutes { get; private set; }
    public string? DeliveryNotes { get; private set; }
    
    private readonly List<LocationUpdate> _locationUpdates = new();
    public IReadOnlyCollection<LocationUpdate> LocationUpdates => _locationUpdates.AsReadOnly();

    private Delivery() { } // EF Constructor

    private Delivery(DeliveryId id, OrderId orderId, UserId riderId, Address pickupAddress,
                    Address deliveryAddress, decimal deliveryFee, int estimatedDurationMinutes) : base(id)
    {
        OrderId = orderId;
        RiderId = riderId;
        PickupAddress = pickupAddress;
        DeliveryAddress = deliveryAddress;
        DeliveryFee = ValidateDeliveryFee(deliveryFee);
        EstimatedDurationMinutes = ValidateEstimatedDuration(estimatedDurationMinutes);
        Status = DeliveryStatus.Assigned;
        AssignedAt = DateTime.UtcNow;
        
        AddDomainEvent(new DeliveryAssignedEvent(Id, orderId, riderId));
    }

    public static Delivery Create(OrderId orderId, UserId riderId, Address pickupAddress,
                                 Address deliveryAddress, decimal deliveryFee, int estimatedDurationMinutes)
    {
        return new Delivery(DeliveryId.New(), orderId, riderId, pickupAddress, deliveryAddress,
                          deliveryFee, estimatedDurationMinutes);
    }

    public void StartDelivery()
    {
        if (Status != DeliveryStatus.Assigned)
            throw new BusinessRuleValidationException("Only assigned deliveries can be started");

        Status = DeliveryStatus.EnRouteToPickup;
        AddDomainEvent(new DeliveryStartedEvent(Id, RiderId));
    }

    public void ConfirmPickup(string? notes = null)
    {
        if (Status != DeliveryStatus.EnRouteToPickup)
            throw new BusinessRuleValidationException("Can only confirm pickup when en route to pickup");

        Status = DeliveryStatus.PickedUp;
        PickedUpAt = DateTime.UtcNow;
        DeliveryNotes = notes?.Trim();
        
        AddDomainEvent(new OrderPickedUpEvent(Id, OrderId, PickedUpAt.Value));
    }

    public void StartDeliveryToCustomer()
    {
        if (Status != DeliveryStatus.PickedUp)
            throw new BusinessRuleValidationException("Can only start delivery after pickup");

        Status = DeliveryStatus.EnRouteToCustomer;
        AddDomainEvent(new DeliveryEnRouteToCustomerEvent(Id));
    }

    public void CompleteDelivery(string? deliveryConfirmation = null)
    {
        if (Status != DeliveryStatus.EnRouteToCustomer)
            throw new BusinessRuleValidationException("Can only complete delivery when en route to customer");

        Status = DeliveryStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        
        if (!string.IsNullOrWhiteSpace(deliveryConfirmation))
        {
            DeliveryNotes = string.IsNullOrWhiteSpace(DeliveryNotes) 
                ? deliveryConfirmation.Trim()
                : $"{DeliveryNotes} | Delivery: {deliveryConfirmation.Trim()}";
        }

        AddDomainEvent(new DeliveryCompletedEvent(Id, OrderId, DeliveredAt.Value));
    }

    public void UpdateLocation(double latitude, double longitude, string? address = null)
    {
        if (Status is DeliveryStatus.Delivered or DeliveryStatus.Cancelled)
            throw new BusinessRuleValidationException("Cannot update location for completed deliveries");

        ValidateCoordinates(latitude, longitude);

        var locationUpdate = new LocationUpdate(latitude, longitude, address);
        _locationUpdates.Add(locationUpdate);

        AddDomainEvent(new RiderLocationUpdatedEvent(Id, RiderId, latitude, longitude, DateTime.UtcNow));
    }

    public void Cancel(string reason)
    {
        if (Status is DeliveryStatus.Delivered or DeliveryStatus.Cancelled)
            throw new BusinessRuleValidationException("Cannot cancel completed deliveries");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Cancellation reason is required", nameof(reason));

        Status = DeliveryStatus.Cancelled;
        AddDomainEvent(new DeliveryCancelledEvent(Id, OrderId, reason.Trim()));
    }

    public Location? GetCurrentLocation()
    {
        var lastUpdate = _locationUpdates.OrderByDescending(u => u.UpdatedAt).FirstOrDefault();
        return lastUpdate != null 
            ? new Location(lastUpdate.Latitude, lastUpdate.Longitude, lastUpdate.Address)
            : null;
    }

    public TimeSpan? GetActualDeliveryDuration()
    {
        return DeliveredAt.HasValue && PickedUpAt.HasValue 
            ? DeliveredAt.Value - PickedUpAt.Value 
            : null;
    }

    public TimeSpan GetTotalDuration()
    {
        var endTime = DeliveredAt ?? DateTime.UtcNow;
        return endTime - AssignedAt;
    }

    private static decimal ValidateDeliveryFee(decimal deliveryFee)
    {
        if (deliveryFee < 0)
            throw new ArgumentException("Delivery fee cannot be negative", nameof(deliveryFee));
        return deliveryFee;
    }

    private static int ValidateEstimatedDuration(int estimatedDurationMinutes)
    {
        if (estimatedDurationMinutes <= 0)
            throw new ArgumentException("Estimated duration must be positive", nameof(estimatedDurationMinutes));
        return estimatedDurationMinutes;
    }

    private static void ValidateCoordinates(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90", nameof(latitude));
        if (longitude < -180 || longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180", nameof(longitude));
    }
}