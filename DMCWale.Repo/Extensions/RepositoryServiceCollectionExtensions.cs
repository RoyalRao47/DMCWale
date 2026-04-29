using DMCWale.Repo.Interfaces;
using DMCWale.Repo.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DMCWale.Repo.Extensions;

public static class RepositoryServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        return services;
    }
}
