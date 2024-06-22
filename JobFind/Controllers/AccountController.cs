using JobFind.Models;
using JobFind.ViewModel.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

    namespace JobFind.Controllers
    {
        [Route("account")]
        public class AccountController(UserManager<AppUser> _userManager,SignInManager<AppUser> _loginManeger) : Controller
        {
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
                    Email = regVM.Email,
                    UserName = regVM.Name,

                };
                IdentityResult result=await _userManager.CreateAsync(user,regVM.Password);
                 if (!result.Succeeded) 
                {
                    foreach (var item in result.Errors)
                    {

                        ModelState.AddModelError("", item.Description);
                    }
                
                }
                 return RedirectToAction("Login");


           

            
            }
        [Route("login")]
            public async Task<IActionResult> Login()
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
            AppUser? user = await _userManager.FindByNameAsync(LogVM.UsernameOrEmail);
            if (User==null) 
            {
                user = await _userManager.FindByEmailAsync(LogVM.UsernameOrEmail);
                if (user==null)
                {
                    ModelState.AddModelError("", "İstifadəçi adı və ya şifrəsi yalnışdır");
                    return View(LogVM);
                }

            }
            var result=await _loginManeger.PasswordSignInAsync(user, LogVM.Password,LogVM.RememberMe,true);
            if (result.Succeeded) 
            {
                ModelState.AddModelError("", "Çox sayda yalnış dəyər göndərdiniz. Zəhmət olmasa gözləyin - " + user.LockoutEnd.Value.ToString("HH:mm:ss"));
                return View(LogVM);
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _loginManeger.SignOutAsync();
            return RedirectToAction("login");
            return View();
        }
        

      
    }
}
