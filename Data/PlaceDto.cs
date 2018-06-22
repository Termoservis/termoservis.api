namespace termoservis.api.Data
{
    public class PlaceDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long CountryId { get; set; }

        public string CountryName { get; set; }

        public string CountryShortName { get; set; }
    }
}