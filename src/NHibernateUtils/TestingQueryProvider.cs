// The source code in this file is from：
// https://stackoverflow.com/questions/61408448/how-to-mock-nhibernate-tolistasync-in-unit-test-with-moq
// https://github.com/rgvlee/StackOverflow/blob/master/Question61408448/Query.linq

using NHibernate;
using NHibernate.Linq;
using NHibernate.Type;
using System.Linq.Expressions;

namespace NHibernateUtils;

/// <summary>
/// Implements <see cref="INhQueryProvider"/> interface for unit testing purpose.
/// </summary>
/// <typeparam name="T"></typeparam>
public class TestingQueryProvider<T> : INhQueryProvider
{
    public TestingQueryProvider(IQueryable<T> source)
    {
        Source = source;
    }
    public IQueryable<T> Source { get; private set; }

    public IQueryable CreateQuery(Expression expression)
    {
        return new TestingQueryable<T>((IQueryable<T>)Source.Provider.CreateQuery(expression));
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new TestingQueryable<TElement>(Source.Provider.CreateQuery<TElement>(expression));
    }

    public object? Execute(Expression expression)
    {
        return Source.Provider.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return Source.Provider.Execute<TResult>(expression);
    }

    public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
    {
        return Task.FromResult(Execute<TResult>(expression));
    }

    public int ExecuteDml<T1>(QueryMode queryMode, Expression expression)
    {
        throw new NotImplementedException();
    }

    public Task<int> ExecuteDmlAsync<T1>(QueryMode queryMode, Expression expression, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IFutureEnumerable<TResult> ExecuteFuture<TResult>(Expression expression)
    {
        throw new NotImplementedException();
    }

    public IFutureValue<TResult> ExecuteFutureValue<TResult>(Expression expression)
    {
        throw new NotImplementedException();
    }

    public void SetResultTransformerAndAdditionalCriteria(IQuery query, NhLinqExpression nhExpression, IDictionary<string, Tuple<object, IType>> parameters)
    {
        throw new NotImplementedException();
    }
}
