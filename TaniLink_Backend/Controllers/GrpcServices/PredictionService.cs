using AutoMapper;
using CsvHelper;
using Grpc.Core;
using RestSharp;
using System.Globalization;
using TaniLink_Backend.Interfaces;
using TaniLink_Backend.Models;

namespace TaniLink_Backend.Controllers.GrpcServices
{
    public class PredictionService : Predictions.PredictionsBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PredictionService> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IPredictionRepository _predictionRepository;
        private readonly ICommodityRepository _commodityRepository;
        private readonly IAreaRepository _areaRepository;

        public PredictionService(IMapper mapper,
            ILogger<PredictionService> logger,
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration,
            IPredictionRepository predictionRepository,
            ICommodityRepository commodityRepository,
            IAreaRepository areaRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _predictionRepository = predictionRepository;
            _commodityRepository = commodityRepository;
            _areaRepository = areaRepository;
        }

        public override async Task<AllPredictionDetail> GetPredictions(PredictionReq request, ServerCallContext context)
        {
            try
            {

                var commodity = await _commodityRepository.GetCommodityById(request.CommodityId);
                if (commodity == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Commodity not found"));

                var area = await _areaRepository.GetAreaById(request.AreaId);
                if (area == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Area not found"));

                var areas = await _areaRepository.GetAllAreas();
                if (!areas.Any())
                    throw new RpcException(new Status(StatusCode.NotFound, "Areas not found"));

                var latestPrediction = await _predictionRepository.GetLatestPredictionByCommodityIdAndAreaId(request.CommodityId, request.AreaId);
                if (latestPrediction == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Latest prediction not found"));

                var requestDate = DateOnly.Parse(request.Date);
                if (requestDate.DayNumber+35 > latestPrediction.Date.DayNumber)
                {
                    int num = requestDate.DayNumber+35 - latestPrediction.Date.DayNumber;

                    var baseUri = _configuration["ApiUrls:Prediction"];
                    RestClient client = new RestClient(new RestClientOptions
                    {
                        BaseUrl = new Uri(baseUri),
                    });

                    var apiReq = new RestRequest($"predictions/{request.CommodityId}/{num+31}");
                    var result = await client.GetAsync<Dictionary<string, string>>(apiReq);

                    foreach (var item in result)
                    {
                        var prediction = await _predictionRepository.GetByPredictionDate(DateOnly.ParseExact(item.Key, "yyyy-MM-dd"));
                        if (prediction != null)
                        {
                            prediction.Price = item.Value;
                            prediction.Commodity = commodity;

                            var update = await _predictionRepository.UpdatePrediction(prediction);
                            if (update == null)
                                throw new RpcException(new Status(StatusCode.Internal, "Failed to update prediction"));
                        }
                        else
                        {
                            var predictionCreate = new Prediction
                            {
                                Date = DateOnly.ParseExact(item.Key, "yyyy-MM-dd"),
                                Price = item.Value,
                                Commodity = commodity,
                                Areas = areas.ToList()
                            };

                            var create = await _predictionRepository.CreatePrediction(predictionCreate);
                            if (create == null)
                                throw new RpcException(new Status(StatusCode.Internal, "Failed to create prediction"));
                        }
                    }
                }

                var predictions = await _predictionRepository.GetAllPredictionByCommodityIdAndAreaIdAndMonthAndYear(request.CommodityId, request.AreaId, requestDate);
                if (!predictions.Any())
                    throw new RpcException(new Status(StatusCode.NotFound, "Predictions not found"));

                var predictionResponse = new AllPredictionDetail();
                var predictionDetails = _mapper.Map<IEnumerable<PredictionDetail>>(predictions);
                predictionResponse.Predictions.AddRange(predictionDetails);
                return predictionResponse;
            }
            catch (RpcException ex)
            {
                throw new RpcException(ex.Status);
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }

        }
    }

    public class DatasetCsv
    {
        public string Date { get; set; }
        public string Price { get; set; }
    }
}











/*var commodityDataset = commodity.PredictionDatasets
                    .FirstOrDefault(pd => pd.IsUsed == true);

                var areas = await _areaRepository.GetAllAreas();
                if (!areas.Any())
                    throw new RpcException(new Status(StatusCode.NotFound, "Areas not found"));

                var webRootPath = _webHostEnvironment.WebRootPath;
                var filePatch = Path.Combine(webRootPath, "predictions", "datasets", commodityDataset.DatasetLink.Split("/")[2]);
                _logger.LogInformation(filePatch);

                using (var reader = new StreamReader(filePatch))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    // Baca semua baris dan map ke kelas POCO (Plain Old C# Object)
                    var records = csv.GetRecords<DatasetCsv>().ToList();

                    // Contoh LINQ Query
                    var filteredRecords = records
                        .Where(record => record.Column1 == "Nilai tertentu")
                        .Select(record => new { Column2 = record.Column2, Column3 = record.Column3 });

                    // Proses data sesuai kebutuhan
                    if (commodity.IsDatasetChanged)
                    {
                        foreach (var item in records)
                        {
                            var prediction = await _predictionRepository.GetByPredictionDate(DateOnly.ParseExact(item.Date, "yyyy-MM-dd"));
                            if (prediction != null)
                            {
                                prediction.Price = item.Price;
                                prediction.Commodity = commodity;
                                prediction.Areas = areas.ToList();

                                var update = await _predictionRepository.UpdatePrediction(prediction);
                                if (update == null)
                                    throw new RpcException(new Status(StatusCode.Internal, "Failed to update prediction"));
                            }
                            else
                            {
                                var predictionCreate = new Prediction
                                {
                                    Date = DateOnly.ParseExact(item.Date, "yyyy-MM-dd"),
                                    Price = item.Price,
                                    Commodity = commodity,
                                    Areas = areas.ToList()
                                };

                                var create = await _predictionRepository.CreatePrediction(predictionCreate);
                                if (create == null)
                                    throw new RpcException(new Status(StatusCode.Internal, "Failed to create prediction"));
                            }
                        }
                    }

                }*/
