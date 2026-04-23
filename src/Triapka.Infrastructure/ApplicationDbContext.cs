using Microsoft.EntityFrameworkCore;

namespace Triapka.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
}