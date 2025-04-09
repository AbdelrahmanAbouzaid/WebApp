using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Models;

namespace WebApp.DAL.Data.Contexts
{
    public class CompanyContext : IdentityDbContext<AppUser>
    {
        public CompanyContext(DbContextOptions<CompanyContext> options)
            :base(options) 
        {
            
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
    }
}
