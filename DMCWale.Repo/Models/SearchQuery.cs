using System.Linq.Expressions;

namespace DMCWale.Repo.Models;

public class SearchQuery<TEntity>
{
    public List<Expression<Func<TEntity, bool>>> Filters { get; set; } = new();

    public string? IncludeProperties { get; set; }

    public List<SortCriteria<TEntity>> SortCriterias { get; set; } = new();

    public int Skip { get; set; }

    public int Take { get; set; }
}
