using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using WebApp.DAL.Data.Contexts;

namespace WebApp.PL.Filters
{
    public class UniqueRoleAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            string name = value.ToString().Trim();

            // Get the RoleManager from the service provider
            var roleManager = (RoleManager<IdentityRole>)validationContext.GetService(typeof(RoleManager<IdentityRole>));

            if (roleManager == null)
            {
                throw new InvalidOperationException("RoleManager is not available.");
            }

            // Check if the role already exists
            var roleExists = roleManager.RoleExistsAsync(name).Result;

            if (roleExists)
            {
                return new ValidationResult("This role name already exists.");
            }

            return ValidationResult.Success;
        }
    }
}
