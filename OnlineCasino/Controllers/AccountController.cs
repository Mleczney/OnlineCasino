using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineCasino.Application.DTOs;
using OnlineCasino.Application.Interfaces;

namespace OnlineCasino.Controllers
{
    public class AccountController : Controller
    {
        private readonly IPlayerService _playerService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(
            IPlayerService playerService,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _playerService = playerService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterPlayerDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Create player in custom Player table
                    var player = await _playerService.CreateAsync(dto);

                    // Create Identity user
                    var identityUser = new IdentityUser
                    {
                        UserName = dto.Username,
                        Email = dto.Email
                    };

                    var result = await _userManager.CreateAsync(identityUser, dto.Password);

                    if (result.Succeeded)
                    {
                        // Assign Player role
                        await _userManager.AddToRoleAsync(identityUser, "Player");

                        // Sign in the user
                        await _signInManager.SignInAsync(identityUser, isPersistent: false);

                        // Store player ID in session
                        HttpContext.Session.SetInt32("PlayerId", player.Id);

                        return RedirectToAction("Index", "Home");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(dto);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(dto.Username, dto.Password, false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Get the user to check roles
                    var user = await _userManager.FindByNameAsync(dto.Username);
                    if (user != null)
                    {
                        // Check user roles and set session accordingly
                        if (await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                            HttpContext.Session.SetString("Role", "Admin");
                            HttpContext.Session.SetString("Username", dto.Username);
                            return RedirectToAction("Index", "Home", new { area = "Admin" });
                        }
                        else if (await _userManager.IsInRoleAsync(user, "Manager"))
                        {
                            HttpContext.Session.SetString("Role", "Manager");
                            HttpContext.Session.SetString("Username", dto.Username);
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            // Get player from custom table for regular players
                            var player = await _playerService.GetByUsernameAsync(dto.Username);
                            if (player != null)
                            {
                                HttpContext.Session.SetInt32("PlayerId", player.Id);
                                HttpContext.Session.SetString("Username", player.Username);
                                HttpContext.Session.SetString("Balance", player.Balance.ToString());
                                HttpContext.Session.SetString("Role", "Player");
                            }
                            else
                            {
                                // Player role but no player record - sign out and show error
                                await _signInManager.SignOutAsync();
                                ModelState.AddModelError(string.Empty, "Profil hráče nebyl nalezen. Kontaktujte administrátora.");
                                return View(dto);
                            }
                        }
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Neplatné přihlašovací údaje");
            }

            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutPost()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var playerId = HttpContext.Session.GetInt32("PlayerId");
            if (playerId == null)
            {
                return RedirectToAction("Login");
            }

            var player = await _playerService.GetByIdAsync(playerId.Value);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Deposit()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit(DepositDto dto)
        {
            if (ModelState.IsValid)
            {
                var playerId = HttpContext.Session.GetInt32("PlayerId");
                if (playerId == null)
                {
                    return RedirectToAction("Login");
                }

                var success = await _playerService.DepositAsync(playerId.Value, dto.Amount);
                if (success)
                {
                    var balance = await _playerService.GetBalanceAsync(playerId.Value);
                    HttpContext.Session.SetString("Balance", balance.ToString());

                    TempData["Message"] = $"Vklad {dto.Amount} Kč byl úspěšný!";
                    return RedirectToAction("Profile");
                }

                ModelState.AddModelError(string.Empty, "Vklad se nezdařil");
            }

            return View(dto);
        }
    }
}
