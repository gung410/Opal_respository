# Outbox Pattern

As part of business logic, microservices often do not only have to update their own local data store, but they also need to notify other services about data changes that happened. Publish a wrong message or a failure to publish an event can mean critical failure to the business process.  The Outbox Pattern describes an approach for letting services execute these two tasks in a safe and consistent manner; it provides source service offering reliable, eventually consistent data exchange across service boundaries.

## Register service

### 1. With EF

1. Register

- On your `Startup.cs` file

    ```csharp
    using Conexus.Opal.OutboxPattern;
    using Microsoft.Extensions.Configuration;

    public class Startup
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Your other code
            services.AddOutboxQueueMessage(Configuration);
            // Your other code
        }
    }
    ```

- Or in `Program.cs`

    ```csharp
        // in ConfigureServices
        collection.AddOutboxQueueMessage(context.Configuration);
    ```

2. Declare OutboxMessages table on your DBContext

    ```csharp
            public DbSet<OutboxMessage> OutboxMessages { get; set; }
    ```

3. On your `.csproj` file

    ```xml
    <ItemGroup>
        <Reference Include="Conexus.Opal.OutboxPattern" />
    </ItemGroup>

    <ItemGroup>
        <Compile Include="..\..\shared\OutboxPattern\OutboxQueueController.cs" Link="Controllers\OutboxQueueController.cs" />
        <Compile Include="..\..\shared\OutboxPattern\OutboxMessageConfiguration.cs" Link="Infrastructure\EntityConfigurations\OutboxMessageConfiguration.cs" />
    </ItemGroup>
    ```

### 2. With MongoDb

1. On the `.csproj` file

    ```xml
      <ItemGroup>
        <Reference Include="Conexus.Opal.OutboxPattern" />
      </ItemGroup>
    ```

2. Implement your DbContext with `IHasOutboxCollection`

    ```csharp
     public class YourDbContext : IHasOutboxCollection
    ```

3. Register DI in the `Program.cs`

    ```csharp
    using Conexus.Opal.OutboxPattern;
    ...
        // in ConfigureServices
        collection.AddMongoOutboxQueueMessage<YourDbContext>(context.Configuration);
    ...

## How to use

- Inject `IOutboxQueue` to your service and declare your message with `QueueMessage` e.g:
    ```csharp
    using Conexus.Opal.OutboxPattern;

    public class MyCommand : BaseThunderCommandHandler
    {
        public MyCommand(IOutboxQueue outboxQueue)
        {
            var queueMessage = new QueueMessage(routingKey, new OpalMQMessage<object> { });
            outboxQueue.QueueMessageAsync(queueMessage);
        }
    }
    ```

## Configurations

See available options that you can configure in the `appsetting.json` file with `OutboxOptions`.

Default:

- Number of messages to be sent at a time: 10
- The number of days before removing a message with SENT status: 7

