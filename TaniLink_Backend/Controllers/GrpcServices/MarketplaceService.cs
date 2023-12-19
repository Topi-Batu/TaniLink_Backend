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
        private readonly IAddressRepository _addressRepository;
        private readonly ICommodityRepository _commodityRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly UserManager<User> _userManager;

        public MarketplaceService(IMapper mapper,
            IAddressRepository addressRepository,
            ICommodityRepository commodityRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            IShoppingCartRepository shoppingCartRepository,
            IInvoiceRepository invoiceRepository,
            UserManager<User> userManager)
        {
            _mapper = mapper;
            _addressRepository = addressRepository;
            _commodityRepository = commodityRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _invoiceRepository = invoiceRepository;
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

        public override async Task<AllCommodityDetails> GetAllCommodities(Empty request, ServerCallContext context)
        {

            try
            {
                var commodities = await _commodityRepository.GetAllCommodities();
                var commoditiesResponse = _mapper.Map<IEnumerable<CommodityDetail>>(commodities);
                var response = new AllCommodityDetails();
                response.Commodities.AddRange(commoditiesResponse);
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

        public override async Task<CommodityDetail> GetCommodityById(IdReq request, ServerCallContext context)
        {
            try
            {
                var commodity = await _commodityRepository.GetCommodityById(request.Id);
                var commodityResponse = _mapper.Map<CommodityDetail>(commodity);
                return commodityResponse;
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
        public override async Task<ShoppingCartDetail> GetShoppingCartById(IdReq request, ServerCallContext context)
        {
            try
            {
                var shoppingCart = await _shoppingCartRepository.GetShoppingCartById(request.Id);
                var shoppingCartResponse = _mapper.Map<ShoppingCartDetail>(shoppingCart);
                return shoppingCartResponse;
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
        public override async Task<OrderDetail> GetOrderById(IdReq request, ServerCallContext context)
        {
            try
            {
                var order = await _orderRepository.GetOrderById(request.Id);
                var orderResponse = _mapper.Map<OrderDetail>(order);
                return orderResponse;
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
        public override async Task<AllInvoiceDetail> GetUserInvoices(Empty request, ServerCallContext context)
        {
            try
            {
                var invoices = await _invoiceRepository.GetAllInvoicesByUser(context.GetHttpContext().User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value);
                var invoicesResponse = _mapper.Map<IEnumerable<InvoiceDetail>>(invoices);
                var response = new AllInvoiceDetail();
                response.Invoices.AddRange(invoicesResponse);
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
        public override async Task<InvoiceDetail> GetInvoiceById(IdReq request, ServerCallContext context)
        {
            try
            {
                var invoice = await _invoiceRepository.GetInvoiceById(request.Id);
                var invoiceResponse = _mapper.Map<InvoiceDetail>(invoice);
                return invoiceResponse;
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public override async Task<InvoiceDetail> Checkout(CheckoutReq request, ServerCallContext context)
        {
            try
            {
                var createdOrders = new List<Order>();

                // Group shopping carts by seller
                var groupedShoppingCarts = request.ShoppingCartId
                    .Select(async item => await _shoppingCartRepository.GetShoppingCartById(item))
                    .Select(cartTask => cartTask.Result)
                    .Where(shoppingCart => shoppingCart != null)
                    .GroupBy(shoppingCart => shoppingCart.Product.Seller.Id);

                // Check product availability before creating any orders
                if (groupedShoppingCarts.Any(sellerGroup => 
                    sellerGroup.Any(shoppingCart => 
                        shoppingCart.Product.AvailableStock < shoppingCart.Amount)))
                            throw new RpcException(new Status(StatusCode.NotFound, "Product out of stock"));

                // Iterate through each group and create an order for each seller
                foreach (var sellerGroup in groupedShoppingCarts)
                {
                    var weight = sellerGroup.Sum(shoppingCart => shoppingCart.Amount);
                    var order = new Order
                    {
                        Status = OrderStatus.Checkout,
                        Weight = weight,
                        DeliveryPrice = 2000 * weight,
                        ShoppingCart = sellerGroup.ToList()
                    };

                    // Save the order
                    await _orderRepository.CreateOrder(order);
                    createdOrders.Add(order);
                }

                // Create an invoice for all orders
                var invoice = new Invoice
                {
                    Status = InvoiceStatus.Unpaid,
                    TotalPrice = createdOrders.Sum(order => order.DeliveryPrice) + 
                                 createdOrders.Sum(order => order.ShoppingCart.Sum(sc => sc.Product.Price * sc.Amount)),
                    Orders = createdOrders
                };

                // Save the invoice
                var createInvoice = await _invoiceRepository.CreateInvoice(invoice);
                var createdInvoice = _mapper.Map<InvoiceDetail>(createInvoice);
                
                return createdInvoice;

                /*var allShoppingCarts = new List<ShoppingCart>();
                var allWeight = 0;
                var sellerTemp = "";
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
                    DeliveryPrice = 2000 * allWeight,
                    ShoppingCart = allShoppingCarts
                };

                var createOrder = await _orderRepository.CreateOrder(order);
                if (createOrder == null)
                    throw new RpcException(new Status(StatusCode.Internal, "Failed to checkout"));

                var createdOrder = _mapper.Map<OrderDetail>(createOrder);

                return createdOrder;*/
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
        public override async Task<InvoiceDetail> PlaceOrder(PlaceOrderReq request, ServerCallContext context)
        {
            try
            {
                var invoice = await _invoiceRepository.GetInvoiceById(request.InvoiceId);
                if (invoice == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Invoice not found"));

                var address = await _addressRepository.GetAddressById(request.AddressId);
                if (address == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Address not found"));

                foreach(var note in request.OrderNotes)
                {
                    var order = await _orderRepository.GetOrderById(note.OrderId);
                    if (order == null)
                        throw new RpcException(new Status(StatusCode.NotFound, "Order not found"));

                    order.Notes = note.Note;
                    order.Address = address;
                    order.Status = OrderStatus.Needpayment;
                    var updateOrder = await _orderRepository.UpdateOrder(order);
                    if (updateOrder == null)
                        throw new RpcException(new Status(StatusCode.Internal, "Failed to place order"));
                }

                return await GetInvoiceById(new IdReq { Id = request.InvoiceId }, context);
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
        public override async Task<OrderDetail> CancelOrder(IdReq request, ServerCallContext context)
        {
            try
            {
                var order = await _orderRepository.GetOrderById(request.Id);
                if (order == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Order not found"));

                if (order.Status == OrderStatus.Delivering || order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Finished || order.Status == OrderStatus.Processing)
                    throw new RpcException(new Status(StatusCode.NotFound, "Order can't be cancelled"));

                order.Status = OrderStatus.Cancelled;
                var updateOrder = await _orderRepository.UpdateOrder(order);
                if (updateOrder == null)
                    throw new RpcException(new Status(StatusCode.Internal, "Failed to cancel order"));

                return await GetOrderById(new IdReq { Id = request.Id }, context);
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
        public override async Task<OrderDetail> ConfirmOrder(IdReq request, ServerCallContext context)
        {
            try
            {
                var order = await _orderRepository.GetOrderById(request.Id);
                if (order == null)
                    throw new RpcException(new Status(StatusCode.NotFound, "Order not found"));

                if (order.Status == OrderStatus.Needpayment || order.Status == OrderStatus.Accepted || order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Checkout)
                    throw new RpcException(new Status(StatusCode.NotFound, "Order can't be confirmed"));

                order.Status = OrderStatus.Finished;
                var updateOrder = await _orderRepository.UpdateOrder(order);
                if (updateOrder == null)
                    throw new RpcException(new Status(StatusCode.Internal, "Failed to confirm order"));

                return await GetOrderById(new IdReq { Id = request.Id }, context);
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
