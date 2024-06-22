using Microsoft.AspNetCore.Mvc;

namespace JobFind.ViewModel.Category
{
    public class AccountController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }
    }
}
