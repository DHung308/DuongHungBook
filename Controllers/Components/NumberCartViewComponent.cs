using Microsoft.AspNetCore.Mvc;
using DHungBooks.Extensions;
using DHungBooks.ModelViews;

namespace DHungBooks.Controllers.Components
{
    public class NumberCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");          
            return View(cart);
        }
    }
}
