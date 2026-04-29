using System.Linq.Expressions;
using DMCWale.Data;
using DMCWale.Repo.Enums;
using DMCWale.Repo.Interfaces;
using DMCWale.Repo.Models;
using DMCWale.Repo.Queries;
using Microsoft.EntityFrameworkCore;

namespace DMCWale.Repo.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public Repository(ApplicationDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public ApplicationDbContext GetDbContext()
    {
        return Context;
    }

    public ApplicationDbContext GetDbContext(int commandTimeout)
    {
        if (commandTimeout > 0)
        {
            Context.Database.SetCommandTimeout(commandTimeout);
        }

        return Context;
    }

    public virtual async Task<TEntity?> FindByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    public virtual TEntity? FindById(object id)
    {
        return DbSet.Find(id);
    }

    public virtual Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return AsQueryable().ToListAsync(cancellationToken);
    }

    public virtual IQueryable<TEntity> AsQueryable(bool trackingEnabled = false)
    {
        IQueryable<TEntity> query = DbSet.AsQueryable();
        return trackingEnabled ? query : query.AsNoTracking();
    }

    public virtual async Task InsertAsync(
        TEntity entity,
        bool saveChanges = true,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);

        if (saveChanges)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    public virtual void Insert(TEntity entity, bool saveChanges = true)
    {
        DbSet.Add(entity);

        if (saveChanges)
        {
            SaveChanges();
        }
    }

    public virtual async Task InsertCollectionAsync(
        IEnumerable<TEntity> entities,
        bool saveChanges = true,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);

        if (saveChanges)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    public virtual void InsertCollection(IEnumerable<TEntity> entities, bool saveChanges = true)
    {
        DbSet.AddRange(entities);

        if (saveChanges)
        {
            SaveChanges();
        }
    }

    public virtual async Task UpdateAsync(
        TEntity entity,
        bool saveChanges = true,
        CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);

        if (saveChanges)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    public virtual void Update(TEntity entity, bool saveChanges = true)
    {
        DbSet.Update(entity);

        if (saveChanges)
        {
            SaveChanges();
        }
    }

    public virtual async Task UpdateCollectionAsync(
        IEnumerable<TEntity> entities,
        bool saveChanges = true,
        CancellationToken cancellationToken = default)
    {
        DbSet.UpdateRange(entities);

        if (saveChanges)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    public virtual void UpdateCollection(IEnumerable<TEntity> entities, bool saveChanges = true)
    {
        DbSet.UpdateRange(entities);

        if (saveChanges)
        {
            SaveChanges();
        }
    }

    public virtual async Task DeleteAsync(
        object id,
        bool saveChanges = true,
        CancellationToken cancellationToken = default)
    {
        var entity = await FindByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return;
        }

        await DeleteAsync(entity, saveChanges, cancellationToken);
    }

    public virtual void Delete(object id, bool saveChanges = true)
    {
        var entity = FindById(id);
        if (entity is null)
        {
            return;
        }

        Delete(entity, saveChanges);
    }

    public virtual async Task DeleteAsync(
        TEntity entity,
        bool saveChanges = true,
        CancellationToken cancellationToken = default)
    {
        if (Context.Entry(entity).State == EntityState.Detached)
        {
            DbSet.Attach(entity);
        }

        DbSet.Remove(entity);

        if (saveChanges)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    public virtual void Delete(TEntity entity, bool saveChanges = true)
    {
        if (Context.Entry(entity).State == EntityState.Detached)
        {
            DbSet.Attach(entity);
        }

        DbSet.Remove(entity);

        if (saveChanges)
        {
            SaveChanges();
        }
    }

    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Context.SaveChangesAsync(cancellationToken);
    }

    public virtual int SaveChanges()
    {
        return Context.SaveChanges();
    }

    public virtual RepositoryQuery<TEntity> Query()
    {
        return new RepositoryQuery<TEntity>(this);
    }

    public void ChangeEntityState<T>(T entity, EntityState state) where T : class
    {
        Context.Entry(entity).State = state;
    }

    public void ChangeEntityCollectionState<T>(ICollection<T> entityCollection, ObjectState state) where T : class
    {
        ChangeEntityCollectionState(entityCollection, ConvertState(state));
    }

    public void ChangeEntityCollectionState<T>(ICollection<T> entityCollection, EntityState state) where T : class
    {
        foreach (var entity in entityCollection)
        {
            Context.Entry(entity).State = state;
        }
    }

    internal IQueryable<TEntity> Get(
        Expression<Func<TEntity, bool>>? filter = null,
        bool trackingEnabled = false,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? customOrderBy = null,
        List<Expression<Func<TEntity, object>>>? includeProperties = null,
        int? page = null,
        int? pageSize = null)
    {
        IQueryable<TEntity> query = DbSet.AsQueryable();

        if (!trackingEnabled)
        {
            query = query.AsNoTracking();
        }

        if (includeProperties is not null)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        if (customOrderBy is not null)
        {
            query = customOrderBy(query);
        }

        if (orderBy is not null)
        {
            query = orderBy(query);
        }

        if (page.HasValue && pageSize.HasValue)
        {
            var normalizedPage = page.Value <= 0 ? 1 : page.Value;
            var normalizedPageSize = pageSize.Value <= 0 ? 10 : pageSize.Value;

            query = query
                .Skip((normalizedPage - 1) * normalizedPageSize)
                .Take(normalizedPageSize);
        }

        return query;
    }

    public virtual PagedListResult<TEntity> Search(SearchQuery<TEntity> searchQuery)
    {
        var sequence = BuildSearchQuery(searchQuery);
        return GetSearchResult(searchQuery, sequence);
    }

    public virtual PagedListResult<TEntity> Search(SearchQuery<TEntity> searchQuery, out int totalCount)
    {
        var sequence = BuildSearchQuery(searchQuery);
        return GetSearchResult(searchQuery, sequence, out totalCount);
    }

    public virtual async Task<PagedListResult<TEntity>> SearchAsync(
        SearchQuery<TEntity> searchQuery,
        CancellationToken cancellationToken = default)
    {
        var sequence = BuildSearchQuery(searchQuery);
        return await GetSearchResultAsync(searchQuery, sequence, cancellationToken);
    }

    public virtual async Task<(PagedListResult<TEntity> Result, int TotalCount)> SearchWithTotalCountAsync(
        SearchQuery<TEntity> searchQuery,
        CancellationToken cancellationToken = default)
    {
        var sequence = BuildSearchQuery(searchQuery);
        var result = await GetSearchResultAsync(searchQuery, sequence, cancellationToken);

        return (result, result.Count);
    }

    public virtual PagedListResult<TEntity> QSearch(SearchQuery<TEntity> searchQuery, out int totalCount)
    {
        var sequence = BuildSearchQuery(searchQuery);
        totalCount = sequence.Count();
        var resultQuery = ApplySearchPaging(searchQuery, sequence);

        return new PagedListResult<TEntity>
        {
            QEntities = resultQuery,
            HasNext = HasNext(searchQuery, totalCount),
            HasPrevious = searchQuery.Skip > 0,
            Count = totalCount
        };
    }

    internal int Count(Expression<Func<TEntity, bool>>? filter = null)
    {
        IQueryable<TEntity> query = DbSet.AsNoTracking();

        return filter is null
            ? query.Count()
            : query.Count(filter);
    }

    protected virtual IQueryable<TEntity> BuildSearchQuery(SearchQuery<TEntity> searchQuery)
    {
        IQueryable<TEntity> sequence = DbSet.AsNoTracking();

        sequence = ManageFilters(searchQuery, sequence);
        sequence = ManageIncludeProperties(searchQuery, sequence);
        sequence = ManageSortCriterias(searchQuery, sequence);

        return sequence;
    }

    protected virtual IQueryable<TEntity> ManageFilters(SearchQuery<TEntity> searchQuery, IQueryable<TEntity> sequence)
    {
        foreach (var filterClause in searchQuery.Filters)
        {
            sequence = sequence.Where(filterClause);
        }

        return sequence;
    }

    protected virtual IQueryable<TEntity> ManageIncludeProperties(SearchQuery<TEntity> searchQuery, IQueryable<TEntity> sequence)
    {
        if (string.IsNullOrWhiteSpace(searchQuery.IncludeProperties))
        {
            return sequence;
        }

        var properties = searchQuery.IncludeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries);

        foreach (var includeProperty in properties)
        {
            var trimmedProperty = includeProperty.Trim();
            if (!string.IsNullOrWhiteSpace(trimmedProperty))
            {
                sequence = sequence.Include(trimmedProperty);
            }
        }

        return sequence;
    }

    protected virtual IQueryable<TEntity> ManageSortCriterias(SearchQuery<TEntity> searchQuery, IQueryable<TEntity> sequence)
    {
        if (searchQuery.SortCriterias.Count == 0)
        {
            return sequence;
        }

        var orderedSequence = searchQuery.SortCriterias[0].ApplyOrdering(sequence, false);

        for (var i = 1; i < searchQuery.SortCriterias.Count; i++)
        {
            orderedSequence = searchQuery.SortCriterias[i].ApplyOrdering(orderedSequence, true);
        }

        return orderedSequence;
    }

    protected virtual PagedListResult<TEntity> GetSearchResult(
        SearchQuery<TEntity> searchQuery,
        IQueryable<TEntity> sequence)
    {
        return GetSearchResult(searchQuery, sequence, out _);
    }

    protected virtual PagedListResult<TEntity> GetSearchResult(
        SearchQuery<TEntity> searchQuery,
        IQueryable<TEntity> sequence,
        out int totalCount)
    {
        totalCount = sequence.Count();
        var result = ApplySearchPaging(searchQuery, sequence).ToList();

        return new PagedListResult<TEntity>
        {
            Entities = result,
            HasNext = HasNext(searchQuery, totalCount),
            HasPrevious = searchQuery.Skip > 0,
            Count = totalCount
        };
    }

    protected virtual async Task<PagedListResult<TEntity>> GetSearchResultAsync(
        SearchQuery<TEntity> searchQuery,
        IQueryable<TEntity> sequence,
        CancellationToken cancellationToken)
    {
        var totalCount = await sequence.CountAsync(cancellationToken);
        var result = await ApplySearchPaging(searchQuery, sequence).ToListAsync(cancellationToken);

        return new PagedListResult<TEntity>
        {
            Entities = result,
            HasNext = HasNext(searchQuery, totalCount),
            HasPrevious = searchQuery.Skip > 0,
            Count = totalCount
        };
    }

    private static IQueryable<TEntity> ApplySearchPaging(SearchQuery<TEntity> searchQuery, IQueryable<TEntity> sequence)
    {
        return searchQuery.Take > 0
            ? sequence.Skip(Math.Max(0, searchQuery.Skip)).Take(searchQuery.Take)
            : sequence;
    }

    private static bool HasNext(SearchQuery<TEntity> searchQuery, int totalCount)
    {
        return searchQuery.Take > 0 && Math.Max(0, searchQuery.Skip) + searchQuery.Take < totalCount;
    }

    private static EntityState ConvertState(ObjectState state)
    {
        return state switch
        {
            ObjectState.Added => EntityState.Added,
            ObjectState.Modified => EntityState.Modified,
            ObjectState.Deleted => EntityState.Deleted,
            _ => EntityState.Unchanged
        };
    }
}
