using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cms.Core.Domain
{
    [Table("Posts")]
    [Index(nameof(Slug), IsUnique = true)]
    public class Post
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(250)]
        public required string Name { get; set; }

        [Required]
        [Column(TypeName = "varchar(250)")]
        public required string Slug { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public Guid CategoryId { get; set; }

        [MaxLength(500)]
        public string? Thumbnail { get; set; }

        public string? Content { get; set; }
        public Guid AuthorUserId { get; set; }

        [MaxLength(128)]
        public string? Source { get; set; }

        [MaxLength(250)]
        public string? Tags { get; set; }

        [MaxLength(250)]
        public string? SeoDescription { get; set; }

        [MaxLength(250)]
        public string? SeoKeyword { get; set; }

        public int ViewCount { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public bool IsPaid { get; set; }
        public double RoyaltyAmount { get; set; }
        public PostStatus Status { get; set; }
    }

    public enum PostStatus
    {
        Draft = 0,
        WaitingForApproval = 1,
        Rejected = 2,
        Published = 3
    }
}
