using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using TaniLink_Backend.Interfaces;

namespace TaniLink_Backend.Controllers.GrpcServices
{
    public class MarketplaceService : Marketplace.MarketplaceBase
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public MarketplaceService(IMapper mapper,
            IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public override async Task<AllProductDetails> GetAllProducts(Empty request, ServerCallContext context)
        {

            try {
                var products = await _productRepository.GetAllProducts();
                var productsResponse = _mapper.Map<IEnumerable<ProductDetail>>(products);
                var response = new AllProductDetails();
                response.Products.AddRange(productsResponse);
                return response;
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

        public override async Task<ProductDetail> GetProductById(IdReq request, ServerCallContext context)
        {
            try {
                var product = await _productRepository.GetProductById(request.Id);
                var productResponse = _mapper.Map<ProductDetail>(product);
                return productResponse;
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

        public override async Task<AllProductDetails> GetProductByAreaId(IdReq request, ServerCallContext context)
        {
            try { 
                var products = await _productRepository.GetProductsByAreaId(request.Id);
                var productsResponse = _mapper.Map<IEnumerable<ProductDetail>>(products);
                var response = new AllProductDetails();
                response.Products.AddRange(productsResponse);
                return response;
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

        public override async Task<AllProductDetails> GetProductByCommodityId(IdReq request, ServerCallContext context)
        {
            try { 
                var products = await _productRepository.GetProductsByCommodityId(request.Id);
                var productsResponse = _mapper.Map<IEnumerable<ProductDetail>>(products);
                var response = new AllProductDetails();
                response.Products.AddRange(productsResponse);
                return response;
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

        public override async Task<AllProductDetails> GetProductBySellerId(IdReq request, ServerCallContext context)
        {
            try
            {
                var products = await _productRepository.GetProductsBySellerId(request.Id);
                var productsResponse = _mapper.Map<IEnumerable<ProductDetail>>(products);
                var response = new AllProductDetails();
                response.Products.AddRange(productsResponse);
                return response;
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
}
