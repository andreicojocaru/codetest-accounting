# Code Test - Accounting
A code test showcasing a (small) Accounting backend distributed system based on microservices.

## Problem Statement

The assessment consists of an API to be used for opening a new “current account” of already existing
customers.

Requirements

- The API will expose an endpoint which accepts the user information (customerID,
initialCredit).
- Once the endpoint is called, a new account will be opened connected to the user whose ID is
customerID.
- Also, if initialCredit is not 0, a transaction will be sent to the new account.
- Another Endpoint will output the user information showing Name, Surname, balance, and
transactions of the accounts.

## Solution Analysis

### Variant 1 - Self-Orchestrating (Micro)Services

In this approach each service can (virtually) talk to any other service. This can have a few benenfits, and tradeoffs outlined below.

#### Pros
- each of the service's operations is self-contained (makes sure to keep the internal state correct)
- inspecting any operation makes it easy to understand what pieces are required to make the operation happen
- validation is part of each operation, even if remote calls to other services are required

#### Cons 
- business logic is spread apart into multiple services, and some can succeed while other fail (no central point of truth)
- services can end up referencing each other, which makes it difficult to reason about the operation's requirements
- implementation of distributed state patterns (e.g. Saga Pattern) are neccessary to make sure one business operation succeeds accross all services and to keep the state of the business operation valid

### Variant 2 - Isolated (Micro)Services. Central Orchestrator.

In this approach each service can only update it's internal state and notify other services of state updates via Events. The business logic orchestration happens in a gateway service (e.g. Backend for Frontend).

#### Pros
- each service's responsability is to keep it's internal state correct
- on an internal update of state, the service can choose to notify other Services (broadcast using a Message Bus, or Pub/Sub)
- business logic steps exist only in one place, the Orchestrator. This talks to all the required services and validates states
- can improve business logic visibility since it's (usually) in one single service
- we have a clear flow of data: from the Orchestrator to the Logical Services. We won't end up with a matrix of references.

#### Cons
- we need to have a central Orchestrator service, which can become a bottleneck
- updating any of the Orchestrator's client services Contracts means multiple reasons for the Orchestrator service to change - some might see it as an anti-pattern
- logical services have to trust the chain of requests for validation. Business logic validation inside the service becomes difficult since the business requirements are usually outside the service (in the Orchestrator)


### Variant 3 - Backend for Frontend + Domain Driven Designed Services

This approach is a mix of the previous 2 approaches with one big difference: there is no central orchestrator. Instead, we model Services based on the business Domain (following Domain Driven Design).

Each of the services would have a concrete Business role, and keep it's state in an Aggregate Root. This Aggregate Root would have to validate and update the internal state, always keeping it valid and up to date.

#### Pros
- Aggregate Roots help with keeping consistent state at all times
- we can implement nice patterns inside the data layer - like Unit of Work, and Repository
- makes it easy to work with CQRS - and building services that handle different perspectives of the State stored by the Aggregate Root
- syncronizing different business processes can be done via Asyncronous communication (Message Bus or Pub/Sub)


#### Cons
- finding the right Domain models has always been difficult; many people in different roles need to come together to find a good Domain Design
- services have a tendency to become big, thus moving away from the microservices architecture (debatable, based on "what is a microservice?")
- sooner or later, and Orchestrator Service might pop up, unless we choose an Event-based architecture

### Conclusion

There are a few valid approaches, in this repository I choose to implement the `Variant 2 - Isolated (Micro)Services. Central Orchestrator`. The reason for that is that the business requirements are very small, and I wanted to avoid potential circular references between the `Accounts` and `Transaction` services. (e.g. creating a Transaction, would prompt the Account to update it's Balance; creating an Account with InitialCredit > 0, would prompt a new Transaction).

The `BFF` service is resposible for Orchestrating the business logic, and all the Logical services expose functionality to allow changing their internal state. 

Another responsability of the `BFF` is to aggregate data required for displaying the user information. It will get user information from the Customer service, account and balance information from the Accounts service, and the transactions list from the Transactions service.

Because of the size of the project, I didn't approach the asyncronous notification (Message Bus or Pub/Sub) part. 

## Implementation

The solution consists of 3 services: Consumers, Accounts and Transactions. It also contains a `Backend for Frontend` implementation, used as an orchestrator for the operations required in the problem description.

The `BFF` implementation exposes an API, as well as a UI Controller, together with a small UI implemented with ASP Razor.

### Architecture

Since the services are really small (only do Create and Read operations), the architecture is simple, layer-based. The layers are the `Application` and `Persistence (DataAccess)`. 

For the sake of simplicity and code reusability, we have a generic `IRepository<>` interface and one in-memory repository implementation.

> Note: In a real microservices scenario, databases and data access implementations should be separate. This allows segration of data storage technologies, as well as good responsability isolation among teams. Again, this data access code is shared for the sake of `DRY` (Don't repeat yourself).

### Service Clients

To showcase service connections between the BFF and the Logical serivces, I chose to generate the C# client classes based off the OpenAPI specification.

Each Logical service exposes a Swagger definition, which I imported into NSwag to generate the client classes.

Multiple approaches are valid, and these clients can be generated either dynamically at build time, or statically using the desktop NSwag Studio. I prefer the latter since the project is small and the client code can easily be inspected.

### (Local) Testing

In order to successfuly test the creation of an account we need two steps:
 - using the `Customers` service, create a customer

> POST https://localhost:50005/api/customer 

> BODY:
   {
    "firstName": "Andi",
    "surname": "C"
   }

![Create Customer](./docs/postman_1_create_customer.png)

 - using the `BFF`, open an account. This will orchestrate the validation and account creation across all services

> POST https://localhost:50001/api/account/open-account

> BODY: 
  {
    "customerId": 1,
    "initialCredit": 150
  }

If the customer doesn't exist, the `BFF` will return a `400 Bad Request` with `Customer not valid!` message.

![Open Account](./docs/postman_2_open_account.png)

 - using the `BFF`, query for User Information. This will orchestrate requests to all services, and consolidate returned data.

 > GET https://localhost:50001/api/information/user/1

 ![User Information](./docs/postman_3_user_information.png)


## Deployment

