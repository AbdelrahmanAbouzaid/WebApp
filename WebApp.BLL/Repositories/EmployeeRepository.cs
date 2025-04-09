using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.BLL.Interfaces;
using WebApp.DAL.Data.Contexts;
using WebApp.DAL.Models;

namespace WebApp.BLL.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly CompanyContext context;

        public EmployeeRepository(CompanyContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Employee>> GetByNameAsync(string name)
        {
            return await context.Employees.Include(e => e.Department).Where(e => e.Name.ToLower().Contains(name.ToLower())).ToListAsync();
        }
    }
}
