// The source code in this file is from：
// https://stackoverflow.com/questions/61408448/how-to-mock-nhibernate-tolistasync-in-unit-test-with-moq
// https://github.com/rgvlee/StackOverflow/blob/master/Question61408448/Query.linq

using System.Collections;
using System.Linq.Expressions;

namespace NHibernateUtils;

/// <summary>
/// Implements <see cref="IQueryable<T>"/> interface for unit testing purpose.
/// </summary>
/// <typeparam name="T"></typeparam>
public class TestingQueryable<T> : IQueryable<T>, IOrderedQueryable<T>
{
    private readonly IQueryable<T> _queryable;

    public TestingQueryable(IQueryable<T> queryable)
    {
        _queryable = queryable;
        Provider = new TestingQueryProvider<T>(_queryable);
    }

    public TestingQueryable(IEnumerable<T> data) : this(data.AsQueryable())
    {
    }


    public Type ElementType => _queryable.ElementType;

    public Expression Expression => _queryable.Expression;

    public IQueryProvider Provider { get; }

    public IEnumerator<T> GetEnumerator()
    {
        return _queryable.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _queryable.GetEnumerator();
    }
}
