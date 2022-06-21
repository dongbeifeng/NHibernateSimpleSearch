# NHibernateUtils

## AuditingInterceptor

When an entity is being saved, `AuditingInterceptor` sets its properties decorated with `CreationTimeAttribute`, `CreationUserAttribute`, `ModificationTimeAttribute` or `ModificationUserAttribute`, 
When an entity is being updated, `AuditingInterceptor` sets its properties decorated with `ModificationTimeAttribute` or `ModificationUserAttribute`:

``` c#

public class Foo
{
    // ...
    
    [CreationTime]
    public virtual DateTime CreationTime { get; set; }
    
    [CreationUser]
    public virtual string? CreationUser { get; set; }
    
    [ModificationTime]
    public virtual DateTime ModificationTime { get; set; }
    
    [ModificationUser]
    public virtual string? ModificationUser { get; set; }
    
    public virtual int Quantity { get; set; }
    
    public virtual bool IsMarked { get; set; }
}



// ...
ClaimsPrincipal principal = ...;
AuditingInterceptor interceptor = new AuditingInterceptor(principal);
using var session = _sessionFactory.WithOptions().Interceptor(interceptor).OpenSession();
using var tx = session.BeginTransaction();
// ...



// ...
await session.SaveAsync(foo1).ConfigureAwait(false);
await session.UpdateAsync(foo2).ConfigureAwait(false);
// ...

await tx.CommitAsync().ConfigureAwait(false);

```

## CheckTransactionListener

`CheckTransactionListener` ensures each sql statement is executed in a transaction, it throws an `NoTransactionException` if there is no transaction.

``` c#

Configuration configuration = new Configuration();
configuration.Configure();
CheckTransactionListener checkTransactionListener = new CheckTransactionListener();
configuration.AppendListeners(ListenerType.PreInsert, new IPreInsertEventListener[] { checkTransactionListener });
configuration.AppendListeners(ListenerType.PreUpdate, new IPreUpdateEventListener[] { checkTransactionListener });
configuration.AppendListeners(ListenerType.PreDelete, new IPreDeleteEventListener[] { checkTransactionListener });
configuration.AppendListeners(ListenerType.PreLoad, new IPreLoadEventListener[] { checkTransactionListener });

// ...

```

## ChunkExtensions

`ChunkExtensions.LoadInChunksAsync` allows loading data from database in chunks by using `foreach` syntax.
It comes from an inventory allocation senario. Assume there are a lot of pallet records in database:

| PalletCode |  Fifo  | Quantity | 
| :--------: | :----: | :------: |
|    P01     |   01   |     1    |
|    P02     |   02   |     2    |
|    P03     |   03   |     3    |
|    ...     |   ...  |    ...   |
|    P99     |   99   |    99    |


The required number is 100 and the top 14 records would suffice, `1 + 2 + 3 + ... + 14 = 105`, it is wasteful to load all these records to do the allocation work.
Loading records in chunks should improve the performance:

``` C#

int required = 100;
int sum = 0;
var q = _session.Query<Pallet>().OrderBy(x => x.Fifo);

// the chunk size is 10 and only 2 chunks will be loaded
await foreach (var item in q.LoadInChunksAsync(10).ConfigureAwait(false))
{
    sum += item.Quantity;
    item.IsAllocated = true;
    if (sum >= 100)
    {
        break;
    }
}

```

## IModelMapperConfigurator

`IModelMapperConfigurator` is a custom interface to configure `NHibernate.Mapping.ByCode.ModelMapper`:

``` C#

public interface IModelMapperConfigurator
{
    void Configure(ModelMapper mapper);
}

```

`NotNullModelMapperConfigurator` maps properties of value type or decorated with `RequiredAttribute` as `not null` , 
`LengthModelMapperConfigurator` specifies the columns' length if there is a `MaxLengthAttribute` on the property.

## SimpleSearchExtensions

`SimpleSearchExtensions` translates a `searchArgs` object into a predicate:

``` c#

    class Student
    {
        public string StudentName { get; set; } = default!;

        public Clazz Clazz { get; set; } = default!;

        public int No { get; set; }
    }

    class Clazz
    {
        public string ClazzName { get; set; } = default!;
    }
    
    
    class StudentSearchArgs
    {
        // default SearchMode is Auto, Like for string and Equals for other datatype
        [SearchArg]
        public string? StudentName { get; set; }
        
        // source property is Clazz.ClazzName
        [SearchArg("", SearchMode.Equal)]
        public string? ClassName { get; set; }
        
        [SearchArg]
        publick int? No { get; set; }
    }
    

    StudentSearchArgs args = new StudentSearchArgs
    {
      StudentName = "A",
      ClassName = "B",
      No = 3,
    };
    
    // args means:
    // q.Where(x => x.StudentName.Like("%A%") && x.Clazz.ClazzName == "B" && x.No == 3);
    
    var IQueryable<Student> q = ...;
    var pagedList = await q.SearchAsync(args, "No", 1, 10);

```

## TestingQueryable

This is a unit testing tool allowing mock IQueryable<T> returned from `ISession.Query<T>` method.


