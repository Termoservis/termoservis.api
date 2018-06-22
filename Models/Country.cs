using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace termoservis.api.Models
{
    public class Country : IDbModel, IEntityTypeConfiguration<Country>
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string ShortName { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }

        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasIndex(country => country.Name);
            builder.HasIndex(country => country.ShortName);
        }
    }
}