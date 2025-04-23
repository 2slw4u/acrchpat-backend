using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Models;
using System.Security.Claims;
using UserService.Models.Entities;
using UserService.Models.ViewModels;
using UserService.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using UserService.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace UserService.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly UserManager<UserEntity> _userManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly AppDbContext _context;
        private readonly ILogger<AccountController> _logger;


        public AccountController(
            AppDbContext context,
            SignInManager<UserEntity> signInManager,
            UserManager<UserEntity> userManager,
            IIdentityServerInteractionService interaction,
            ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _interaction = interaction;
            _context = context;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var claims = HttpContext.User.Claims;
            var sub = claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var name = claims.FirstOrDefault(c => c.Type == "name")?.Value;
            return Ok(new { Sub = sub, Name = name });
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string button)
        {
            if (button != "login")
            {
                var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
                if (context != null)
                {
                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    return Redirect("~/");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            _logger.LogInformation(model.Phone);
            _logger.LogInformation(model.Password);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.Phone);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login credentials.");
                return View(model);
            }
            _logger.LogInformation(user.Id.ToString());


            var verificationResult = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
            if (verificationResult != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError(string.Empty, "Invalid login credentials.");
                return View(model);
            }
            _logger.LogInformation("Password ok");
            _logger.LogInformation(model.ReturnUrl);

            await _signInManager.SignInAsync(user, isPersistent: false);

            _logger.LogInformation("signin successful");

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            return Redirect("~/");
        }
    }
}


