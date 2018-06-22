using AutoMapper;
using termoservis.api.Models;

namespace termoservis.api.Data
{
    public class PlaceProfile : Profile
    {
        public PlaceProfile()
        {
            this.CreateMap<Place, PlaceDto>();
        }
    }
}