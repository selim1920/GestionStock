using GestionStock.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionStock.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
             : base(options)
        {
        }

        // Définition des tables de la base de données via DbSet
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<SousCategorie> SousCategories { get; set; }
        public DbSet<Produit> Produits { get; set; }
        public DbSet<MouvementStock> MouvementsStock { get; set; }
        public DbSet<Entrepot> Entrepots { get; set; }
        public DbSet<Categorie> Categories { get; set; }

        // Configuration des relations entre les entités dans la base de données via Fluent API

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration de la relation entre Produit et Stock
            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Produit)
                .WithMany(p => p.Stocks)
                .HasForeignKey(s => s.ProduitId)
                .OnDelete(DeleteBehavior.Restrict); // Suppression restreinte


            

            // Configuration de la relation entre Entrepot et MouvementStock
            modelBuilder.Entity<MouvementStock>()
                .HasOne(m => m.Entrepot)
                .WithMany(e => e.MouvementsStock)
                .HasForeignKey(m => m.EntrepotId)
                .OnDelete(DeleteBehavior.Restrict); // Suppression restreinte

            // Configuration de la relation entre Produit et MouvementStock
            modelBuilder.Entity<MouvementStock>()
                .HasOne(m => m.Produit)
                .WithMany(p => p.MouvementsStock)
                .HasForeignKey(m => m.ProduitId)
                .OnDelete(DeleteBehavior.Restrict); // Suppression restreinte

            // Configuration de la relation entre SousCategorie et Produit
            modelBuilder.Entity<Produit>()
                .HasOne(p => p.SousCategorie)
                .WithMany(s => s.Produits)
                .HasForeignKey(p => p.SousCategorieId)
                .OnDelete(DeleteBehavior.Restrict); // Pas de suppression en cascade sur les sous-catégories

            // Configuration de la relation entre Categorie et SousCategorie
            modelBuilder.Entity<SousCategorie>()
                .HasOne(s => s.Categorie)
                .WithMany(c => c.SousCategories)
                .HasForeignKey(s => s.CategorieId)
                .OnDelete(DeleteBehavior.Restrict); // Suppression restreinte

          
        }
    }
}
