using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GestionStock.Models
{
    public class SousCategorie
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de la sous-catégorie est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
        public string Nom { get; set; }

        // Clé étrangère vers Categorie
        [Required(ErrorMessage = "Veuillez sélectionner une catégorie.")]
        [ForeignKey("Categorie")]
        public int CategorieId { get; set; }

        public virtual Categorie? Categorie { get; set; }

        // Relation avec les Produits
        public virtual ICollection<Produit> Produits { get; set; } = new List<Produit>();
    }
}

