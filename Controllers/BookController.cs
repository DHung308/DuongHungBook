using DHungBooks.Data;
using DHungBooks.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace DHungBooks.Controllers
{
    public class BookController : Controller
	{
		private readonly NguyenDuongHungBookContext _context;

		public BookController(NguyenDuongHungBookContext context)
		{
			_context = context;
		}

		[Route("shop.html", Name = "ShopBook")]
		public IActionResult Index(int? page)
		{
			try
			{
				var pageNumber = page == null || page <= 0 ? 1 : page.Value;
				var pageSize = 20; //Show 20 rows every page
				var lsProducts = _context.Books
				   .AsNoTracking()
				   .OrderByDescending(x => x.DateCreated);
				PagedList<Book> models = new PagedList<Book>(lsProducts, pageNumber, pageSize);
				ViewBag.CurrentPage = pageNumber;
                return View(models);
			}
			catch
			{
				return RedirectToAction("Index");
			}


		}

		[Route("/{Alias}", Name = "ListBook")]
		public IActionResult List(string Alias, int page = 1)
		{
			try
			{
                var pageSize = 20; //Show 20 rows every page
                var danhmuc = _context.Categories.AsNoTracking().SingleOrDefault(x => x.Alias == Alias);
                var lsPage = _context.Books
                    .AsNoTracking()
                    .Where(x => x.CaId == danhmuc.CaId)
                    .OrderByDescending(x => x.DateCreated);
                PagedList<Book> models = new PagedList<Book>(lsPage, page, pageSize);
                ViewBag.CurrentPage = page;
                ViewBag.CurrentCa = danhmuc;
                return View(models);
            }
			catch
			{
                return RedirectToAction("Index", "Home");
            }
			
		}

		[Route("/{Alias}--{id}.html", Name = "BookDetails")]
        public IActionResult Details(int id)
		{
            try
            {
                var product = _context.Books.Include(x => x.Ca).FirstOrDefault(x => x.BookId == id);
                if (product == null)
                {
                    return RedirectToAction("Index");
                }

                var lsProduct = _context.Books.AsNoTracking()
                    .Where(x => x.CaId == product.CaId && x.BookId != id && x.Active == true)
                    .OrderByDescending(x => x.DateCreated)
                    .Take(4)
                    .ToList();



                ViewBag.SanPham = lsProduct;
                return View(product);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
	}
}
