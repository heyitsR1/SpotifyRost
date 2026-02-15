using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpotifyRoast.Models
{
    public class Roast
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int PersonalityId { get; set; }

        [Required]
        public string SpotifyLink { get; set; }

        [Required]
        public string RoastContent { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("PersonalityId")]
        public virtual RoastPersonality Personality { get; set; }
    }
}
