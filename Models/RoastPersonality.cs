using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpotifyRoast.Models
{
    public class RoastPersonality
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string SystemPrompt { get; set; }

        [StringLength(50)]
        public string Icon { get; set; } // e.g. "fa-robot", "fa-fire"

        [DefaultValue(false)]
        public bool IsDeleted { get; set; } = false;
    }
}
