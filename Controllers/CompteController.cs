using GestionStock.Data;
using GestionStock.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestionStock.Controllers
{
    public class CompteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompteController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string motDePasse)
        {
            var utilisateur = _context.Utilisateurs.FirstOrDefault(u => u.Email == email);

            if (utilisateur != null)
            {
                var hasher = new PasswordHasher<Utilisateur>();
                var result = hasher.VerifyHashedPassword(utilisateur, utilisateur.MotDePasse, motDePasse);

                if (result == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, utilisateur.Nom),
                        new Claim(ClaimTypes.Email, utilisateur.Email),
                        new Claim(ClaimTypes.Role, utilisateur.Role.ToString())
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    TempData["Success"] = "Connexion réussie !";
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Erreur = "Email ou mot de passe incorrect.";
            return View("Index");
        }

        [HttpPost]
        public IActionResult Register(Utilisateur utilisateur)
        {
            if (ModelState.IsValid)
            {
                if (_context.Utilisateurs.Any(u => u.Email == utilisateur.Email))
                {
                    ViewBag.Erreur = "Cet email est déjà utilisé.";
                    return View("Index");
                }

                // Hash du mot de passe avant de sauvegarder
                var hasher = new PasswordHasher<Utilisateur>();
                utilisateur.MotDePasse = hasher.HashPassword(utilisateur, utilisateur.MotDePasse);

                utilisateur.Role = Role.Admin; // Ou Role.Utilisateur
                _context.Utilisateurs.Add(utilisateur);
                _context.SaveChanges();

                TempData["Success"] = "Inscription réussie ! Connectez-vous.";
                return RedirectToAction("Index");
            }

            return View("Index", utilisateur);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "Déconnexion réussie.";
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
