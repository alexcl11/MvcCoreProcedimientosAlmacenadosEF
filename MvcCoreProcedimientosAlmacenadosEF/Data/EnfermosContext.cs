using Microsoft.EntityFrameworkCore;
using MvcCoreProcedimientosAlmacenadosEF.Models;

namespace MvcCoreProcedimientosAlmacenadosEF.Data
{
    
    public class EnfermosContext: DbContext
    {
        public EnfermosContext
            (DbContextOptions<EnfermosContext> options) : base(options) { }
        public DbSet<Enfermo> Enfermos { get; set; }
    }
}
