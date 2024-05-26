using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FFMonitorWeb.Models;
using FFMonitorWeb.Data;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace FFMonitorWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly MongoDbContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, MongoDbContext context, ILogger<AccountController> logger, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context; // Ensure this is properly assigned
            _logger = logger;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Login()
        {
            _logger.LogInformation("Login GET action called");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            _logger.LogInformation("Login POST action called with email: {Email}", email);

            /*Additional logging for debugging
            _logger.LogInformation("Attempting to find user by email: {Email}", email);

            // Use the custom method for debugging
            var debugUser = await _context.FindUserByEmailAsync(email);
            _logger.LogInformation("Custom method found user: {DebugUser}", debugUser);

            // Original UserManager method for comparison
            //var user = await _userManager.FindByEmailAsync(email);*/
            // Fetch all users from the database
            var allUsers = await _context.AppUsers.Find(_ => true).ToListAsync();
            if (allUsers.Count == 0)
            {
                _logger.LogWarning("No users found in the database.");
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View();
            }

            // Perform in-memory case-insensitive comparison
            _logger.LogInformation("Performing in-memory case-insensitive comparison for email: {Email}", email);
            var user = allUsers.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (user != null)
            {
                _logger.LogInformation("User found with email: {Email}", email);

                // Validate the user's password using custom PasswordHasher
                _logger.LogInformation("Validating password for user: {Email}", email);
                if (PasswordHasher.VerifyPassword(user.Password, password))
                {
                    _logger.LogInformation("Password verified for user: {Email}", email);

                    // Additional user checks before signing in
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        _logger.LogWarning("Email not confirmed for user: {Email}", email);
                        ModelState.AddModelError(string.Empty, "You must confirm your email to log in.");
                        return View();
                    }

                    if (await _userManager.IsLockedOutAsync(user))
                    {
                        _logger.LogWarning("User account locked out: {Email}", email);
                        ModelState.AddModelError(string.Empty, "Your account is locked out.");
                        return View();
                    }

                    _logger.LogInformation("Attempting to sign in user: {Email} using SignInManager", email);
                    var signInResult = await _signInManager.PasswordSignInAsync(user.UserName, password, isPersistent: false, lockoutOnFailure: false);

                    if (signInResult.Succeeded)
                    {
                        _logger.LogInformation("User {Email} signed in successfully using SignInManager.", email);
                        return RedirectToAction("Index", "Home");
                    }
                    else if (signInResult.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out: {Email}", email);
                        ModelState.AddModelError(string.Empty, "User account locked out.");
                    }
                    else if (signInResult.RequiresTwoFactor)
                    {
                        _logger.LogWarning("Two-factor authentication required for user: {Email}", email);
                        return RedirectToAction("LoginWith2fa", new { ReturnUrl = "/", RememberMe = false });
                    }
                    else if (signInResult.IsNotAllowed)
                    {
                        _logger.LogWarning("User not allowed to sign in: {Email}", email);
                        ModelState.AddModelError(string.Empty, "User not allowed to sign in.");
                    }
                    else
                    {
                        _logger.LogWarning("Failed to sign in user: {Email} using SignInManager. SignInResult: {SignInResult}", email, signInResult);
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    }

                    _logger.LogWarning("SignInResult details for user: {Email} - Succeeded: {Succeeded}, IsLockedOut: {IsLockedOut}, RequiresTwoFactor: {RequiresTwoFactor}, IsNotAllowed: {IsNotAllowed}", 
                        email, signInResult.Succeeded, signInResult.IsLockedOut, signInResult.RequiresTwoFactor, signInResult.IsNotAllowed);
                }
                else
                {
                    _logger.LogWarning("Invalid password attempt for user: {Email}", email);
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            else
            {
                _logger.LogWarning("No user found with email: {Email}", email);
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View();

        }

        [HttpGet]
        public IActionResult Register()
        {
            _logger.LogInformation("Register GET action called");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            _logger.LogInformation("Register POST action called with email: {Email}", model.Email);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid");
                return View(model);
            }

            var user = new User
            {
                UserName = model.Email, // Using Email as Username
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User registered successfully with email: {Email}", model.Email);

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

                        // Send email
                await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

                _logger.LogInformation("Sent email confirmation link to user: {Email}", model.Email);

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                _logger.LogError("Error during user registration: {Error}", error.Description);
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }


        [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (userId == null || token == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            _logger.LogInformation("Email confirmed for user: {Email}", user.Email);
            return View("ConfirmEmail");
        }

        _logger.LogError("Error confirming email for user: {Email}", user.Email);
        return View("Error");
    }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Logout action called");
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
