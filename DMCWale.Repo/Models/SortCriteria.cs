using System.Linq.Expressions;

namespace DMCWale.Repo.Models;

public class SortCriteria<TEntity>
{
    public required Expression<Func<TEntity, object>> SortExpression { get; set; }

    public bool Descending { get; set; }

    public IOrderedQueryable<TEntity> ApplyOrdering(IQueryable<TEntity> query, bool thenBy)
    {
        if (!thenBy)
        {
            return Descending
                ? query.OrderByDescending(SortExpression)
                : query.OrderBy(SortExpression);
        }

        if (query is not IOrderedQueryable<TEntity> orderedQuery)
        {
            return Descending
                ? query.OrderByDescending(SortExpression)
                : query.OrderBy(SortExpression);
        }

        return Descending
            ? orderedQuery.ThenByDescending(SortExpression)
            : orderedQuery.ThenBy(SortExpression);
    }
}
