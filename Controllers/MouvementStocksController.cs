using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionStock.Data;
using GestionStock.Models;
using ClosedXML.Excel;
using System.IO;
using System.Collections.Generic;

namespace GestionStock.Controllers
{
    public class MouvementStocksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 5;

        public MouvementStocksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MouvementStocks
        public async Task<IActionResult> Index(string searchProduit, string searchEntrepot, TypeMouvement? typeMouvement, int page = 1)
        {
            var mouvements = _context.MouvementsStock
                .Include(m => m.Produit)
                .Include(m => m.Entrepot)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchProduit))
                mouvements = mouvements.Where(m => m.Produit.Nom.Contains(searchProduit));

            if (!string.IsNullOrEmpty(searchEntrepot))
                mouvements = mouvements.Where(m => m.Entrepot.Adresse.Contains(searchEntrepot));

            if (typeMouvement != null)
                mouvements = mouvements.Where(m => m.TypeMouvement == typeMouvement);

            var totalCount = await mouvements.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            var mouvementsPage = await mouvements
                .OrderByDescending(m => m.Date)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchProduit = searchProduit;
            ViewBag.SearchEntrepot = searchEntrepot;
            ViewBag.TypeMouvement = typeMouvement;

            return View(mouvementsPage);
        }

        // EXPORT EXCEL
        public async Task<IActionResult> ExportToExcel()
        {
            var mouvements = await _context.MouvementsStock
                .Include(m => m.Produit)
                .Include(m => m.Entrepot)
                .OrderByDescending(m => m.Date)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Mouvements");

            worksheet.Cell(1, 1).Value = "Produit";
            worksheet.Cell(1, 2).Value = "Entrepôt";
            worksheet.Cell(1, 3).Value = "Type";
            worksheet.Cell(1, 4).Value = "Quantité";
            worksheet.Cell(1, 5).Value = "Date";

            int row = 2;
            foreach (var item in mouvements)
            {
                worksheet.Cell(row, 1).Value = item.Produit.Nom;
                worksheet.Cell(row, 2).Value = item.Entrepot.Adresse;
                worksheet.Cell(row, 3).Value = item.TypeMouvement.ToString();
                worksheet.Cell(row, 4).Value = item.Quantite;
                worksheet.Cell(row, 5).Value = item.Date.ToString("yyyy-MM-dd");
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "MouvementsStock.xlsx");
        }

        // GET: MouvementStocks/Create
        public IActionResult Create()
        {
            ViewBag.EntrepotId = new SelectList(_context.Entrepots, "Id", "Nom");
            ViewBag.ProduitId = new SelectList(_context.Produits, "Id", "Nom");
            return View();
        }

        // POST: MouvementStocks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EntrepotId,ProduitId,TypeMouvement,Quantite,Date")] MouvementStock mouvementStock)
        {
            if (ModelState.IsValid)
            {
                var stock = await _context.Stocks
                    .FirstOrDefaultAsync(s => s.ProduitId == mouvementStock.ProduitId && s.EntrepotId == mouvementStock.EntrepotId);

                if (mouvementStock.TypeMouvement == TypeMouvement.Entree)
                {
                    if (stock == null)
                    {
                        stock = new Stock
                        {
                            ProduitId = mouvementStock.ProduitId,
                            EntrepotId = mouvementStock.EntrepotId,
                            Quantite = mouvementStock.Quantite
                        };
                        _context.Stocks.Add(stock);
                    }
                    else
                    {
                        stock.Quantite += mouvementStock.Quantite;
                        _context.Stocks.Update(stock);
                    }
                }
                else if (mouvementStock.TypeMouvement == TypeMouvement.Sortie)
                {
                    if (stock == null || stock.Quantite < mouvementStock.Quantite)
                    {
                        ModelState.AddModelError("", "Stock insuffisant pour effectuer la sortie.");
                        ViewBag.EntrepotId = new SelectList(_context.Entrepots, "Id", "Nom", mouvementStock.EntrepotId);
                        ViewBag.ProduitId = new SelectList(_context.Produits, "Id", "Nom", mouvementStock.ProduitId);
                        return View(mouvementStock);
                    }

                    stock.Quantite -= mouvementStock.Quantite;
                    _context.Stocks.Update(stock);
                }

                _context.MouvementsStock.Add(mouvementStock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.EntrepotId = new SelectList(_context.Entrepots, "Id", "Nom", mouvementStock.EntrepotId);
            ViewBag.ProduitId = new SelectList(_context.Produits, "Id", "Nom", mouvementStock.ProduitId);
            return View(mouvementStock);
        }

        // GET: MouvementStocks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var mouvementStock = await _context.MouvementsStock.FindAsync(id);
            if (mouvementStock == null) return NotFound();

            ViewBag.EntrepotId = new SelectList(_context.Entrepots, "Id", "Nom", mouvementStock.EntrepotId);
            ViewBag.ProduitId = new SelectList(_context.Produits, "Id", "Nom", mouvementStock.ProduitId);
            return View(mouvementStock);
        }

        // POST: MouvementStocks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EntrepotId,ProduitId,TypeMouvement,Quantite,Date")] MouvementStock mouvementStock)
        {
            if (id != mouvementStock.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mouvementStock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.MouvementsStock.Any(e => e.Id == mouvementStock.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.EntrepotId = new SelectList(_context.Entrepots, "Id", "Nom", mouvementStock.EntrepotId);
            ViewBag.ProduitId = new SelectList(_context.Produits, "Id", "Nom", mouvementStock.ProduitId);
            return View(mouvementStock);
        }

        // GET: MouvementStocks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var mouvementStock = await _context.MouvementsStock
                .Include(m => m.Produit)
                .Include(m => m.Entrepot)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mouvementStock == null) return NotFound();

            return View(mouvementStock);
        }

        // GET: MouvementStocks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var mouvementStock = await _context.MouvementsStock
                .Include(m => m.Entrepot)
                .Include(m => m.Produit)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mouvementStock == null) return NotFound();

            return View(mouvementStock);
        }

        // POST: MouvementStocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mouvementStock = await _context.MouvementsStock.FindAsync(id);
            _context.MouvementsStock.Remove(mouvementStock);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // AJAX: Get produits disponibles dans un entrepôt
        public JsonResult GetProduitsDispo(int entrepotId)
        {
            var produits = _context.Stocks
                .Include(s => s.Produit)
                .Where(s => s.EntrepotId == entrepotId && s.Quantite > 0)
                .Select(s => new
                {
                    id = s.Produit.Id,
                    nom = s.Produit.Nom
                }).ToList();

            return Json(produits);
        }
    }
}