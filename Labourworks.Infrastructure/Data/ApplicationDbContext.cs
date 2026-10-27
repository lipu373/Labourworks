
using Labourworks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Labourworks.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
    {
    }
    public DbSet<PlantMaster> PlantMaster { get; set; }
    public DbSet<ContractorMaster> ContractorMaster { get; set; }
    public DbSet<UserMaster> UserMaster { get; set; }
    public DbSet<RoleMaster> RoleMaster { get; set; }
    public DbSet<DepartmentMaster> DepartmentMaster { get; set; }
    public DbSet<WorkOrderMaster> WorkOrderMaster { get; set; }

}
