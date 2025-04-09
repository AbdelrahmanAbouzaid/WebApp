using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApp.PL.Dtos
{
    public class EmployeeDto
    {
        [Required(ErrorMessage = "Enter The Name")]
        [MaxLength(40, ErrorMessage = "Maxmum length 40")]
        [MinLength(3, ErrorMessage = "Minmum length 3")]
        public string Name { get; set; }

        [Range(18, 50, ErrorMessage = "Range between 18 and 20")]
        public int? Age { get; set; }

        [Required(ErrorMessage = "Enter The Gender")]
        [RegularExpression("(male|female|Male|Female)", ErrorMessage = "Invalid input ")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Enter The Phone")]
        [Phone]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Enter The Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter The Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Enter The Salary")]
        [Range(4000, 60000, ErrorMessage = "Range between 4000 and 60000")]
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime HiringDate { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        [DisplayName("Department")]
        public int? DepartmentId { get; set; }
        public string? ImageName { get; set; }

        public IFormFile? Image { get; set; }

    }
}
