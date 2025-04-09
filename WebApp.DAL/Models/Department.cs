using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Models
{
    public class Department : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Maneger { get; set; }
        public DateTime CreateAt { get; set; }

        public ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();
    }
}
