using System.ComponentModel.DataAnnotations;

namespace CMC_Backend.Models
{
    public class Bett
    {
        [Key]
        public string BettNummer { get; set; } = string.Empty;

        public int ZimmerNummer { get; set; }

        public string Zimmer { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Frei|Belegt)$")]
        public string Status { get; set; } = "Frei";

        [Required]
        [RegularExpression("^(Sauber|Nicht sauber)$")]
        public string Wartung { get; set; } = "Sauber";

        public DateTime LetztGeaendert { get; set; } = DateTime.UtcNow;
    }
}
