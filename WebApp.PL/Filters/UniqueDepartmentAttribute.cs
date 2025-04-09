using System.ComponentModel.DataAnnotations;
using WebApp.BLL;
using WebApp.DAL.Data.Contexts;

namespace WebApp.PL.Filters
{
    public class UniqueDepartmentAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            //if (value == null) return null;
            //string name = value.ToString();

            ////var oldName = context.Departments.FirstOrDefault(e => e.Name == name);
            ////if (oldName != null)
            ////{
            ////    return new ValidationResult("The Depaertment in already existing");
            ////}
            //return ValidationResult.Success;


            if (value == null)
                return ValidationResult.Success;

            string name = value.ToString().Trim();

            // Get the ApplicationDbContext from the service provider
            var Context = (CompanyContext)validationContext.GetService(typeof(CompanyContext));

            if (Context == null)
            {
                throw new InvalidOperationException("Database context is not available.");
            }

            // Check if department name already exists
            var exists = Context.Departments.Any(d => d.Name == name);
            if (exists)
            {
                return new ValidationResult("The department already exists.");
            }

            return ValidationResult.Success;
        }
    }
}
