using Data.Repository;
using Data.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Data;

public static class Module
{
    public static void AddData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRepositoryLink, RepositoryLink>();
    }
}