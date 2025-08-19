// ✅ ProduitsController.cs FINAL avec recherche, filtres, tri, pagination et export Excel
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GestionStock.Data;
using GestionStock.Models;
using ClosedXML.Excel;
using System.IO;

namespace GestionStock.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProduitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProduitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, int? sousCategorieId, string fournisseur, int? seuilMin, string sortOrder, int page = 1)
        {
            int pageSize = 5;

            var produitsQuery = _context.Produits.Include(p => p.SousCategorie).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
                produitsQuery = produitsQuery.Where(p => p.Nom.Contains(searchString) || p.Fournisseur.Contains(searchString) || p.SousCategorie.Nom.Contains(searchString));

            if (sousCategorieId.HasValue && sousCategorieId.Value != 0)
                produitsQuery = produitsQuery.Where(p => p.SousCategorieId == sousCategorieId);

            if (!string.IsNullOrEmpty(fournisseur))
                produitsQuery = produitsQuery.Where(p => p.Fournisseur == fournisseur);

            if (seuilMin.HasValue)
                produitsQuery = produitsQuery.Where(p => p.SeuilMin >= seuilMin);

            ViewData["SortOrder"] = sortOrder;
            produitsQuery = sortOrder switch
            {
                "prix_desc" => produitsQuery.OrderByDescending(p => p.Prix),
                "prix_asc" => produitsQuery.OrderBy(p => p.Prix),
                "nom_desc" => produitsQuery.OrderByDescending(p => p.Nom),
                "seuil_asc" => produitsQuery.OrderBy(p => p.SeuilMin),
                "seuil_desc" => produitsQuery.OrderByDescending(p => p.SeuilMin),
                _ => produitsQuery.OrderBy(p => p.Nom),
            };

            int totalCount = await produitsQuery.CountAsync();
            var produits = await produitsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var sousCategories = await _context.SousCategories.OrderBy(c => c.Nom).ToListAsync();
            var fournisseurs = await _context.Produits.Select(p => p.Fournisseur).Where(f => f != null).Distinct().OrderBy(f => f).ToListAsync();

            ViewData["SousCategorieId"] = new SelectList(sousCategories, "Id", "Nom", sousCategorieId);
            ViewData["Fournisseurs"] = new SelectList(fournisseurs);
            ViewData["SeuilMin"] = seuilMin;
            ViewData["SearchString"] = searchString;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalCount / pageSize);

            return View(produits);
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var produits = await _context.Produits.Include(p => p.SousCategorie).OrderBy(p => p.Nom).ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Produits");

            worksheet.Cell(1, 1).Value = "Nom";
            worksheet.Cell(1, 2).Value = "Prix";
            worksheet.Cell(1, 3).Value = "Fournisseur";
            worksheet.Cell(1, 4).Value = "Sous-Catégorie";
            worksheet.Cell(1, 5).Value = "Seuil Min";

            for (int i = 0; i < produits.Count; i++)
            {
                var row = i + 2;
                worksheet.Cell(row, 1).Value = produits[i].Nom;
                worksheet.Cell(row, 2).Value = produits[i].Prix;
                worksheet.Cell(row, 3).Value = produits[i].Fournisseur;
                worksheet.Cell(row, 4).Value = produits[i].SousCategorie?.Nom;
                worksheet.Cell(row, 5).Value = produits[i].SeuilMin;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Produits.xlsx");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var produit = await _context.Produits.Include(p => p.SousCategorie).FirstOrDefaultAsync(p => p.Id == id);
            if (produit == null) return NotFound();
            return View(produit);
        }

        public IActionResult Create()
        {
            ViewData["SousCategorieId"] = new SelectList(_context.SousCategories, "Id", "Nom");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nom,Prix,Fournisseur,SeuilMin,SousCategorieId")] Produit produit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(produit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SousCategorieId"] = new SelectList(_context.SousCategories, "Id", "Nom", produit.SousCategorieId);
            return View(produit);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var produit = await _context.Produits.FindAsync(id);
            if (produit == null) return NotFound();
            ViewData["SousCategorieId"] = new SelectList(_context.SousCategories, "Id", "Nom", produit.SousCategorieId);
            return View(produit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,Prix,Fournisseur,SeuilMin,SousCategorieId")] Produit produit)
        {
            if (id != produit.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Produits.Any(e => e.Id == produit.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SousCategorieId"] = new SelectList(_context.SousCategories, "Id", "Nom", produit.SousCategorieId);
            return View(produit);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var produit = await _context.Produits.Include(p => p.SousCategorie).FirstOrDefaultAsync(p => p.Id == id);
            if (produit == null) return NotFound();
            return View(produit);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produit = await _context.Produits.FindAsync(id);
            if (produit != null)
            {
                _context.Produits.Remove(produit);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}