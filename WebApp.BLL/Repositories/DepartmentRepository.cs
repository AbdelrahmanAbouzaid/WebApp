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
    public class DepartmentRepository : GenericRepository<Department>,IDepartmentRepository
    {
        public DepartmentRepository(CompanyContext context):base(context) { }
       
    }
}
