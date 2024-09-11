# Tracking
This is a simple proof of concept application used to demonstrate real time tracking using RabbitMQ for inter-service communication.
Project consists of 4 parts:
- Tracking.Api - used to capture events and send them to Tracking.Client via RabbitMQ
- Tracking.Client - used to display received events
- MSSQL - used for data persistence
- RabbitMQ - used for sending events between Tracking.Api and Tracking.Client

## Getting started
To run the application using Docker, just type `docker compose up --build` and connect to [localhost:5000/swagger](http://localhost:5000/swagger) to test the API.

Note: *Tracking.Api application is dependant on MSSQL database and RabbitMQ, Tracking.Client is dependant on RabbitMQ, both services will automatically set up and recover once their dependencies are available.*

For easier testing the following accounts are already generated:

| Id | Name | IsActive |
| :--- | :--- |:---------|
| 1 | Active account | **true** |
| 2 | Inactive account | false    |
| 3 | Another active account | **true** |
| 4 | Yet another active account | **true** |

Client is configured to process messages from accounts 1 and 3. This is configurable in `docker-componse.yml`.

## Tracking.Api

### HTTP Endpoints
Tracking.Api is a simple web application with a single HTTP endpoint used to receive events and publish them to a RabbitMQ exchange. The endpoint is available at:

`[POST] http://localhost:5000/tracking-events/{accountId}?data={eventData}`

This is fine for POC application, we can send some text/base64 encoded events, while being limited with maximum URI length.
However in real world, we would send data in request body as JSON and enhance it with additional information such as EventId,
CorrelationId so we can follow the chain of calls through the network, EventEmitted timestamp, maybe information about who emitted the event etc.
If serialization performance was critical we might even replace JSON with more performant serialization format, such as Protobuf.

### Persistence 
For persistence we are using MSSQL database combined with Entity Framework Core library. EF Core supports the use of migrations, so we can easily define schema with code first approach and then upgrade / downgrade the database accordingly.

TrackingEvent entity has GUID for its primary key. This gives us flexibility when scaling the application, we might want more instances, maybe each instance
will even have its own sharded database. This allows us to easily split or aggregate existing data.

### Publisher
RabbitMQ is used for publishing messages to connected services. The MassTransit library provides an abstraction over the broker 
and ensures that the application remains fault-tolerant even in case of problems with the broker. It also allows us to 
easily implement [ransactional Outbox pattern](https://microservices.io/patterns/data/transactional-outbox.html) which allows our application to work even when our message broker is unavailable and guarantees us
that events will be published once message broker recovers. MassTransit also supports Azure Service Bus and Amazon SQS, so we can change the underlying broker with a simple configuration change.
Since we are only storing the events and publishing them, maybe a persistent event log such as Apache Kafka would be suitable. 
In that case our Tracking.Api application would become redundant.

### Integration tests
Our Tracking.Api application has two dependencies - MSSQL database and RabbitMQ message broker. When running integration tests
we spin up both dependencies in docker containers, run tests and then destroy containers afterwards. We are essentially testing 
on real version of our application - no in memory database or "fake" implementations of a publisher.

## Tracking.Client
Client application is connected to our message broker and displays received messages to console. MassTransit is again used
to make our application fault tolerant and automatically recoverable from network errors. It also supports filtering based on 
one or more AccountIds, which is used as RouteKey. We could easily change to routing based on headers if there was a need to filter
events by multiple parameters. It is also easy to scale the application, as we can spin up multiple (competing) consumers, or assign a subset of accountIds to each consumer.

## Questions

### Database
> Provide a database query for counting requests for unique accountIDs within the last week

Given the following dataset:

| Id | AccountId | Data              | Timestamp                             |
| :--- | :--- |:------------------|:--------------------------------------|
| 85f533cb-503e-4828-a85b-80a0381542d4 | 4 | Test              | **2024-09-10 15:48:44.0329075 +00:00** |
| 44409fe7-8c2d-450b-932a-28db10208989 | 3 | Another test      | **2024-09-10 15:48:35.2044445 +00:00** |
| 6c1f0e0d-4549-42c4-9541-b5f44531333f | 3 | Yet another test  | **2024-09-10 15:48:29.7362652 +00:00** |
| 1f27b35e-3b8b-4c64-8dc8-c632b4d8efde | 1 | Important data    | **2024-09-10 15:48:23.3611679 +00:00** |
| f8ac8d39-7dba-4f95-9c3b-bea3ee35a30a | 1 | Some data         | **2024-09-10 15:48:12.8343786 +00:00** |
| aaa2cebf-3930-4827-afe9-730d17df25e1 | 1 | Old things        | 2024-09-02 15:48:21.0482620 +00:00    |
| 53108a42-24a9-454c-a2b6-d8cd6ecba076 | 4 | Older things      | 2024-09-01 15:48:46.9372980 +00:00    |
| fe18e74d-005d-4f6e-afb3-73524b7607be | 3 | Even older things | 2024-09-01 15:48:31.2852960 +00:00    |
| 1cf0796f-18e4-4a29-b267-43271992c440 | 1 | Oldest things     | 2024-09-01 15:48:17.8842950 +00:00    |

We can filter out data that is older than a week and then group the remaining rows by AccountId:
```
SELECT t.AccountId, COUNT(*) as EventsCount
FROM dbo.TrackingEvents t
WHERE Timestamp >= DATEADD(day, -7, GETDATE())
GROUP BY t.AccountId
```

To receive the following result:

| AccountId | EventsCount |
| :--- | :--- |
| 1 | 2 |
| 3 | 2 |
| 4 | 1 |

### Scala
> Provided you would implement the solution using Scala, which Scala constructs would
you use (and why) to represent the event* that is propagated to the pub/sub system.

In Scala we would use `case class` due to its immutable property. Dotnet's equivalent `record` was used in this project.