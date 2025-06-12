using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ai_addresses_data.Entity
{
    [Index(nameof(Name), IsUnique = true)]
    public class AddressType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        // Navigation property (optional)
        [JsonIgnore]
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}