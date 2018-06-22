using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using termoservis.api.Models;

namespace termoservis.api.Data
{
    public interface IQueryHandlerAsync<in TRequest, TResponse>
        where TRequest : IQueryRequest
        where TResponse : IQueryResponse
    {
        Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IQueryHandler<in TRequest, out TResponse>
        where TRequest : IQueryRequest
        where TResponse : IQueryResponse
    {
        TResponse Handle(TRequest request);
    }

    public interface IPlacesRepository
    {
        Task<long> AddAsync(Place place, CancellationToken cancellationToken = default(CancellationToken));
        
        IQueryable<Place> QueryAll();
    }

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

    public interface ICommandRequest
    {
    }

    public interface ICommandResponse
    {
        bool IsSuccess { get; set; }

        Exception Error { get; set; }
    }

    public class CommandResponse : ICommandResponse
    {
        public bool IsSuccess { get; set; }

        public Exception Error { get; set; }
    }

    public interface ICommandHandler<in TRequest, out TResponse>
        where TRequest : ICommandRequest
        where TResponse : ICommandResponse
    {
        TResponse Handle(TRequest request);
    }

    public interface ICommandHandlerAsync<in TRequest, TResponse>
        where TRequest : ICommandRequest
        where TResponse : ICommandResponse
    {
        Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface ICommandDispatcher 
    {
        Task<TResponse> DispatchAsync<TCommandHandler, TRequest, TResponse>(
            TRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
            where TCommandHandler : ICommandHandlerAsync<TRequest, TResponse>
            where TRequest : ICommandRequest
            where TResponse : ICommandResponse;

        TResponse Dispatch<TCommandHandler, TRequest, TResponse>(TRequest request)
            where TCommandHandler : ICommandHandler<TRequest, TResponse>
            where TRequest : ICommandRequest
            where TResponse : ICommandResponse;
    }

    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider serviceProvider;


        public CommandDispatcher(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }


        private TCommandHandler ResolveAsyncHandler<TCommandHandler, TRequest, TResponse>()
            where TCommandHandler : ICommandHandlerAsync<TRequest, TResponse>
            where TRequest : ICommandRequest
            where TResponse : ICommandResponse
        {
            var handler = this.serviceProvider.GetService(typeof(TCommandHandler));
            if (handler == null)
                throw new InvalidOperationException("Couldn't resolve command handler. " + typeof(TCommandHandler).FullName);

            return (TCommandHandler)handler;
        }

        private TCommandHandler ResolveHandler<TCommandHandler, TRequest, TResponse>()
            where TCommandHandler : ICommandHandler<TRequest, TResponse>
            where TRequest : ICommandRequest
            where TResponse : ICommandResponse
        {
            var handler = this.serviceProvider.GetService(typeof(TCommandHandler));
            if (handler == null)
                throw new InvalidOperationException("Couldn't resolve command handler. " + typeof(TCommandHandler).FullName);

            return (TCommandHandler)handler;
        }
        
        public TResponse Dispatch<TCommandHandler, TRequest, TResponse>(TRequest request)
            where TCommandHandler : ICommandHandler<TRequest, TResponse>
            where TRequest : ICommandRequest
            where TResponse : ICommandResponse
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return this.ResolveHandler<TCommandHandler, TRequest, TResponse>().Handle(request);
        }

        public Task<TResponse> DispatchAsync<TCommandHandler, TRequest, TResponse>(
            TRequest request, 
            CancellationToken cancellationToken = default(CancellationToken))
            where TCommandHandler : ICommandHandlerAsync<TRequest, TResponse>
            where TRequest : ICommandRequest
            where TResponse : ICommandResponse
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (cancellationToken == null)
            {
                throw new ArgumentNullException(nameof(cancellationToken));
            }

            return this.ResolveAsyncHandler<TCommandHandler, TRequest, TResponse>().HandleAsync(request, cancellationToken);
        }
    }

    public class CommandCountryCreateRequest : ICommandRequest
    {
        public string Name { get; set; }

        public string ShortName { get; set; }
    }

    public class CommandCountryCreateResponse : CommandResponse
    {
        public long CountryId { get; set; }
    }

    public class CommandCountryCreate : ICommandHandlerAsync<CommandCountryCreateRequest, CommandCountryCreateResponse>
    {
        private readonly ICountriesRepository countriesRepository;

        public CommandCountryCreate(ICountriesRepository countriesRepository)
        {
            this.countriesRepository = countriesRepository ?? throw new ArgumentNullException(nameof(countriesRepository));
        }


        public async Task<CommandCountryCreateResponse> HandleAsync(
            CommandCountryCreateRequest request, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (cancellationToken == null) throw new ArgumentNullException(nameof(cancellationToken));

            var response = new CommandCountryCreateResponse();
            try 
            {
                var countryId = await this.countriesRepository
                    .AddAsync(new Country {
                        Name = request.Name.Trim(),
                        ShortName = request.ShortName.Trim()
                    });

                response.IsSuccess = true;
                response.CountryId = countryId;
            }
            catch(Exception ex)
            {
                response.Error = ex;
            }
            return response;
        }
    }

    public class CommandPlaceCreateRequest : ICommandRequest
    {
        public string Name { get; set; }

        public long CountryId { get; set; }
    }

    public class CommandPlaceCreateResponse : CommandResponse
    {
        public long PlaceId { get; set; }
    }

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

    public interface ICountriesRepository 
    {
        Task<long> AddAsync(Country country, CancellationToken cancellationToken = default(CancellationToken));

        IQueryable<Country> QueryAll();
    }

    public class CountriesRepository : ICountriesRepository
    {
        private readonly ITermoservisContext context;

        public CountriesRepository(ITermoservisContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<long> AddAsync(Country country, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (country == null) throw new ArgumentNullException(nameof(country));
            if (cancellationToken == null) throw new ArgumentNullException(nameof(cancellationToken));

            this.context.Countries.Add(country);
            await this.context.SaveChangesAsync(cancellationToken);

            return country.Id;   
        }

        public IQueryable<Country> QueryAll()
        {
            return this.context.Countries;
        }
    }

    public interface IQueryRequest
    {
    }

    public interface IQueryResponse
    {
    }

    public class QueryPlacesFilteredRequest : IQueryRequest
    {
        public string Keywords { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }

    public class QueryPlacesFilteredResponse : IQueryResponse
    {
        public QueryPlacesFilteredResponse(IEnumerable<Place> places)
        {
            this.Places = places ?? throw new ArgumentNullException(nameof(places));
        }


        public IEnumerable<Place> Places { get; }
    }

    public interface IQueryDispatcher
    {
        Task<TResponse> DispatchAsync<TQueryHandler, TRequest, TResponse>(
            TRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
            where TQueryHandler : IQueryHandlerAsync<TRequest, TResponse>
            where TRequest : IQueryRequest
            where TResponse : IQueryResponse;

        TResponse Dispatch<TQueryHandler, TRequest, TResponse>(TRequest request)
            where TQueryHandler : IQueryHandler<TRequest, TResponse>
            where TRequest : IQueryRequest
            where TResponse : IQueryResponse;
    }

    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider serviceProvider;


        public QueryDispatcher(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }


        private TQueryHandler ResolveAsyncHandler<TQueryHandler, TRequest, TResponse>()
            where TQueryHandler : IQueryHandlerAsync<TRequest, TResponse>
            where TRequest : IQueryRequest
            where TResponse : IQueryResponse
        {
            var handler = this.serviceProvider.GetService(typeof(TQueryHandler));
            if (handler == null)
                throw new InvalidOperationException("Couldn't resolve query handler. " + typeof(TQueryHandler).FullName);

            return (TQueryHandler)handler;
        }

        private TQueryHandler ResolveHandler<TQueryHandler, TRequest, TResponse>()
            where TQueryHandler : IQueryHandler<TRequest, TResponse>
            where TRequest : IQueryRequest
            where TResponse : IQueryResponse
        {
            var handler = this.serviceProvider.GetService(typeof(TQueryHandler));
            if (handler == null)
                throw new InvalidOperationException("Couldn't resolve query handler. " + typeof(TQueryHandler).FullName);

            return (TQueryHandler)handler;
        }

        public TResponse Dispatch<TQueryHandler, TRequest, TResponse>(TRequest request)
            where TQueryHandler : IQueryHandler<TRequest, TResponse>
            where TRequest : IQueryRequest
            where TResponse : IQueryResponse
        {
            if (request == null) 
            {
                throw new ArgumentNullException(nameof(request));
            }

            return this.ResolveHandler<TQueryHandler, TRequest, TResponse>()
                       .Handle(request);
        }

        public Task<TResponse> DispatchAsync<TQueryHandler, TRequest, TResponse>(
            TRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
            where TQueryHandler : IQueryHandlerAsync<TRequest, TResponse>
            where TRequest : IQueryRequest
            where TResponse : IQueryResponse
        {
            if (request == null) 
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (cancellationToken == null) 
            {
                throw new ArgumentNullException(nameof(cancellationToken));
            }

            return this.ResolveAsyncHandler<TQueryHandler, TRequest, TResponse>()
                       .HandleAsync(request, cancellationToken);
        }
    }

    public class QueryPlacesFiltered : IQueryHandlerAsync<QueryPlacesFilteredRequest, QueryPlacesFilteredResponse>
    {
        private readonly IPlacesRepository placesRepository;


        public QueryPlacesFiltered(IPlacesRepository placesRepository)
        {
            this.placesRepository = placesRepository ?? throw new System.ArgumentNullException(nameof(placesRepository));
        }


        public async Task<QueryPlacesFilteredResponse> HandleAsync(QueryPlacesFilteredRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
            {
                throw new System.ArgumentNullException(nameof(request));
            }
            if (cancellationToken == null)
            {
                throw new ArgumentNullException(nameof(cancellationToken));
            }

            var keywords = request.Keywords
                .ToLowerInvariant()
                .Split(new[] { " ", ".", "-" }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            var query = this.placesRepository.QueryAll();
            if (keywords.Count > 0)
                query = query.Where(place => keywords.Contains(place.Name.ToLower()));
            query = query.OrderBy(place => place.Name);

            var placesList = await query
                .Skip(request.Skip)
                .Take(request.Take)
                .ToListAsync(cancellationToken);

            return new QueryPlacesFilteredResponse(placesList);
        }
    }

    public class PlaceDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long CountryId { get; set; }

        public string CountryName { get; set; }

        public string CountryShortName { get; set; }
    }

    public class PlaceProfile : Profile
    {
        public PlaceProfile()
        {
            this.CreateMap<Place, PlaceDto>();
        }
    }

    public class CountryDto 
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }
    }

    public class CountryProfile : Profile 
    {
        public CountryProfile() 
        {
            this.CreateMap<Country, CountryDto>();
        }
    }

    public interface IPlacesApiService
    {
        Task<long> CreatePlaceAsync(
            string name,
            long countryId
        );

        Task<IEnumerable<PlaceDto>> QueryPlacesByKeywordsAsync(
            string keywords,
            int skip,
            int take
        );
    }

    public interface ICountriesApiService 
    {
        Task<long> CreateCountryAsync(
            string name,
            string shortName
        );
    }

    public class CountriesApiService : ICountriesApiService
    {
        private readonly IQueryDispatcher queryDispatcher;
        private readonly ICommandDispatcher commandDispatcher;

        public CountriesApiService(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            this.queryDispatcher = queryDispatcher ?? throw new ArgumentNullException(nameof(queryDispatcher));
            this.commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));
        }


        public async Task<long> CreateCountryAsync(string name, string shortName)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));
            if (string.IsNullOrWhiteSpace(shortName)) throw new ArgumentException(nameof(shortName));

            var response = await this.commandDispatcher.DispatchAsync<CommandCountryCreate, CommandCountryCreateRequest, CommandCountryCreateResponse>(
                new CommandCountryCreateRequest {
                    Name = name,
                    ShortName = shortName
                }
            );
            if (!response.IsSuccess)
                throw response.Error;

            return response.CountryId;
        }
    }

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

    public interface ITermoservisContext
    {
        DbSet<Place> Places { get; set; }

        DbSet<Country> Countries { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }

    public class TermoservisContext : IdentityDbContext<ApplicationUser>, ITermoservisContext
    {
        public TermoservisContext(DbContextOptions options) : base(options)
        {
        }

        protected TermoservisContext()
        {
        }


        public DbSet<Country> Countries { get; set; }

        public DbSet<Place> Places { get; set; }
    }
}