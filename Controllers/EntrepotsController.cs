using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionStock.Data;
using GestionStock.Models;
using Microsoft.AspNetCore.Authorization;

namespace GestionStock.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EntrepotsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EntrepotsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ GET: Entrepots (avec pagination, recherche, stocks et mouvements)
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            int pageSize = 5;

            var query = _context.Entrepots
                .Include(e => e.Stocks)
                    .ThenInclude(s => s.Produit)
                .Include(e => e.MouvementsStock) // ✅ inclusion des mouvements
                    .ThenInclude(m => m.Produit)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(e => e.Nom.Contains(searchString));
            }

            int totalCount = await query.CountAsync();

            var entrepots = await query
                .OrderBy(e => e.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Dictionary<int, int> capacitesRestantes = new Dictionary<int, int>();
            foreach (var entrepot in entrepots)
            {
                int totalQuantite = entrepot.Stocks.Sum(s => s.Quantite);
                int capaciteRestante = entrepot.CapaciteDisponible - totalQuantite;
                capacitesRestantes[entrepot.Id] = capaciteRestante;
            }

            ViewData["CapacitesRestantes"] = capacitesRestantes;
            ViewData["SearchString"] = searchString;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalCount / pageSize);

            return View(entrepots);
        }

        // ✅ GET: Entrepots/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var entrepot = await _context.Entrepots
                .Include(e => e.Stocks)
                    .ThenInclude(s => s.Produit)
                .Include(e => e.MouvementsStock)
                    .ThenInclude(m => m.Produit)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entrepot == null)
                return NotFound();

            return View(entrepot);
        }

        // ✅ GET: Entrepots/Create
        public IActionResult Create()
        {
            return View();
        }

        // ✅ POST: Entrepots/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nom,Adresse,Capacite")] Entrepot entrepot)
        {
            if (ModelState.IsValid)
            {
                entrepot.InitialiserCapaciteDisponible();
                entrepot.CalculerNombreRayons();

                _context.Add(entrepot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(entrepot);
        }

        // ✅ GET: Entrepots/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var entrepot = await _context.Entrepots.FindAsync(id);
            if (entrepot == null)
                return NotFound();

            return View(entrepot);
        }

        // ✅ POST: Entrepots/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,Adresse,Capacite")] Entrepot entrepot)
        {
            if (id != entrepot.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    entrepot.CalculerNombreRayons();
                    _context.Update(entrepot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntrepotExists(entrepot.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(entrepot);
        }

        // ✅ GET: Entrepots/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var entrepot = await _context.Entrepots
                .FirstOrDefaultAsync(m => m.Id == id);
            if (entrepot == null)
                return NotFound();

            return View(entrepot);
        }

        // ✅ POST: Entrepots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entrepot = await _context.Entrepots.FindAsync(id);
            if (entrepot != null)
            {
                _context.Entrepots.Remove(entrepot);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EntrepotExists(int id)
        {
            return _context.Entrepots.Any(e => e.Id == id);
        }
    }
}
