using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionStock.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le produit est requis.")]
        [Display(Name = "Produit")]
        [ForeignKey("Produit")]
        public int ProduitId { get; set; }

        public Produit? Produit { get; set; }

        [Required(ErrorMessage = "L'entrepôt est requis.")]
        [Display(Name = "Entrepôt")]
        [ForeignKey("Entrepot")]
        public int EntrepotId { get; set; }

        public Entrepot? Entrepot { get; set; }

        [Required(ErrorMessage = "Le seuil d'alerte est requis.")]
        [Display(Name = "Seuil d'alerte")]
        [Range(1, int.MaxValue, ErrorMessage = "Le seuil d'alerte doit être supérieur à 0.")]
        public int SeuilAlerte { get; set; }

        [Required(ErrorMessage = "Le rayon est requis.")]
        [Range(1, 100, ErrorMessage = "Le rayon doit être entre 1 et 8.")]
        public int Rayon { get; set; }

        [Required(ErrorMessage = "La quantité est requise.")]
        [Display(Name = "Quantité en stock")]
        [Range(0, int.MaxValue, ErrorMessage = "La quantité doit être un nombre positif.")]
        public int Quantite { get; set; }
    }
}
