namespace DMCWale.Repo.Models;

public class PagedListResult<TEntity>
{
    public List<TEntity> Entities { get; set; } = new();

    public IQueryable<TEntity>? QEntities { get; set; }

    public bool HasNext { get; set; }

    public bool HasPrevious { get; set; }

    public int Count { get; set; }
}
