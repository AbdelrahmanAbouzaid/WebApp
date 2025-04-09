using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WebApp.BLL.Interfaces;
using WebApp.DAL.Models;
using WebApp.PL.Dtos;

namespace WebApp.PL.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public DepartmentController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var departments = await unitOfWork.DepartmentRepository.GetAllAsync();
            return View(departments);
        }

        [HttpGet]
        public IActionResult Create() 
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentDto model)
        {
            if (ModelState.IsValid)
            {
                var department = mapper.Map<Department>(model);
                await unitOfWork.DepartmentRepository.AddAsync(department);
                int result = await unitOfWork.SaveChangesAsync();
                if (result > 0)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            ModelState.AddModelError("","Invalid Operation!");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id, string viewName = "Details")
        {
            if(id is null) return BadRequest("Invalid Id");
            var department = await unitOfWork.DepartmentRepository.GetAsync(id.Value);
            if (department == null) return NotFound($"There Is No Department With {id}");
            var dto = mapper.Map<DepartmentDto>(department);

            TempData["Id"] = id;

            return View(viewName,dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            return await Details(id,"Edit");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute]int? id,DepartmentDto model)
        {
            if (ModelState.IsValid)
            {
                var DId = TempData["Id"];
                if ((int)DId != id.Value) return BadRequest("Invalid Operation!");
         
                var department = mapper.Map<Department>(model);
                department.Id = id.Value;
                unitOfWork.DepartmentRepository.Update(department);
                await unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("","Invalid Operation!");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            return await Details(id,"Delete");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute]int? id,DepartmentDto model)
        {
            if (!ModelState.IsValid)
            {
                var DId = TempData["Id"];
                if ((int)DId != id.Value) return BadRequest("Invalid Operation!");
                var department = mapper.Map<Department>(model);
                department.Id = id.Value;
                unitOfWork.DepartmentRepository.Delete(department);
                await unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}
