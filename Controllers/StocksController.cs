using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionStock.Data;
using GestionStock.Models;
using Microsoft.AspNetCore.Authorization;

namespace GestionStock.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StocksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StocksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Stocks
        public async Task<IActionResult> Index(string produit, string entrepot)
        {
            var stocksQuery = _context.Stocks
                .Include(s => s.Entrepot)
                .Include(s => s.Produit)
                .AsQueryable();

            if (!string.IsNullOrEmpty(produit))
                stocksQuery = stocksQuery.Where(s => s.Produit.Nom.Contains(produit));

            if (!string.IsNullOrEmpty(entrepot))
                stocksQuery = stocksQuery.Where(s => s.Entrepot.Nom.Contains(entrepot));

            var stocks = await stocksQuery.ToListAsync();

            var alertMessages = stocks
                .Where(s => s.Quantite <= s.SeuilAlerte)
                .Select(s => $"⚠️ Le stock du produit '{s.Produit?.Nom}' dans l'entrepôt '{s.Entrepot?.Adresse}' est faible ({s.Quantite} ≤ seuil : {s.SeuilAlerte}).")
                .ToList();

            ViewData["AlertMessages"] = alertMessages;
            ViewData["Produits"] = new SelectList(await _context.Produits.ToListAsync(), "Nom", "Nom");
            ViewData["Entrepots"] = new SelectList(await _context.Entrepots.ToListAsync(), "Nom", "Nom");

            return View(stocks);
        }

        // GET: Stocks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var stock = await _context.Stocks
                .Include(s => s.Entrepot)
                .Include(s => s.Produit)
                .FirstOrDefaultAsync(m => m.Id == id);

            return stock == null ? NotFound() : View(stock);
        }

        // GET: Stocks/Create
        public IActionResult Create()
        {
            ViewData["EntrepotId"] = new SelectList(_context.Entrepots, "Id", "Nom");
            ViewData["ProduitId"] = new SelectList(_context.Produits, "Id", "Nom");
            return View();
        }

        // POST: Stocks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProduitId,EntrepotId,SeuilAlerte,Rayon,Quantite")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                var entrepot = await _context.Entrepots.FindAsync(stock.EntrepotId);
                if (entrepot != null)
                {
                    var totalQuantite = _context.Stocks
                        .Where(s => s.EntrepotId == stock.EntrepotId)
                        .Sum(s => s.Quantite);

                    if (entrepot.Capacite - totalQuantite < stock.Quantite)
                    {
                        ModelState.AddModelError("Quantite", $"Capacité restante insuffisante ({entrepot.Capacite - totalQuantite} unités).");
                        ViewData["EntrepotId"] = new SelectList(_context.Entrepots, "Id", "Nom", stock.EntrepotId);
                        ViewData["ProduitId"] = new SelectList(_context.Produits, "Id", "Nom", stock.ProduitId);
                        return View(stock);
                    }

                    var quantiteRayon = _context.Stocks
                        .Where(s => s.EntrepotId == stock.EntrepotId && s.Rayon == stock.Rayon && s.ProduitId == stock.ProduitId)
                        .Sum(s => s.Quantite);

                    if (quantiteRayon + stock.Quantite > 50)
                    {
                        ModelState.AddModelError("Quantite", "La quantité dans ce rayon dépasse la limite de 50.");
                        ViewData["EntrepotId"] = new SelectList(_context.Entrepots, "Id", "Nom", stock.EntrepotId);
                        ViewData["ProduitId"] = new SelectList(_context.Produits, "Id", "Nom", stock.ProduitId);
                        return View(stock);
                    }
                }

                _context.Add(stock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EntrepotId"] = new SelectList(_context.Entrepots, "Id", "Nom", stock.EntrepotId);
            ViewData["ProduitId"] = new SelectList(_context.Produits, "Id", "Nom", stock.ProduitId);
            return View(stock);
        }

        // GET: Stocks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null) return NotFound();

            ViewData["EntrepotId"] = new SelectList(_context.Entrepots, "Id", "Nom", stock.EntrepotId);
            ViewData["ProduitId"] = new SelectList(_context.Produits, "Id", "Nom", stock.ProduitId);
            return View(stock);
        }

        // POST: Stocks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProduitId,EntrepotId,SeuilAlerte,Rayon,Quantite")] Stock stock)
        {
            if (id != stock.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var entrepot = await _context.Entrepots.FindAsync(stock.EntrepotId);
                    if (entrepot != null)
                    {
                        var totalQuantite = _context.Stocks
                            .Where(s => s.EntrepotId == stock.EntrepotId && s.Id != stock.Id)
                            .Sum(s => s.Quantite);

                        if (entrepot.Capacite - totalQuantite < stock.Quantite)
                        {
                            ModelState.AddModelError("Quantite", $"Capacité restante insuffisante ({entrepot.Capacite - totalQuantite} unités).");
                            ViewData["EntrepotId"] = new SelectList(_context.Entrepots, "Id", "Nom", stock.EntrepotId);
                            ViewData["ProduitId"] = new SelectList(_context.Produits, "Id", "Nom", stock.ProduitId);
                            return View(stock);
                        }

                        var quantiteRayon = _context.Stocks
                            .Where(s => s.EntrepotId == stock.EntrepotId && s.Rayon == stock.Rayon && s.ProduitId == stock.ProduitId && s.Id != stock.Id)
                            .Sum(s => s.Quantite);

                        if (quantiteRayon + stock.Quantite > 50)
                        {
                            ModelState.AddModelError("Quantite", "La quantité dans ce rayon dépasse la limite de 50.");
                            ViewData["EntrepotId"] = new SelectList(_context.Entrepots, "Id", "Nom", stock.EntrepotId);
                            ViewData["ProduitId"] = new SelectList(_context.Produits, "Id", "Nom", stock.ProduitId);
                            return View(stock);
                        }
                    }

                    _context.Update(stock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockExists(stock.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["EntrepotId"] = new SelectList(_context.Entrepots, "Id", "Nom", stock.EntrepotId);
            ViewData["ProduitId"] = new SelectList(_context.Produits, "Id", "Nom", stock.ProduitId);
            return View(stock);
        }

        // GET: Stocks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var stock = await _context.Stocks
                .Include(s => s.Entrepot)
                .Include(s => s.Produit)
                .FirstOrDefaultAsync(m => m.Id == id);

            return stock == null ? NotFound() : View(stock);
        }

        // POST: Stocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock != null)
            {
                _context.Stocks.Remove(stock);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool StockExists(int id)
        {
            return _context.Stocks.Any(e => e.Id == id);
        }

        // GET: AJAX - Rayons disponibles
        [HttpGet]
        public JsonResult GetRayons(int entrepotId)
        {
            var entrepot = _context.Entrepots.FirstOrDefault(e => e.Id == entrepotId);
            if (entrepot != null)
            {
                var rayons = new List<int>();
                for (int i = 1; i <= entrepot.NombreRayons; i++)
                {
                    rayons.Add(i);
                }
                return Json(rayons);
            }
            return Json(new List<int>());
        }
    }
}
