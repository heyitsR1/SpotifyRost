using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpotifyRoast.Models
{
    public class Menu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ParentId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string Url { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(100)]
        public string Icon { get; set; }

        public virtual ICollection<MenuRole> MenuRoles { get; set; }
    }
}
