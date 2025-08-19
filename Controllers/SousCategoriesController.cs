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
    public class SousCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SousCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SousCategories
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SousCategories.Include(s => s.Categorie);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SousCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sousCategorie = await _context.SousCategories
                .Include(s => s.Categorie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sousCategorie == null)
            {
                return NotFound();
            }

            return View(sousCategorie);
        }

        // GET: SousCategories/Create
        public IActionResult Create()
        {
            ViewData["CategorieId"] = new SelectList(_context.Categories, "Id", "Nom");
            return View();
        }

        // POST: SousCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nom,CategorieId")] SousCategorie sousCategorie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sousCategorie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategorieId"] = new SelectList(_context.Categories, "Id", "Nom", sousCategorie.CategorieId);
            return View(sousCategorie);
        }

        // GET: SousCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sousCategorie = await _context.SousCategories.FindAsync(id);
            if (sousCategorie == null)
            {
                return NotFound();
            }
            ViewData["CategorieId"] = new SelectList(_context.Categories, "Id", "Nom", sousCategorie.CategorieId);
            return View(sousCategorie);
        }

        // POST: SousCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,CategorieId")] SousCategorie sousCategorie)
        {
            if (id != sousCategorie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sousCategorie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SousCategorieExists(sousCategorie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategorieId"] = new SelectList(_context.Categories, "Id", "Nom", sousCategorie.CategorieId);
            return View(sousCategorie);
        }

        // GET: SousCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sousCategorie = await _context.SousCategories
                .Include(s => s.Categorie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sousCategorie == null)
            {
                return NotFound();
            }

            return View(sousCategorie);
        }

        // POST: SousCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sousCategorie = await _context.SousCategories.FindAsync(id);
            if (sousCategorie != null)
            {
                _context.SousCategories.Remove(sousCategorie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SousCategorieExists(int id)
        {
            return _context.SousCategories.Any(e => e.Id == id);
        }
    }
}
