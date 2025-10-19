using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Task.AirAstana.Application.Features.Flights.Models.DTOs;
using Task.AirAstana.Domain.Repositories;
using Task.AirAstana.Domain.Services;

namespace Task.AirAstana.Application.Features.Flights.Queries;

  public class GetFlightsQueryHandler : IRequestHandler<GetFlightsQuery, List<FlightDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetFlightsQueryHandler> _logger;
        private const string CacheKey = "flights:all";

        public GetFlightsQueryHandler( IUnitOfWork unitOfWork, ICacheService cacheService, IMapper mapper, ILogger<GetFlightsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<FlightDto>> Handle(GetFlightsQuery request, CancellationToken cancellationToken)
        {
            // ВАЖНО: Читаем ТОЛЬКО из кэша согласно ТЗ
            var cachedFlights = await _cacheService.GetAsync<List<FlightDto>>(CacheKey, cancellationToken);
            List<FlightDto> flights;

            if (cachedFlights == null || !cachedFlights.Any())
            {
                _logger.LogInformation("Кэш пуст. Загрузка рейсов из БД");

                // Если кэш пуст - загрузить из БД
                var flightsFromDb = await _unitOfWork.Flights.GetAllAsync(cancellationToken);

                // Маппинг в DTO
                flights = _mapper.Map<List<FlightDto>>(flightsFromDb);

                // Сортировка по Arrival (обязательно по ТЗ)
                flights = flights.OrderBy(f => f.Arrival).ToList();

                // Положить в кэш на 1 час
                await _cacheService.SetAsync(CacheKey, flights, TimeSpan.FromHours(1), cancellationToken);

                _logger.LogInformation("Рейсы загружены из БД и закэшированы. Количество: {Count}", flights.Count);
            }
            else
            {
                flights = cachedFlights;
                _logger.LogInformation("Рейсы загружены из кэша. Количество: {Count}", flights.Count);
            }

            // Фильтрация по Origin и/или Destination
            if (!string.IsNullOrWhiteSpace(request.Origin))
            {
                flights = flights.Where(f => f.Origin.Contains(request.Origin, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(request.Destination))
            {
                flights = flights.Where(f => f.Destination.Contains(request.Destination, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            _logger.LogInformation("Возвращено рейсов после фильтрации: {Count}", flights.Count);

            return flights;
        }
    }