using AutoMapper;
using Grpc.Core;
using RestSharp;
using System.Threading;
using TaniLink_Backend.Interfaces;

namespace TaniLink_Backend.Controllers.GrpcServices
{
    public class PredictionService : Predictions.PredictionsBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IPredictionRepository _predictionRepository;
        private readonly ICommodityRepository _commodityRepository;
        private readonly IAreaRepository _areaRepository;

        public PredictionService(IMapper mapper,
            IConfiguration configuration,
            IPredictionRepository predictionRepository,
            ICommodityRepository commodityRepository,
            IAreaRepository areaRepository)
        {
            _mapper = mapper;
            _configuration = configuration;
            _predictionRepository = predictionRepository;
            _commodityRepository = commodityRepository;
            _areaRepository = areaRepository;
        }

       /* public override async Task<AllPredictionDetail> GetPredictions(PredictionReq request, ServerCallContext context)
        {
            try
            {
                var predictions = await _predictionRepository.GetAllPredictions();
                if (!predictions.Any())
                    throw new RpcException(new Status(StatusCode.NotFound, "Predictions not found"));

                var commodity = await _commodityRepository.GetCommodityById(request.CommodityId); 
                if (commodity == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Commodity not found"));

                var area = await _areaRepository.GetAreaById(request.AreaId);
                if (area == null)
                    throw new RpcException(new Status(StatusCode.NotFound, $"Area with id {request.AreaId} not found"));

                var baseUri = _configuration["ApiUrls:Prediction"];
                var endpoint = commodity.Name.ToLower().Replace(" ", "-");
                RestClient client = new RestClient(new RestClientOptions
                {
                    BaseUrl = new Uri(baseUri),
                });

                var apiReq = new RestRequest($"{endpoint}/{10}");
                var result = await client.GetAsync<Dictionary<string, string>>(apiReq);
            }
            catch (RpcException ex)
            {
                throw new RpcException(ex.Status);
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }

        }*/
    }
}
