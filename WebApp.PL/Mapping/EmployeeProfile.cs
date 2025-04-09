using AutoMapper;
using WebApp.DAL.Models;
using WebApp.PL.Dtos;

namespace WebApp.PL.Mapping
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile() 
        {
            CreateMap<Employee, EmployeeDto>().ReverseMap(); 
        }
    }
}
