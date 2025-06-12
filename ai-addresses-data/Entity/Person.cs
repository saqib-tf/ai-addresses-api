using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ai_addresses_data.Entity
{
    public class Person
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }

        // Foreign key property
        public int? GenderId { get; set; }

        // Navigation property for Gender
        [ForeignKey(nameof(GenderId))]
        public Gender? Gender { get; set; }

        [StringLength(500)]
        public string? ProfilePictureUrl { get; set; }
    }
}