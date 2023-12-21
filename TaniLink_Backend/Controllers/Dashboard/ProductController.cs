using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaniLink_Backend.Interfaces;
using TaniLink_Backend.Models;
using TaniLink_Backend.ViewModels;

namespace TaniLink_Backend.Controllers.Dashboard
{
    [Area("Dashboard")]
    public class ProductController : Controller
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IProductRepository _productRepository;
        private readonly ISellerRepository _sellerRepository;
        private readonly ICommodityRepository _commodityRepository;
        private readonly IAreaRepository _areaRepository;

        public ProductController(IMapper mapper,
            UserManager<User> userManager,
            IProductRepository productRepository,
            ISellerRepository sellerRepository,
            ICommodityRepository commodityRepository,
            IAreaRepository areaRepository)
        {
            _mapper = mapper;
            _userManager = userManager;
            _productRepository = productRepository;
            _sellerRepository = sellerRepository;
            _commodityRepository = commodityRepository;
            _areaRepository = areaRepository;
        }

        [HttpGet]
        [Route("Dashboard/Product")]
        [Authorize(Roles = "Admin, Petani, Mitra")]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var seller = await _sellerRepository.GetSellerByUserId(userId);

            ViewBag.Products = await _productRepository.GetProductsBySellerId(seller.Id);
            ViewBag.Commodities = await _commodityRepository.GetAllCommodities();
            ViewBag.Areas = await _areaRepository.GetAllAreas();

            if (TempData.ContainsKey("IsSuccess"))
            {
                ViewBag.IsSuccess = TempData["IsSuccess"];
                ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Petani, Mitra")]
        [Route("Dashboard/Product")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Create(ProductViewModel productViewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["IsSuccess"] = false;
                TempData["SuccessMessage"] = $"Harus di isi semua";
                return Redirect("/Dashboard/Product");
            }

            var userId = _userManager.GetUserId(User);
            var commodity = await _commodityRepository.GetCommodityById(productViewModel.CommodityId);
            var area = await _areaRepository.GetAreaById(productViewModel.AreaId);

            var updatedProduct = _mapper.Map<Product>(productViewModel);
            updatedProduct.Commodity = commodity;
            updatedProduct.Area = area;
            updatedProduct.Seller = await _sellerRepository.GetSellerByUserId(userId);
            updatedProduct.Images = new List<ProductImage>
            {
                new ProductImage
                {
                    Image = productViewModel.Thumbnail
                }
            };

            var result = await _productRepository.CreateProduct(updatedProduct);
            if (result == null)
            {
                TempData["IsSuccess"] = false;
                TempData["SuccessMessage"] = "Gagal Menambah produk.";
                return Redirect("/Dashboard/Product");
            }

            TempData["IsSuccess"] = true;
            TempData["SuccessMessage"] = $"produk {productViewModel.Name} Berhasil Ditambahkan";
            return Redirect("/Dashboard/Product");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Petani, Mitra")]
        [Route("Dashboard/Product/{productId}/edit")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Update(string productId, ProductViewModel productViewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["IsSuccess"] = false;
                TempData["SuccessMessage"] = $"Harus di isi semua";
                return Redirect($"/Dashboard/Product/{productId}");
            }

            var product = await _productRepository.GetProductById(productId);
            var commodity = await _commodityRepository.GetCommodityById(productViewModel.CommodityId);
            var area = await _areaRepository.GetAreaById(productViewModel.AreaId);

            var updatedProduct = _mapper.Map(productViewModel, product);
            updatedProduct.Commodity = commodity;
            updatedProduct.Area = area;

            var result = await _productRepository.UpdateProduct(updatedProduct);
            if (result.UpdatedAt == null)
            {
                TempData["IsSuccess"] = false;
                TempData["SuccessMessage"] = "Gagal mengubah produk.";
                return Redirect($"/Dashboard/Product/{productId}");
            }

            TempData["IsSuccess"] = true;
            TempData["SuccessMessage"] = $"produk {productViewModel.Name} Berhasil Ubah";
            return Redirect($"/Dashboard/Product/{productId}");
        }

        [HttpDelete]
        [Authorize(Roles = "Admin, Petani, Mitra")]
        [Route("Dashboard/Product/{productId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Delete(string productId)
        {

            var result = await _productRepository.DeleteProduct(productId);
            if (result.DeletedAt == null)
                return Json(new { ok = false, message = "Gagal menghapus produk." });

            return Json(new { ok = true });
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Petani, Mitra")]
        [Route("Dashboard/Product/{productId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Detail(string productId)
        {
            ViewBag.Product = await _productRepository.GetProductById(productId);
            ViewBag.Commodities = await _commodityRepository.GetAllCommodities();
            ViewBag.Areas = await _areaRepository.GetAllAreas();

            if (TempData.ContainsKey("IsSuccess"))
            {
                ViewBag.IsSuccess = TempData["IsSuccess"];
                ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;
            }

            return View();
        }
    }
}
