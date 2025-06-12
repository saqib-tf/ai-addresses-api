using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ai_addresses_data.Entity
{
    [Index(nameof(Code), IsUnique = true)]
    [Index(nameof(Name), IsUnique = true)]
    public class Country
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 2)]
        [Column(TypeName = "varchar(10)")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        // Navigation property for related states
        [JsonIgnore]
        public virtual ICollection<State> States { get; set; } = new List<State>();
    }
}