using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.DAL.Models;
using WebApp.PL.Dtos;

namespace WebApp.PL.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> userManager;

        public UserController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string? searchInput)
        {
            IEnumerable<UserToReturnDto> users;
            if (!string.IsNullOrEmpty(searchInput))
            {
                users = userManager.Users.Select(u => new UserToReturnDto()
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Roles = userManager.GetRolesAsync(u).Result
                }).Where(u => u.UserName.ToLower().Contains(searchInput.ToLower()));
            }
            else
                users = userManager.Users.Select(u => new UserToReturnDto()
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Roles = userManager.GetRolesAsync(u).Result
                });

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string? id, string viewName = "Details")
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Invalid Id !");
            var user = await userManager.FindByIdAsync(id);
            if (user is null) return NotFound($"user With Id {id} Is Not Found");
            var dto = new UserToReturnDto()
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = userManager.GetRolesAsync(user).Result
            };

            return View(viewName, dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] string? id)
        {
            return await Details(id, "Edit");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] string id, UserToReturnDto model)
        {
            if (ModelState.IsValid)
            {
                if (id != model.Id) return BadRequest("Invalid Operation");
                var user = await userManager.FindByIdAsync(id);
                if (user is not null)
                {
                    user.UserName = model.UserName;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.Id = id;
                }

                var result = await userManager.UpdateAsync(user);
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
            var user = await userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound($"User With Id {id} Is Not Found");

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
                return RedirectToAction(nameof(Index));
            return View(nameof(Delete), id);
        }
    }
}
