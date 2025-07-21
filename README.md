# Cat Eats Delivery Platform - .NET Aspire & Kubernetes

A comprehensive food ordering and delivery platform for 🐱 built with .NET 9, .NET Aspire, and designed for Kubernetes deployment. 

This project demonstrates modern microservices architecture, Domain-Driven Design (DDD), Test-Driven Development (TDD), and cloud-native patterns.

---

## 🏗️ Architecture Overview

### Technology Stack
- **.NET 9** - Latest .NET runtime and framework
- **.NET Aspire** - Orchestration, observability, and service discovery
- **Kubernetes** - Container orchestration and deployment
- **Docker** - Containerization
- **PostgreSQL** - Primary relational database
- **MongoDB** - Location and tracking data
- **Redis** - Caching and session management
- **RabbitMQ** - Asynchronous messaging
- **SignalR** - Real-time notifications
- **gRPC** - High-performance inter-service communication
- **Entity Framework Core** - Object-Relational Mapping
- **MassTransit** - Message bus abstraction
- **OpenTelemetry** - Distributed tracing and metrics
- **Serilog** - Structured logging

### Testing Stack
- **xUnit** - Unit testing framework
- **Reqnroll** - BDD integration testing (SpecFlow successor)
- **AwesomeAssertions** - Awesome assertion library (Formerly known as FluentAssertions)
- **Testcontainers** - Integration testing with real databases
- **NSubstitute** - Mocking framework

---

## 📁 Project Structure

```
cat-eats-delivery-service/
├── src/
│   ├── CatEats.AppHost/              # .NET Aspire orchestration
│   ├── CatEats.ServiceDefaults/      # Shared Aspire configuration
│   ├── CatEats.Domain/               # Domain models and business logic
│   ├── CatEats.Gateway/              # API Gateway (Ocelot/YARP)
│   ├── CatEats.UserService/          # User management microservice
│   ├── CatEats.RestaurantService/    # Restaurant management microservice
│   ├── CatEats.OrderService/         # Order processing microservice
│   ├── CatEats.DeliveryService/      # Delivery tracking microservice
│   ├── CatEats.NotificationService/  # Real-time notifications
│   ├── CatEats.Domain.Tests/         # Domain unit tests
│   ├── CatEats.UserService.IntegrationTests/
│   ├── CatEats.RestaurantService.IntegrationTests/
│   ├── CatEats.OrderService.IntegrationTests/
│   └── CatEats.DeliveryService.IntegrationTests/
├── k8s/                                   # Kubernetes manifests
├── docker/                                # Docker configurations
├── docs/                                  # Documentation
└── scripts/                               # Build and deployment scripts
```

---

## 🔧 Domain Models

### Core Aggregates

#### User Aggregate
- **User** - Customers, riders, and restaurant owners
- **Address** - Delivery addresses with geolocation
- **UserRole** - Customer, Rider, RestaurantOwner
- **UserStatus** - Active, Inactive, Deactivated

#### Restaurant Aggregate
- **Restaurant** - Restaurant information and business rules
- **MenuCategory** - Food categories (Pizzas, Burgers, etc.)
- **MenuItem** - Individual food items with pricing
- **RestaurantStatus** - PendingApproval, Active, Suspended, Closed

#### Order Aggregate
- **Order** - Customer orders with full lifecycle
- **OrderItem** - Individual items in an order
- **OrderStatus** - Created, Placed, Confirmed, InPreparation, ReadyForPickup, OutForDelivery, Delivered, Cancelled

#### Delivery Aggregate
- **Delivery** - Delivery assignments and tracking
- **LocationUpdate** - Real-time rider location tracking
- **DeliveryStatus** - Assigned, EnRouteToPickup, PickedUp, EnRouteToCustomer, Delivered, Cancelled

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET Aspire Workload](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/ChaosHelme/cat-eats-devlivery-service.git
   cd cat-eats-delivery-service
   ```

2. **Install .NET Aspire workload**
   ```bash
   dotnet workload install aspire
   ```

3. **Restore packages**
   ```bash
   dotnet restore
   ```

4. **Run the application**
   ```bash
   dotnet run --project src/CatEats.AppHost
   ```

5. **Access the Aspire Dashboard**
    - Open your browser to the URL shown in the console (usually https://localhost:17206)
    - View logs, traces, metrics, and service health

### Running Tests

**Unit Tests**
```bash
dotnet test src/CatEats.Domain.Tests/
```

**Integration Tests** (requires Docker)
```bash
dotnet test src/CatEats.UserService.IntegrationTests/
```

**All Tests**
```bash
dotnet test src/
```

---

## 🏛️ Architecture Patterns

### Domain-Driven Design (DDD)
- **Aggregates** - User, Restaurant, Order, Delivery
- **Value Objects** - Address, Location, Money
- **Domain Events** - Cross-aggregate communication
- **Repository Pattern** - Data access abstraction
- **Domain Services** - Complex business logic

### Test-Driven Development (TDD)
- **Red-Green-Refactor** cycle
- **Unit Tests** - Domain logic validation
- **Integration Tests** - End-to-end scenarios with Reqnroll
- **Test Containers** - Real database integration testing

### Microservices Patterns
- **API Gateway** - Single entry point
- **Service Discovery** - Automatic service registration
- **Circuit Breaker** - Fault tolerance
- **Saga Pattern** - Distributed transactions
- **Event Sourcing** - Audit trail and replay capability

---

## 📊 Observability

### Metrics
- Application metrics (request count, duration, errors)
- Business metrics (orders placed, delivery times)
- Infrastructure metrics (CPU, memory, disk)

### Logging
- Structured logging with Serilog
- Correlation IDs for request tracing
- Centralized log aggregation with Seq

### Tracing
- Distributed tracing with OpenTelemetry
- Request flow visualization
- Performance bottleneck identification

### Health Checks
- Service health endpoints
- Database connectivity checks
- External dependency monitoring

---

## 🔄 Event-Driven Architecture

### Domain Events
- `UserRegisteredEvent` - New user registration
- `OrderPlacedEvent` - Order placement
- `OrderConfirmedEvent` - Restaurant confirmation
- `DeliveryAssignedEvent` - Rider assignment
- `RiderLocationUpdatedEvent` - Real-time tracking

### Message Bus Integration
- **MassTransit** with RabbitMQ
- **Publish/Subscribe** patterns
- **Request/Response** patterns
- **Saga State Machines** for complex workflows

---

## 🚢 Kubernetes Deployment

### Container Strategy
- Multi-stage Docker builds
- Minimal base images (distroless)
- Non-root user execution
- Resource limits and requests

### Kubernetes Resources
- **Deployments** - Service deployment and scaling
- **Services** - Internal service communication
- **Ingress** - External traffic routing
- **ConfigMaps** - Configuration management
- **Secrets** - Sensitive data management
- **StatefulSets** - Database deployments

### Service Mesh (Future)
- Istio integration for advanced networking
- mTLS for service-to-service communication
- Traffic management and observability

---

## 🎯 API Endpoints

### User Service
- `POST /api/users/customers` - Register customer
- `POST /api/users/riders` - Register rider
- `GET /api/users/{id}` - Get user by ID
- `GET /api/users/riders/available` - Get available riders
- `POST /api/users/{id}/addresses` - Add user address

### Restaurant Service
- `POST /api/restaurants` - Register restaurant
- `GET /api/restaurants/{id}` - Get restaurant details
- `POST /api/restaurants/{id}/menu-categories` - Add menu category
- `POST /api/restaurants/{id}/menu-items` - Add menu item
- `PUT /api/restaurants/{id}/status` - Update restaurant status

### Order Service
- `POST /api/orders` - Create new order
- `GET /api/orders/{id}` - Get order details
- `PUT /api/orders/{id}/place` - Place order
- `PUT /api/orders/{id}/confirm` - Restaurant confirmation
- `PUT /api/orders/{id}/cancel` - Cancel order

### Delivery Service
- `POST /api/deliveries` - Create delivery
- `PUT /api/deliveries/{id}/start` - Start delivery
- `PUT /api/deliveries/{id}/pickup` - Confirm pickup
- `PUT /api/deliveries/{id}/complete` - Complete delivery
- `PUT /api/deliveries/{id}/location` - Update rider location

---

## 🔐 Security Considerations

### Authentication & Authorization
- JWT token-based authentication
- Role-based access control (RBAC)
- API key management for external services

### Data Protection
- Encryption at rest and in transit
- PII data handling compliance
- GDPR compliance considerations

### Network Security
- TLS everywhere
- Network policies in Kubernetes
- Service mesh security policies

---

## 📈 Performance & Scalability

### Caching Strategy
- Redis for session management
- Application-level caching
- Database query optimization

### Database Optimization
- Read replicas for scaling reads
- Connection pooling
- Index optimization
- Query performance monitoring

### Horizontal Scaling
- Stateless service design
- Kubernetes Horizontal Pod Autoscaler
- Load balancing strategies

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines
- Follow TDD practices
- Write comprehensive tests
- Use conventional commits
- Update documentation
- Follow C# coding standards

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 📚 Learning Resources

### .NET Aspire
- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Aspire Samples](https://github.com/dotnet/aspire-samples)

### Domain-Driven Design
- [Domain-Driven Design by Eric Evans](https://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215)
- [Implementing Domain-Driven Design by Vaughn Vernon](https://www.amazon.com/Implementing-Domain-Driven-Design-Vaughn-Vernon/dp/0321834577)

### Microservices
- [Microservices Patterns by Chris Richardson](https://microservices.io/book)
- [Building Microservices by Sam Newman](https://www.amazon.com/Building-Microservices-Designing-Fine-Grained-Systems/dp/1492034029)

### Kubernetes
- [Kubernetes Documentation](https://kubernetes.io/docs/)
- [Kubernetes Patterns by Bilgin Ibryam](https://www.amazon.com/Kubernetes-Patterns-Designing-Cloud-Native-Applications/dp/1492050288)

---

## 🎯 Roadmap

### Phase 1 - Core Services ☑️
- [x] Domain models and unit tests
- [x] User service with registration
- [x] Basic Aspire orchestration
- [ ] Integration tests with Reqnroll

### Phase 2 - Business Logic 🔄
- [ ] Restaurant service implementation
- [ ] Order service implementation
- [ ] Delivery service implementation
- [ ] Real-time notifications with SignalR

### Phase 3 - Advanced Features 📋
- [ ] Payment service integration
- [ ] Advanced search and filtering
- [ ] Machine learning for delivery optimization
- [ ] Mobile app API support

### Phase 4 - Production Ready 📋
- [ ] Kubernetes deployment manifests
- [ ] CI/CD pipeline setup
- [ ] Security hardening
- [ ] Performance optimization
- [ ] Monitoring and alerting

---

**Happy Coding! 🚀**