using Microsoft.EntityFrameworkCore;

namespace MHemmer.Boilerplate.Infra;

public sealed class DomainDbContext : DbContext
{
    public DomainDbContext(DbContextOptions<DomainDbContext> options) : base(options)
    {

    }
}