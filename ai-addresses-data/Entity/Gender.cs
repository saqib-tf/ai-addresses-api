using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ai_addresses_data.Entity
{
    [Index(nameof(Code), IsUnique = true)]
    [Index(nameof(Name), IsUnique = true)]
    public class Gender
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 1)]
        [Column(TypeName = "varchar(10)")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        // Navigation property (optional)
        [JsonIgnore]
         public virtual ICollection<Person> Persons { get; set; } = new List<Person>();
    }
}