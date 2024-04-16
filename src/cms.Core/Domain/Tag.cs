using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace cms.Core.Domain
{
    [Table("Tags")]
    public class Tag
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Slug { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }
    }
}
