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
    public class GenericRepository<TEntity>(CompanyContext context) : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            if (typeof(TEntity) == typeof(Department))
            {
                return (IEnumerable<TEntity>)await context.Departments.Include(d => d.Employees).ToListAsync();
            }
            else if (typeof(TEntity) == typeof(Employee))
            {
                return (IEnumerable<TEntity>)await context.Employees.Include(e => e.Department).ToListAsync();
            }
            return await context.Set<TEntity>().ToListAsync();
        }
        public async Task<TEntity?> GetAsync(int id)
        {
            if (typeof(TEntity) == typeof(Department))
            {
                return await context.Departments.Include(d => d.Employees).FirstOrDefaultAsync(d => d.Id == id) as TEntity;
            }
            else if (typeof(TEntity) == typeof(Employee))
            {
                return await context.Employees.Include(e => e.Department).FirstOrDefaultAsync(d => d.Id == id) as TEntity;

            }
            return await context.Set<TEntity>().FindAsync(id);
        }
        public async Task AddAsync(TEntity model)
        {
            await context.Set<TEntity>().AddAsync(model);
        }
        public void Update(TEntity model)
        {
            context.Set<TEntity>().Update(model);
        }
        public void Delete(TEntity model)
        {
            context.Set<TEntity>().Remove(model);
        }

    }
}
