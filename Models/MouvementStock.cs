using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GestionStock.Models
{
    public enum TypeMouvement
    {
        Entree,
        Sortie
    }

    public class MouvementStock
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

        [Required(ErrorMessage = "Le type de mouvement est requis.")]
        [Display(Name = "Type de mouvement")]
        public TypeMouvement TypeMouvement { get; set; }

        [Required(ErrorMessage = "La quantité est requise.")]
        [Range(1, int.MaxValue, ErrorMessage = "La quantité doit être supérieure à 0.")]
        [Display(Name = "Quantité")]
        public int Quantite { get; set; }

        [Required(ErrorMessage = "La date est requise.")]
        [Display(Name = "Date du mouvement")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
