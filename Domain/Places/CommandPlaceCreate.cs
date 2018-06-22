using System;
using System.Threading;
using System.Threading.Tasks;
using termoservis.api.Data;
using termoservis.api.Domain.Abstract;
using termoservis.api.Models;

namespace termoservis.api.Domain.Places
{
    public class CommandPlaceCreate : ICommandHandlerAsync<CommandPlaceCreateRequest, CommandPlaceCreateResponse>
    {
        private readonly IPlacesRepository placesRepository;


        public CommandPlaceCreate(
            IQueryDispatcher queryDispatcher,
            IPlacesRepository placesRepository)
        {
            this.placesRepository = placesRepository ?? throw new ArgumentNullException(nameof(placesRepository));
        }


        public async Task<CommandPlaceCreateResponse> HandleAsync(CommandPlaceCreateRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (cancellationToken == null)
            {
                throw new ArgumentNullException(nameof(cancellationToken));
            }

            var response = new CommandPlaceCreateResponse();
            try 
            {
                var placeId = await this.placesRepository.AddAsync(new Place {
                    Name = request.Name.Trim(),
                    CountryId = request.CountryId
                }, cancellationToken);

                response.IsSuccess = true;
                response.PlaceId = placeId;
            }
            catch (Exception ex) 
            {
                response.Error = ex;
            }
            return response;
        }
    }
}