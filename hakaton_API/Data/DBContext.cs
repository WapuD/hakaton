using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using hakaton_API.Data.Models;

namespace hakaton_API.Data
{
    public class DBContext : DbContext
    {
        public DBContext (DbContextOptions<DBContext> options)
            : base(options)
        {
        }

        public DbSet<hakaton_API.Data.Models.Role> Role { get; set; } = default!;
        public DbSet<hakaton_API.Data.Models.Competency> Competency { get; set; } = default!;
        public DbSet<hakaton_API.Data.Models.Employee> Employee { get; set; } = default!;
        public DbSet<hakaton_API.Data.Models.Interview> Interview { get; set; } = default!;
        public DbSet<hakaton_API.Data.Models.IPR> IPR { get; set; } = default!;
        public DbSet<hakaton_API.Data.Models.Review> Review { get; set; } = default!;
        public DbSet<hakaton_API.Data.Models.Survey> Survey { get; set; } = default!;
        public DbSet<hakaton_API.Data.Models.Test> Test { get; set; } = default!;
        public DbSet<hakaton_API.Data.Models.Testing> Testing { get; set; } = default!;
    }
}
