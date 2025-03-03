using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopWebApplication.Models;
using ShopWebApplication.ViewModels;

namespace ShopWebApplication.Controllers;

[Route("account")]
public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }
    
    [HttpGet("register")]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Користувач з таким Email вже існує.");
                return View(model);
            }

            var role = await _roleManager.FindByNameAsync("user");

            if (role == null)
            {
                ModelState.AddModelError(string.Empty, "Не можна створити юзера, через внутрішньо сервісні неполадки");
            }
            
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            
            var result = await _userManager.CreateAsync(user, model.Password);

            //var isRoleAdded = await _userManager.AddToRoleAsync(user, "user");
            
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            
            /*foreach (var error in isRoleAdded.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }*/
        }

        return View(model);
    }
    
    [HttpGet("login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Невірний логін або пароль.");
            return View(model);
        }
        
        return View(model);
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
