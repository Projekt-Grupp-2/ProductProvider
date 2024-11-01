using Microsoft.EntityFrameworkCore;

namespace ProductProvider.Infrastructure.Contexts;

public class TestContext : DataContext
{
    public TestContext(DbContextOptions<DataContext> options)
       : base(options) { }
}
