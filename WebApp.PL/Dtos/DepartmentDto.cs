using System.ComponentModel.DataAnnotations;
using WebApp.DAL.Data.Contexts;
using WebApp.DAL.Models;
using WebApp.PL.Filters;

namespace WebApp.PL.Dtos
{
    public class DepartmentDto
    {
        [Required(ErrorMessage = "Enter The Code")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Enter The Name")]
        [MaxLength(40, ErrorMessage = "Maxmum length 40")]
        [MinLength(2, ErrorMessage = "Minmum length 2")]
        [UniqueDepartment]
        public string Name { get; set; }
        [Required(ErrorMessage = "Enter The Manager Name")]
        [MaxLength(40, ErrorMessage = "Maxmum length 40")]
        [MinLength(3, ErrorMessage = "Minmum length 3")]
        public string Maneger { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;

    }
}
