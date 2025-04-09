using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.BLL.Interfaces;
using WebApp.DAL.Models;
using WebApp.PL.Dtos;
using WebApp.PL.Helpers;

namespace WebApp.PL.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public EmployeeController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string? searchInput)
        {
            IEnumerable<Employee> employees;
            if (!string.IsNullOrEmpty(searchInput))
                employees = await unitOfWork.EmployeeRepository.GetByNameAsync(searchInput);
            else
                employees = await unitOfWork.EmployeeRepository.GetAllAsync();

            return View(employees);
        }
        [HttpGet]
        public async Task<IActionResult> Search(string searchInput)
        {
            IEnumerable<Employee> employees;
            if (!string.IsNullOrEmpty(searchInput))
                employees = await unitOfWork.EmployeeRepository.GetByNameAsync(searchInput);
            else
                employees = await unitOfWork.EmployeeRepository.GetAllAsync();

            return PartialView("_EmployeeListPartial", employees);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var departments = await unitOfWork.DepartmentRepository.GetAllAsync();
            ViewData["dapartments"] = departments;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeDto model)
        {
            if (ModelState.IsValid)
            {
                var employee = mapper.Map<Employee>(model);
                if (model.Image is not null)
                    employee.ImageName = DocumentSettings.UploadFile(model.Image, "images");
                await unitOfWork.EmployeeRepository.AddAsync(employee);
                int count = await unitOfWork.SaveChangesAsync();
                if (count > 0)
                    return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Invalid Opearation!");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details([FromRoute] int? id, string viewName = "Details")
        {
            if (id is null) return BadRequest("Invalid Operation!");
            var employee = await unitOfWork.EmployeeRepository.GetAsync(id.Value);
            if (employee is null) return NotFound($"There Is No Employee With Id:{id}");
            var dto = mapper.Map<EmployeeDto>(employee);
            TempData["Id"] = id;
            if(employee.Department is not null)
                TempData["department"] = employee.Department.Name;

            return View(viewName, dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int? id)
        {
            var departments = await unitOfWork.DepartmentRepository.GetAllAsync();
            ViewData["dapartments"] = departments;
            return await Details(id, "Edit");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int? id, EmployeeDto model)
        {
            if (ModelState.IsValid)
            {
                if (model.ImageName is not null && model.Image is not null)
                {
                    DocumentSettings.DeleteFile(model.ImageName, "images");
                }
                if (model.Image is not null)
                {
                    model.ImageName = DocumentSettings.UploadFile(model.Image, "images");
                }
                var DId = TempData["Id"];
                if ((int)DId != id.Value) return BadRequest("Invalid Operation!");
                var employee = mapper.Map<Employee>(model);
                employee.Id = id.Value;
                unitOfWork.EmployeeRepository.Update(employee);
                int count = await unitOfWork.SaveChangesAsync();
                if (count > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            ModelState.AddModelError("", "Invalid Operation!");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            return await Details(id, "Delete");
        }
        [HttpPost]
        [ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmDelete([FromRoute] int? id)
        {
            var DId = TempData["Id"];
            if ((int)DId != id.Value) return BadRequest("Invalid Operation!");
            if (id is null)
                return BadRequest("Invalid Id");
            var employee = await unitOfWork.EmployeeRepository.GetAsync(id.Value);
            if (employee is null)
                return NotFound($"Employee With Id {id} Is Not Found");
            if (employee.ImageName is not null)
            {
                DocumentSettings.DeleteFile(employee.ImageName, "images");
            }
            unitOfWork.EmployeeRepository.Delete(employee);
            int count = await unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
