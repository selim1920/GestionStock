using GestionStock.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GestionStock.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore; // 👈 Nécessaire pour .Include
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionStock.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            int nombreEntrepots = _context.Entrepots.Count();
            int nombreProduits = _context.Produits.Count();
            int nombreCategories = _context.Categories.Count();
            int nombreUtilisateurs = _context.Utilisateurs.Count(); // ✅ Correction ici

            var stockData = _context.Stocks
                .Include(s => s.Entrepot)
                .GroupBy(s => s.Entrepot.Nom)
                .Select(g => new
                {
                    Entrepot = g.Key,
                    TotalStock = g.Sum(s => s.Quantite)
                })
                .ToList();

            ViewBag.Produits = new SelectList(_context.Produits.ToList(), "Id", "Nom");

            var produitStockData = _context.Stocks
                .Include(s => s.Produit)
                .GroupBy(s => s.Produit.Nom)
                .Select(g => new
                {
                    Produit = g.Key,
                    QuantiteTotale = g.Sum(s => s.Quantite)
                }).ToList();

            ViewBag.NombreEntrepots = nombreEntrepots;
            ViewBag.NombreProduits = nombreProduits;
            ViewBag.StockData = stockData;
            ViewBag.NombreCategories = nombreCategories;
            ViewBag.NombreUtilisateurs = nombreUtilisateurs;
            ViewBag.ProduitStockData = produitStockData;

            return View();
        }

        [HttpGet]
        public JsonResult GetProduitStockByProduit(int produitId)
        {
            Console.WriteLine($"Filtrage des stocks pour Produit ID : {produitId}");

            var data = _context.Stocks
                .Include(s => s.Produit)
                .Where(s => s.ProduitId == produitId)
                .GroupBy(s => s.Produit.Nom)
                .Select(g => new
                {
                    Produit = g.Key,
                    QuantiteTotale = g.Sum(s => s.Quantite)
                })
                .ToList();

            Console.WriteLine($"Données filtrées : {data.Count} produits trouvés");

            return Json(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
