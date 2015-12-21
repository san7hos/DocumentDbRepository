# DocumentDbRepository
**DocumentDbRepository is** a dependency injection friendly **repository pattern 
for Azure DocumentDb**.
It simplifies user workflow with Azure DocumentDb 
by providing an additional layer on top of 
[Azure DocumentDb Client](https://www.nuget.org/packages/Microsoft.Azure.DocumentDB/).

## Usage
### The most simplified scenario
Create a repository that stores documents of type MyClass. 
The type has to inherit from type Resource from Azure DocumentDb Client.

```csharp
public class MyClass : Microsoft.Azure.Documents.Resource
{
    public string Name { get; set; }
    public int Number { get; set; }
}
```

```csharp
var documentClient = new Microsoft.Azure.Documents.Client.DocumentClient(...);
var repository = new Repository<MyClass>(documentClient, "myDatabase");
```

And then simply use the repository

```csharp
repository.Create(new MyClass
{
    Name = "James",
    Number = 7
});

MyClass james = await repository.Get(mc => mc.Number = 7);

james.Name = "James Bond";

await repository.Update(james);

await repository.Delete(james);
```

The repository takes care of creating the database and the collection for you 
using the built-in *BasicDatabaseProvider* and *GenericCollectionProvider*.

*BasicDatabaseProvider* is a simple implementation of IDatabaseProvider
that takes the database id from its constructor (in this case "myDatabase")
and ensures that the database exists when we want to use it.

*GenericCollectionProvider* is a simple implementation of ICollectionProvider
that handles collections based on the generic type name given to the provider.

## Advanced usage
You can create your own ICollectionProvider (and IDatabaseProvider) to customise
how the database is created and how the collections are created. In that case,
use the other repository constructor, e.g.:

```csharp
public class MyCollectionProvider : ICollectionProvider
{
    // some implementation
}

public class MyDatabaseProvider : IDatabaseProvider
{
    // some implementation
}

var documentClient = new Microsoft.Azure.Documents.Client.DocumentClient(...);
var repository = new Repository<MyClass>(
    documentClient,
    new MyCollectionProvider(new MyDatabaseProvider()));
```

Obviously, it is possible to implement ICollectionProvider only.

## Usage with Ninject in an Asp.Net MVC app
Somewhere in the galaxy...excuse me, somewhere in the ninject configuration:

```csharp
kernel.Bind<Microsoft.Azure.Documents.Client.DocumentClient>()
    .ToSelf()
    .InRequestScope()
    .WithConstructorArgument(new Uri(...))
    .WithConstructorArgument("auth key")
    .WithConstructorArgument<ConnectionPolicy>(null)
    .WithConstructorArgument<ConsistencyLevel?>(null);

kernel.Bind<IDbProvider>()
    .To<DbProvider>()
    .InRequestScope()
    .WithConstructorArgument("myDatabase");

kernel.Bind(typeof(Repository<>))
    .ToSelf()
    .InRequestScope();
```

In a controller:

```csharp
public class MyController : System.Web.Mvc.Controller
{
    private readonly Repository<MyClass> repository;

    public MyController(Repository<MyClass> repository)
    {
        this.repository = repository;
    }

    public async Task<ActionResult> Index()
    {
        return this.View((await this.repository.GetAll()).ToList());
    }
}
```

