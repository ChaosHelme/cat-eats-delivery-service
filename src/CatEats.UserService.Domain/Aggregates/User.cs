using CatEats.Domain.Common;
using CatEats.Domain.ValueObjects;
using CatEats.UserService.Domain.DomainEvents;
using CatEats.UserService.Domain.Enumerations;

namespace CatEats.UserService.Domain.Aggregates;

public record User : AggregateRoot<UserId>
{
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string PhoneNumber { get; private set; }
    public UserRole Role { get; private set; }
    public UserStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    
    private readonly List<Address> _addresses = new();
    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

    private User() { } // EF Constructor

    private User(UserId id, string email, string firstName, string lastName, 
                string phoneNumber, UserRole role) : base(id)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Role = role;
        Status = UserStatus.Active;
        CreatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new UserRegisteredEvent(Id, email, role));
    }

    public static User CreateCustomer(string email, string firstName, string lastName, string phoneNumber)
    {
        ValidateEmail(email);
        ValidateName(firstName, nameof(firstName));
        ValidateName(lastName, nameof(lastName));
        ValidatePhoneNumber(phoneNumber);
        
        return new User(UserId.New(), email, firstName, lastName, phoneNumber, UserRole.Customer);
    }

    public static User CreateRider(string email, string firstName, string lastName, string phoneNumber)
    {
        ValidateEmail(email);
        ValidateName(firstName, nameof(firstName));
        ValidateName(lastName, nameof(lastName));
        ValidatePhoneNumber(phoneNumber);
        
        return new User(UserId.New(), email, firstName, lastName, phoneNumber, UserRole.Rider);
    }

    public void AddAddress(string street, string city, string postalCode, string country, 
                          double latitude, double longitude, bool isDefault = false)
    {
        if (isDefault)
        {
            foreach (var address in _addresses)
            {
                address.SetAsNonDefault();
            }
        }
        
        var newAddress = new Address(street, city, postalCode, country, latitude, longitude, isDefault);
        _addresses.Add(newAddress);
        
        AddDomainEvent(new AddressAddedEvent(Id, newAddress));
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (Status == UserStatus.Deactivated)
            throw new InvalidOperationException("User is already deactivated");
            
        Status = UserStatus.Deactivated;
        AddDomainEvent(new UserDeactivatedEvent(Id));
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
            
        if (!email.Contains('@'))
            throw new ArgumentException("Invalid email format", nameof(email));
    }

    private static void ValidateName(string name, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"{fieldName} cannot be empty", fieldName);
            
        if (name.Length < 2)
            throw new ArgumentException($"{fieldName} must be at least 2 characters", fieldName);
    }

    private static void ValidatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));
            
        if (phoneNumber.Length < 10)
            throw new ArgumentException("Phone number must be at least 10 digits", nameof(phoneNumber));
    }
}