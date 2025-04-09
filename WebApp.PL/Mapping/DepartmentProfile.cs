using AutoMapper;
using WebApp.DAL.Models;
using WebApp.PL.Dtos;

namespace WebApp.PL.Mapping
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<Department, DepartmentDto>().ReverseMap();
        }
    }
}
