using System.Linq.Expressions;
using DMCWale.Repo.Models;
using DMCWale.Repo.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DMCWale.Repo.Queries;

public sealed class RepositoryQuery<TEntity> where TEntity : class
{
    private readonly List<Expression<Func<TEntity, object>>> _includeProperties = new();
    private readonly Repository<TEntity> _repository;
    private Expression<Func<TEntity, bool>>? _filter;
    private Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? _orderByQueryable;
    private Func<IQueryable<TEntity>, IQueryable<TEntity>>? _customOrderByQueryable;
    private int? _page;
    private int? _pageSize;
    private bool _trackingEnabled;

    public RepositoryQuery(Repository<TEntity> repository)
    {
        _repository = repository;
    }

    public RepositoryQuery<TEntity> Filter(Expression<Func<TEntity, bool>> filter)
    {
        _filter = filter;
        return this;
    }

    public RepositoryQuery<TEntity> AsTracking()
    {
        _trackingEnabled = true;
        return this;
    }

    public RepositoryQuery<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
    {
        _orderByQueryable = orderBy;
        return this;
    }

    public RepositoryQuery<TEntity> CustomOrderBy(Func<IQueryable<TEntity>, IQueryable<TEntity>> orderBy)
    {
        _customOrderByQueryable = orderBy;
        return this;
    }

    public RepositoryQuery<TEntity> Include(Expression<Func<TEntity, object>> expression)
    {
        _includeProperties.Add(expression);
        return this;
    }

    public IEnumerable<TEntity> Get()
    {
        return GetQueryable().AsEnumerable();
    }

    public Task<List<TEntity>> GetAsync(CancellationToken cancellationToken = default)
    {
        return GetQueryable().ToListAsync(cancellationToken);
    }

    public IEnumerable<TEntity> GetPage(int page, int pageSize, out int totalCount)
    {
        page = NormalizePage(page);
        pageSize = NormalizePageSize(pageSize);
        totalCount = BuildQueryWithoutPaging().Count();

        _page = page;
        _pageSize = pageSize;

        return GetQueryable().AsEnumerable();
    }

    public async Task<PagedListResult<TEntity>> GetPageAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = NormalizePage(page);
        pageSize = NormalizePageSize(pageSize);

        var countQuery = BuildQueryWithoutPaging();
        var totalCount = await countQuery.CountAsync(cancellationToken);

        _page = page;
        _pageSize = pageSize;

        var entities = await GetQueryable().ToListAsync(cancellationToken);

        return new PagedListResult<TEntity>
        {
            Entities = entities,
            HasNext = page * pageSize < totalCount,
            HasPrevious = page > 1,
            Count = totalCount
        };
    }

    public IQueryable<TEntity> GetQueryable()
    {
        return _repository.Get(
            _filter,
            _trackingEnabled,
            _orderByQueryable,
            _customOrderByQueryable,
            _includeProperties,
            _page,
            _pageSize);
    }

    [Obsolete("Use GetQueryable instead.")]
    public IQueryable<TEntity> GetQuerable()
    {
        return GetQueryable();
    }

    [Obsolete("Use GetQueryable instead.")]
    public IQueryable<TEntity> GetQuerable(
        Expression<Func<TEntity, bool>>? filter = null,
        bool trackingEnabled = false,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderByQuerable = null,
        List<Expression<Func<TEntity, object>>>? includeProperties = null,
        int? page = null,
        int? pageSize = null)
    {
        return _repository.Get(
            filter,
            trackingEnabled,
            orderByQuerable,
            _customOrderByQueryable,
            includeProperties,
            page,
            pageSize);
    }

    public int Count()
    {
        return _repository.Count(_filter);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return BuildQueryWithoutPaging().CountAsync(cancellationToken);
    }

    private IQueryable<TEntity> BuildQueryWithoutPaging()
    {
        return _repository.Get(
            _filter,
            _trackingEnabled,
            _orderByQueryable,
            _customOrderByQueryable,
            _includeProperties);
    }

    private static int NormalizePage(int page)
    {
        return page <= 0 ? 1 : page;
    }

    private static int NormalizePageSize(int pageSize)
    {
        return pageSize <= 0 ? 10 : pageSize;
    }
}
