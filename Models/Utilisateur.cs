using System.ComponentModel.DataAnnotations;

namespace GestionStock.Models
{
    public enum Role
    {
        Admin,
        Membre
    }

    public class Utilisateur
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 100 caractères.")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "L'email n'est pas valide.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères.")]
        [DataType(DataType.Password)]
        public string MotDePasse { get; set; }

        [Required(ErrorMessage = "Le rôle est obligatoire.")]
        public Role Role { get; set; }
    }
}
