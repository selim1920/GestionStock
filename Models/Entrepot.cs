using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace GestionStock.Models
{
    public class Entrepot
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de l'entrepôt est obligatoire.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Le nom doit contenir entre 3 et 200 caractères.")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "L'adresse est obligatoire.")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "L'adresse doit contenir entre 5 et 500 caractères.")]
        public string Adresse { get; set; }

        [Required(ErrorMessage = "La capacité est obligatoire.")]
        [Range(1, 10000, ErrorMessage = "La capacité doit être comprise entre 1 et 10 000.")]
        public int Capacite { get; set; }

        // Champ persistant dans la base
        public int NombreRayons { get; set; }

        // Champ mis à jour automatiquement
        public int CapaciteDisponible { get; set; }

        // Relations
        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
        public ICollection<MouvementStock> MouvementsStock { get; set; } = new List<MouvementStock>();

        // Méthode pour calculer NombreRayons
        public void CalculerNombreRayons()
        {
            int nbRayons = Capacite / 50;

            if (nbRayons == 0) nbRayons = 1;
            NombreRayons = (nbRayons > 8) ? 8 : nbRayons;
        }

        // Méthode pour initialiser la capacité disponible
        public void InitialiserCapaciteDisponible()
        {
            CapaciteDisponible = Capacite;
        }
    }
}
