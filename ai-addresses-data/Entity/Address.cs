using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ai_addresses_data.Entity
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string City { get; set; } = string.Empty;

        // Foreign key for State
        public int? StateId { get; set; }

        [ForeignKey(nameof(StateId))]
        public State? State { get; set; }

        // Foreign key for Country
        public int? CountryId { get; set; }

        [ForeignKey(nameof(CountryId))]
        public Country? Country { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        // Foreign key for AddressType
        public int? AddressTypeId { get; set; }

        [ForeignKey(nameof(AddressTypeId))]
        public AddressType? AddressType { get; set; }

        // Foreign key for Person
        public int PersonId { get; set; }

        [ForeignKey(nameof(PersonId))]
        public Person Person { get; set; } = null!;
    }
}
