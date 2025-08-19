using System.ComponentModel.DataAnnotations;

namespace GestionStock.Models
{
    public class Categorie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; }

        // Relation : Une catégorie a plusieurs sous-catégories
        public ICollection<SousCategorie> SousCategories { get; set; } = new List<SousCategorie>();
    }
}

