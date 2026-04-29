using DMCWale.Data;
using DMCWale.Repo.Enums;
using DMCWale.Repo.Models;
using DMCWale.Repo.Queries;
using Microsoft.EntityFrameworkCore;

namespace DMCWale.Repo.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> FindByIdAsync(object id, CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task InsertAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default);

    Task InsertCollectionAsync(IEnumerable<TEntity> entities, bool saveChanges = true, CancellationToken cancellationToken = default);

    Task UpdateAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default);

    Task UpdateCollectionAsync(IEnumerable<TEntity> entities, bool saveChanges = true, CancellationToken cancellationToken = default);

    Task DeleteAsync(object id, bool saveChanges = true, CancellationToken cancellationToken = default);

    Task DeleteAsync(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    RepositoryQuery<TEntity> Query();

    IQueryable<TEntity> AsQueryable(bool trackingEnabled = false);

    ApplicationDbContext GetDbContext();

    void ChangeEntityState<T>(T entity, EntityState state) where T : class;

    void ChangeEntityCollectionState<T>(ICollection<T> entityCollection, ObjectState state) where T : class;

    void ChangeEntityCollectionState<T>(ICollection<T> entityCollection, EntityState state) where T : class;

    PagedListResult<TEntity> Search(SearchQuery<TEntity> searchQuery);

    PagedListResult<TEntity> Search(SearchQuery<TEntity> searchQuery, out int totalCount);

    Task<PagedListResult<TEntity>> SearchAsync(SearchQuery<TEntity> searchQuery, CancellationToken cancellationToken = default);

    Task<(PagedListResult<TEntity> Result, int TotalCount)> SearchWithTotalCountAsync(
        SearchQuery<TEntity> searchQuery,
        CancellationToken cancellationToken = default);

    PagedListResult<TEntity> QSearch(SearchQuery<TEntity> searchQuery, out int totalCount);

    TEntity? FindById(object id);

    void Insert(TEntity entity, bool saveChanges = true);

    void InsertCollection(IEnumerable<TEntity> entities, bool saveChanges = true);

    void Update(TEntity entity, bool saveChanges = true);

    void UpdateCollection(IEnumerable<TEntity> entities, bool saveChanges = true);

    void Delete(object id, bool saveChanges = true);

    void Delete(TEntity entity, bool saveChanges = true);

    int SaveChanges();
}
