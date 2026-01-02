namespace Ordering.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices
            (this IServiceCollection services, IConfiguration configuration)
        {
            var connStr = configuration.GetConnectionString("Database");

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connStr));

            //services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            return services;
        }
    }
}