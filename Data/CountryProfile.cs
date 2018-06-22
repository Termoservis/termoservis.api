using AutoMapper;
using termoservis.api.Models;

namespace termoservis.api.Data
{
    public class CountryProfile : Profile 
    {
        public CountryProfile() 
        {
            this.CreateMap<Country, CountryDto>();
        }
    }
}