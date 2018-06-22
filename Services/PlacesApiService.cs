using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using termoservis.api.Data;
using termoservis.api.Domain.Abstract;
using termoservis.api.Domain.Places;

namespace termoservis.api.Services
{
    public class PlacesApiService : IPlacesApiService
    {
        private readonly IQueryDispatcher queryDispatcher;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IMapper mapper;


        public PlacesApiService(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, IMapper mapper)
        {
            this.queryDispatcher = queryDispatcher ?? throw new ArgumentNullException(nameof(queryDispatcher));
            this.commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<long> CreatePlaceAsync(string name, long countryId)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));
            if (countryId <= 0) throw new ArgumentOutOfRangeException(nameof(countryId));

            var response = await this.commandDispatcher.DispatchAsync<CommandPlaceCreate, CommandPlaceCreateRequest, CommandPlaceCreateResponse>(
                new CommandPlaceCreateRequest {
                    Name = name,
                    CountryId = countryId
                }
            );
            if (!response.IsSuccess)
                throw response.Error;

            return response.PlaceId;
        }

        public async Task<IEnumerable<PlaceDto>> QueryPlacesByKeywordsAsync(string keywords, int skip, int take)
        {
            if (keywords == null)
            {
                throw new ArgumentNullException(nameof(keywords));
            }

            var response = await this.queryDispatcher.DispatchAsync<QueryPlacesFiltered, QueryPlacesFilteredRequest, QueryPlacesFilteredResponse>(
                new QueryPlacesFilteredRequest()
                {
                    Keywords = keywords.Trim(),
                    Skip = Math.Max(0, skip),
                    Take = Math.Max(0, take)
                });
            if (response.Places == null)
                return Enumerable.Empty<PlaceDto>();
            return this.mapper.Map<IEnumerable<PlaceDto>>(response.Places);
        }
    }
}