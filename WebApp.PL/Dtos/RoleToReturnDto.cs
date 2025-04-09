using System.ComponentModel.DataAnnotations;
using WebApp.PL.Filters;

namespace WebApp.PL.Dtos
{
    public class RoleToReturnDto
    {
        public string? Id { get; set; }
        [Required(ErrorMessage = "Role Name is required!")]
        [UniqueRole]
        public string Name { get; set; }
    }
}
