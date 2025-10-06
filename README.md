# Bank account withdrawal exercise

### Fundamental Business Capability

The core business operation is:

1. Validate account has sufficient funds for withdrawal
2. Deduct withdrawal amount from account balance
3. Notify external systems of successful withdrawal event

## My Approach/Strategy:

### Phase 1: The Problem & Solution

1. Understand core business rule/logic from code snippet
2. Investigate and identify issues in the code logic/snippet
3. Identify established patterns to solve common problem

### Phase 2: Foundation

1. Implement Clean Architecture structure
2. Add proper dependency injection
3. Implement Result pattern for standardized error/response handling
4. Add logging

### Phase 3: Data Consistency

1. Implement transactional outbox pattern
2. Implement proper transaction management
3. Add event sourcing foundation

### Phase 4: Reliability

1. Implement background event processing
2. Implement error handling

This approach ensures that the fundamental business capability remains unchanged while significantly improving the code's structure, reliability, maintainability, and overall quality. The implementation prioritizes correctness and consistency while providing a foundation for future enhancements and scalability requirements.

### Issues Identified

#### 1. Architectural Violations

- Fat Controller (business logic + persistence + integration in controller)
- Missing abstraction layers (no service, repository, or event publisher interfaces)
- Tight coupling to infrastructure (JdbcTemplate + AWS SNS directly)
- No transactional boundary around read-modify-write (check-then-update race condition)
- Dual-write problem (state change and event publish not atomically coordinated)
- Resource lifecycle not managed via DI (SnsClient manually built in controller)
- Mixed concerns (transport, business rules, persistence, serialization interleaved)

#### 2. Data Consistency Problems

- Race conditions in balance checking and updating
- No transaction management
- Potential for partial failures leaving system in inconsistent state

#### 3. Error Handling

- Simple string returns instead of proper error responses
- No structured error information
- Missing validation for edge cases
- No handling of external system connection failures

#### 4. Event Publishing Issues

- Unreachable code
- No handling of SNS publishing failures
- No retry mechanisms

#### 5. Configuration Management

- Hardcoded AWS region and topic ARN
- Missing environment-specific configuration
- No configuration validation

#### 6. Code Quality Issues

- No input validation
- Poor separation between data and behavior
- Manual JSON serialization prone to errors
- No logging or observability

## Improvement Approach Outline

### 1. Architecture Transformation

Implement Clean Architecture principles with proper layer separation:

**Domain Layer**: Core business entities and rules

- BankAccount entity with business logic
- WithdrawalEvent domain event

**Application Layer**: Use cases and coordination

- Withdrawal command and handler
- Event publishing coordination
- Transaction management

**Infrastructure Layer**: External concerns

- Database repository implementations
- AWS SNS integration

**Presentation Layer**: HTTP API concerns

- Request/response models
- Controller delegation to application layer

### 2. Data Consistency Strategy

Implement transactional outbox pattern:

- single transaction for balance update and event storage
- Background service for reliable event publishing
- Eventual consistency for external notifications

### 3. Error Handling Enhancement

Implement Result pattern for error management:

- Structured & standardized error responses with error codes
- Proper HTTP status code mapping
- Comprehensive validation with detailed messages

### 4. Event Publishing Reliability

Implement reliable messaging pattern:

- Store events in database within same transaction
- Background processing for event publishing/processing
- Implementation can be expanded to include:
  - Retry mechanisms
  - Dead letter queue for failed messages

### 5. Configuration Management

Externalize all configuration:

- Environment-specific settings
- AWS configuration through proper SDK configuration
- Validation of required configuration at startup
- Type-safe configuration classes

## Implementation Choices Explanation

### Choice 1: Clean Architecture over Layered Architecture

**Decision**: Implement Clean Architecture with dependency inversion
**Why**:

- Better testability through dependency injection
- Improved maintainability with clear boundaries
- Enhanced flexibility for future changes
- Reduced coupling between layers

### Choice 2: CQRS Pattern for Commands and Queries

**Decision**: Separate command (withdrawal) from query operations
**Why**:

- Clear separation of read and write operations
- Better scalability for complex business operations
- Improved testability of business logic

### Choice 3: Transactional Outbox Pattern

**Decision**: Store events in database within business transaction
**Why**:

- Guarantees consistency between state changes and events
- Eliminates dual-write problem
- Provides reliable event publishing
- Supports event retries, idempotency safeguards, and debugging
- Implementation can be expanded to:
  - include idempotency key, and enforce retries

### Choice 4: Result Pattern for Error Handling

**Decision**: Use Result types instead of exceptions for business errors
**Why**:

- Improves performance by avoiding exception overhead
- Forces proper error handling at call sites
- Provides structured error information

### Choice 5: Repository Pattern with Unit of Work

**Decision**: Abstract data access through repository interfaces
**Why**:

- Enables easier testing with mock repositories
- Provides clear data access abstraction
- Supports different data storage implementations
- Integrates well with transaction management

### Choice 6: MediatR for Command/Query Handling

**Decision**: Use MediatR library for command/query dispatch
**Why**:

- Decouples controllers from specific handlers
- Provides cross-cutting concern pipeline (logging, validation)
- Improves testability and maintainability

## Library Usage Documentation

### MediatR

Command/Query mediator pattern implementation:

- IRequest interface for commands and queries
- IRequestHandler for command/query handlers
- Pipeline behaviors for cross-cutting concerns

### FluentValidation

Input validation framework:

- Declarative validation rules
- Integration with MediatR pipeline
- Detailed validation error messages

### Serilog

Structured logging framework:

- Request/response logging
- Error logging with context
- Performance monitoring
- Integration with application insights

### AutoMapper (ToDo)

- Object-to-object mapping
