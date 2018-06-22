using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using termoservis.api.Models;

namespace termoservis.api.Data
{
    public class PlacesRepository : IPlacesRepository
    {
        private readonly ITermoservisContext context;


        public PlacesRepository(ITermoservisContext context)
        {
            this.context = context ?? throw new System.ArgumentNullException(nameof(context));
        }


        public async Task<long> AddAsync(Place place, CancellationToken cancellationToken = default(CancellationToken)) 
        {
            if (place == null)
            {
                throw new ArgumentNullException(nameof(place));
            }

            this.context.Places.Add(place);
            await this.context.SaveChangesAsync(cancellationToken);

            return place.Id;
        }

        public IQueryable<Place> QueryAll()
        {
            return this.context.Places.Include(place => place.Country);
        }
    }
}