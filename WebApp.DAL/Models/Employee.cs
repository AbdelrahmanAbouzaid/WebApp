using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Models
{
    public class Employee : BaseEntity
    {
        public string Name { get; set; }
        public int? Age { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime HiringDate { get; set; }
        public DateTime CreateAt { get; set; }
        public string? ImageName { get; set; }

        public int? DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
