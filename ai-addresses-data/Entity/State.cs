using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ai_addresses_data.Entity
{
    [Index(nameof(CountryId), nameof(Code), IsUnique = true)]
    [Index(nameof(CountryId), nameof(Name), IsUnique = true)]
    public class State
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        [Column(TypeName = "varchar(10)")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // Foreign key for Country
        public int CountryId { get; set; }

        [ForeignKey(nameof(CountryId))]
        public Country Country { get; set; } = null!;
    }
}