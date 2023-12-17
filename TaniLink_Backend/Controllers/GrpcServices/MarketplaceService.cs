using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TaniLink_Backend.Interfaces;
using TaniLink_Backend.Models;

namespace TaniLink_Backend.Controllers.GrpcServices
{
    public class MarketplaceService : Marketplace.MarketplaceBase
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly UserManager<User> _userManager;

        public MarketplaceService(IMapper mapper,
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            IShoppingCartRepository shoppingCartRepository,
            UserManager<User> userManager)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _userManager = userManager;
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

        public override async Task<AllProductDetails> GetProductByPriceRange(PriceRange request, ServerCallContext context)
        {
            try
            {
                var products = await _productRepository.GetProductByPriceRange(Convert.ToDecimal(request.MinValue), Convert.ToDecimal(request.MaxValue));
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

        public override async Task<AllProductDetails> SearchProduct(Query request, ServerCallContext context)
        {
            try
            {
                var products = await _productRepository.GetProductsBySearch(request.Query_);
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public override  async Task<AllShoppingCartDetail> GetUserShoppingCarts(Empty request, ServerCallContext context)
        {
            try
            {
                var shoppingCarts = await _shoppingCartRepository.GetAllShoppingCartsByUser(context.GetHttpContext().User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
                var skipIfInOrder = shoppingCarts.Where(sc => !sc.Orders.Any()).ToList();
                var shoppingCartsResponse = _mapper.Map<IEnumerable<ShoppingCartDetail>>(skipIfInOrder);
                var response = new AllShoppingCartDetail();
                response.ShoppingCarts.AddRange(shoppingCartsResponse);
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public override async Task<AllOrderDetail> GetUserOrders(Empty request, ServerCallContext context)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersByBuyerId(context.GetHttpContext().User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
                var orderResponse = _mapper.Map<IEnumerable<OrderDetail>>(orders);
                var response = new AllOrderDetail();
                response.Orders.AddRange(orderResponse);
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public override async Task<AllShoppingCartDetail> AddProductToShoppingCart(IdReq request, ServerCallContext context)
        {
            try
            {
                var product = await _productRepository.GetProductById(request.Id);
                if (product == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Product not found"));

                if (product.AvailableStock == 0)
                    throw new RpcException(new Status(StatusCode.NotFound, "Product out of stock"));

                if (product.Seller.User.Id == context.GetHttpContext().User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value)
                    throw new RpcException(new Status(StatusCode.NotFound, "You can't buy your own product"));

                var shoppingCart = await _shoppingCartRepository.GetAllShoppingCartsByUser(context.GetHttpContext().User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
                var shoppingCartExist = shoppingCart.FirstOrDefault(sc => sc.Product.Id == product.Id);
                if (shoppingCartExist != null)
                {
                    shoppingCartExist.Amount += 1;
                    var updateShoppingCart = await _shoppingCartRepository.UpdateShoppingCart(shoppingCartExist);
                }
                else
                {
                    var newShoppingCart = new ShoppingCart
                    {
                        Product = product,
                        Amount = 1,
                        User = await _userManager.FindByIdAsync(context.GetHttpContext().User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value)
                    };
                    var createShoppingChart = await _shoppingCartRepository.CreateShoppingCart(newShoppingCart);
                    if (createShoppingChart == null)
                        throw new RpcException(new Status(StatusCode.Internal, "Failed to add product to shopping cart"));
                }

                return await GetUserShoppingCarts(new Empty(), context);
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public override async Task<AllShoppingCartDetail> DecreaseProductInShoppingCart(IdReq request, ServerCallContext context)
        {
            try
            {
                var product = await _productRepository.GetProductById(request.Id);
                if (product == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Product not found"));

                var shoppingCart = await _shoppingCartRepository.GetAllShoppingCartsByUser(context.GetHttpContext().User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
                var shoppingCartExist = shoppingCart.FirstOrDefault(sc => sc.Product.Id == product.Id);
                if (shoppingCartExist != null)
                {
                    if (shoppingCartExist.Amount == 1)
                    {
                        var deleteShoppingCart = await _shoppingCartRepository.DeleteShoppingCart(shoppingCartExist.Id);
                        if (deleteShoppingCart == null)
                            throw new RpcException(new Status(StatusCode.Internal, "Failed to decrease product in shopping cart"));
                    }
                    else
                    {
                        shoppingCartExist.Amount -= 1;
                        var updateShoppingCart = await _shoppingCartRepository.UpdateShoppingCart(shoppingCartExist);
                    }
                }
                else
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Product not found in shopping cart"));
                }

                return await GetUserShoppingCarts(new Empty(), context);
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public override async Task<AllShoppingCartDetail> RemoveProductFromShoppingCart(IdReq request, ServerCallContext context)
        {
            try
            {
                var product = await _productRepository.GetProductById(request.Id);
                if (product == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Product not found"));

                var shoppingCart = await _shoppingCartRepository.GetAllShoppingCartsByUser(context.GetHttpContext().User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
                var shoppingCartExist = shoppingCart.FirstOrDefault(sc => sc.Product.Id == product.Id);
                if (shoppingCartExist != null)
                {
                    var deleteShoppingCart = await _shoppingCartRepository.DeleteShoppingCart(shoppingCartExist.Id);
                    if (deleteShoppingCart == null)
                        throw new RpcException(new Status(StatusCode.Internal, "Failed to delete product in shopping cart"));
                }
                else
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Product not found in shopping cart"));
                }

                return await GetUserShoppingCarts(new Empty(), context);
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

        /*[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public override async Task<OrderDetail> Checkout(CheckoutReq request, ServerCallContext context)
        {
            try
            {
                var allShoppingCarts = new List<ShoppingCart>();
                var allWeight = 0;
                foreach (var item in request.ShoppingCartId)
                {
                    var shoppingCart = await _shoppingCartRepository.GetShoppingCartById(item);
                    if (shoppingCart == null)
                        throw new RpcException(new Status(StatusCode.NotFound, "Shopping cart not found"));

                    if (shoppingCart.User.Id != context.GetHttpContext().User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value)
                        throw new RpcException(new Status(StatusCode.NotFound, "Shopping cart not found"));

                    if (shoppingCart.Product.AvailableStock < shoppingCart.Amount)
                        throw new RpcException(new Status(StatusCode.NotFound, "Product out of stock"));

                    allShoppingCarts.Add(shoppingCart);
                    allWeight += shoppingCart.Amount;
                }

                var order = new Order
                {
                    Status = OrderStatus.Checkout,
                    Weight = allWeight,
                    DeliveryPrice = 2000*allWeight,
                    ShoppingCart = allShoppingCarts
                };

                var createOrder = await _orderRepository.CreateOrder(order);
                if (createOrder == null)
                    throw new RpcException(new Status(StatusCode.Internal, "Failed to checkout"));

                var createdOrder = _mapper.Map<OrderDetail>(createOrder);

                return createdOrder;
            }
            catch (RpcException ex)
            {
                throw new RpcException(ex.Status);
            }
            catch (Exception)
            {
                throw new RpcException(new Status(StatusCode.Internal, "Failed to checkout"));
            }
        }*/
    }
}
