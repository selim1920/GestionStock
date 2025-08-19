using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GestionStock.Models
{
    public class Produit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nom { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Prix { get; set; }

        public string? Fournisseur { get; set; }

        [Required]
        public int SeuilMin { get; set; }

        // Relation avec Sous-Catégorie
        [Required]
        [ForeignKey("SousCategorie")]
        public int SousCategorieId { get; set; }
        public SousCategorie? SousCategorie { get; set; }

        // Relation avec Stock
        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();

        // Relation avec MouvementStock
        public ICollection<MouvementStock> MouvementsStock { get; set; } = new List<MouvementStock>();
    }
}

