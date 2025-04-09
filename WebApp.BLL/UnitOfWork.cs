using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.BLL.Interfaces;
using WebApp.BLL.Repositories;
using WebApp.DAL.Data.Contexts;

namespace WebApp.BLL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CompanyContext context;
        public IDepartmentRepository DepartmentRepository { get; }
        public IEmployeeRepository EmployeeRepository { get; }
        public UnitOfWork(CompanyContext _context)
        {
            context = _context;
            DepartmentRepository = new DepartmentRepository(context);
            EmployeeRepository = new EmployeeRepository(context);
        }
        public Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }
        public async ValueTask DisposeAsync()
        {
            await context.DisposeAsync();
        }
    }
}
