using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.DAL.Models;
using WebApp.PL.Dtos;

namespace WebApp.PL.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        [HttpGet]
        public IActionResult Index(string? searchInput)
        {
            IEnumerable<RoleToReturnDto> roles;
            if (!string.IsNullOrEmpty(searchInput))
            {
                roles = roleManager.Roles.Select(r => new RoleToReturnDto()
                {
                    Id = r.Id,
                    Name = r.Name,

                }).Where(r => r.Name.ToLower().Contains(searchInput.ToLower()));
            }
            else
                roles = roleManager.Roles.Select(r => new RoleToReturnDto()
                {
                    Id = r.Id,
                    Name = r.Name,
                });

            return View(roles);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RoleToReturnDto model)
        {
            if (ModelState.IsValid)
            {
                var role = await roleManager.FindByNameAsync(model.Name);
                if (role is null)
                {
                    var roleResult = new IdentityRole()
                    {
                        Name = model.Name
                    };
                    var result = await roleManager.CreateAsync(roleResult);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            ModelState.AddModelError("", "invalid Operation");
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Details(string? id, string viewName = "Details")
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Invalid Id !");
            var role = await roleManager.FindByIdAsync(id);
            if (role is null) return NotFound($"user With Id {id} Is Not Found");
            var dto = new RoleToReturnDto()
            {
                Id = role.Id,
                Name = role.Name,
            };
            TempData["id"] = id;
            return View(viewName, dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit( string? id)
        {
            return await Details(id,"Edit");
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string id, RoleToReturnDto model)
        {
            if (ModelState.IsValid)
            {
                if (id != model.Id) return BadRequest("Invalid Operation");
                var role = await roleManager.FindByIdAsync(id);
                if (role is not null)
                {
                    role.Id = id;
                    role.Name = model.Name;
                }

                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded) return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "invalid Update");
            return View(model);
        }

        public async Task<IActionResult> Delete(string? id)
        {
            return await Details(id, "Delete");
        }

        [HttpPost]
        [ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmDelete([FromRoute] string? id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Invalid Id !");
            var role = await roleManager.FindByIdAsync(id);
            if (role is null)
                return NotFound($"User With Id {id} Is Not Found");

            await roleManager.DeleteAsync(role);

            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult> AddOrRemoveUsers(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role is null) return NotFound();
            var users = await userManager.Users.ToListAsync();
            var userInRole = new List<UserInRoleDto>();
            foreach (var user in users)
            {
                var dto = new UserInRoleDto()
                {
                    UserId = user.Id,
                    UserName = user.UserName,

                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                    dto.IsSelected = true;
                else
                    dto.IsSelected = false;
                userInRole.Add(dto);
            }

            return View(userInRole);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrRemoveUsers(string id, List<UserInRoleDto> users)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role is null) return NotFound();
            if (ModelState.IsValid)
            {
                foreach (var user in users)
                {
                    var appUser = await userManager.FindByIdAsync(user.UserId);
                    if (appUser is not null)
                    {
                        if (user.IsSelected && !await userManager.IsInRoleAsync(appUser, role.Name))
                        {
                            await userManager.AddToRoleAsync(appUser, role.Name);
                        }
                        else if (!user.IsSelected && await userManager.IsInRoleAsync(appUser, role.Name))
                        {
                            await userManager.RemoveFromRoleAsync(appUser, role.Name);
                        }
                    }

                }

                return RedirectToAction(nameof(Index), new { Id = id });
            }
            return View(users);
        }
    }
}
