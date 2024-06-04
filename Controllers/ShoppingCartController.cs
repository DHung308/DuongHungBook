using AspNetCoreHero.ToastNotification.Abstractions;
using DHungBooks.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DHungBooks.Extensions;
using DHungBooks.Models;
using DHungBooks.ModelViews;
using DHungBooks.Data;

namespace DHungBooks.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly NguyenDuongHungBookContext _context;
        public INotyfService _notyfService { get; }

        public ShoppingCartController(NguyenDuongHungBookContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }


        public List<CartItem> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (gh == default(List<CartItem>))
                {
                    gh = new List<CartItem>();
                }
                return gh;
            }
        }


        [HttpPost]
        [Route("api/cart/add")]
        public IActionResult AddToCart(int productID, int? quantity)
        {
            List<CartItem> cart = GioHang;
            // thêm sản phảm vào giỏ hàng
            try
            {
                CartItem item = cart.SingleOrDefault(p => p.product.BookId == productID);
                if (item != null)
                {
                    item.amount = item.amount + quantity.Value;
                    //Lưu lại session
                    HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                }
                else
                {
                    Book hh = _context.Books.SingleOrDefault(p => p.BookId == productID);
                    item = new CartItem
                    {
                        amount = quantity.HasValue ? quantity.Value : 1,
                        product = hh
                    };
                    cart.Add(item); // thêm vào giỏ
                }
                // lưu lại session
                HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                _notyfService.Success("Sản phẩm đã được thêm vào giỏ hàng");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }


        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int productID, int? quantity)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            try
            {
                if (cart != null)
                {
                    CartItem item = cart.SingleOrDefault(p => p.product.BookId == productID);
                    if (item != null && quantity.HasValue)
                    {
                        item.amount = quantity.Value;
                    }
                    // Lưu lại session
                    HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                }
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }


        [HttpPost]
        [Route("api/cart/remove")]
        public IActionResult Remove(int productID)
        {
            try
            {
                List<CartItem> gioHang = GioHang;
                CartItem item = gioHang.SingleOrDefault(p => p.product.BookId == productID);
                if (item != null)
                {
                    gioHang.Remove(item);
                }
                // lưu lại session
                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }



        [Route("cart.html", Name = "Cart")]
        public IActionResult Index()
        {
            return View(GioHang);
        }
    }
}
