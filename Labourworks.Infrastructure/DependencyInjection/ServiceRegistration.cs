using Labourworks.Application.Interfaces;
using Labourworks.Infrastructure.Data;
using Labourworks.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



namespace Labourworks.Infrastructure.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
           
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));        
           
            services.AddScoped<IPlantMasterRepository, PlantMasterRepository>();

            services.AddScoped<IContractorMasterRepository, ContractorMasterRepository>();

            services.AddScoped<IUserMasterRepository, UserMasterRepository>();

            services.AddScoped<IDepartmentMasterRepository, DepartmentMasterRepository>();

            services.AddScoped<IWorkOrderMasterRepository, WorkOrderMasterRepository>();

            return services;
        }
    }
}