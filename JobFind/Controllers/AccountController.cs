using JobFind.Enums;
using JobFind.Models;
using JobFind.ViewModel.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace JobFind.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _loginManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> loginManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _loginManager = loginManager;
            _roleManager = roleManager;
        }

        [Route("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(registerVM regVM)
        {
            if (!ModelState.IsValid)
            {
                return View(regVM);
            }

            AppUser user = new AppUser
            {
                Name = regVM.Name,
                Surname = regVM.Surname,
                UserName = regVM.Username,
                Email = regVM.Email,
            };

            IdentityResult result = await _userManager.CreateAsync(user, regVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(regVM);
            }

            await _userManager.AddToRoleAsync(user, UserRoles.Member.ToString());
            return RedirectToAction(nameof(Login));
        }

        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginVM LogVM)
        {
            if (!ModelState.IsValid)
            {
                return View(LogVM);
            }

            AppUser user = await _userManager.FindByNameAsync(LogVM.UsernameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(LogVM.UsernameOrEmail);
                if (user == null)
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre yanlış.");
                    return View(LogVM);
                }
            }

            var result = await _loginManager.PasswordSignInAsync(user, LogVM.Password, LogVM.RememberMe, true);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Hesap kilitlendi, lütfen daha sonra tekrar deneyin.");
                return View(LogVM);
            }

            ModelState.AddModelError("", "Geçersiz giriş denemesi.");
            return View(LogVM);
        }

        
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            await _loginManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Route("createroles")]
        public async Task<IActionResult> CreateRoles()
        {
            foreach (var item in Enum.GetValues(typeof(UserRoles)))
            {
                if (!await _roleManager.RoleExistsAsync(item.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = item.ToString(),
                    });
                }
            }
            return Content("Ok");
        }
    }
}