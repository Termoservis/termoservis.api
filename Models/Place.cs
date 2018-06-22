using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace termoservis.api.Models
{
    public class Place : IDbModel, IEntityTypeConfiguration<Place>
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        public long CountryId { get; set; }

        [ForeignKey(nameof(CountryId))]
        public Country Country { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public void Configure(EntityTypeBuilder<Place> builder)
        {
            builder.HasIndex(place => place.Name);
        }
    }
}