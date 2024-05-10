using DHungBooks.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DHungBooks.Models;
using System.Diagnostics;

namespace DHungBooks.Areas.Admin.Controllers
{
    [Area("Admin")]
	public class SearchController : Controller
	{
		private readonly NguyenDuongHungBookContext _context;
		public SearchController(NguyenDuongHungBookContext context)
		{
			_context = context;
		}
		// GET: Search/FindBook 
		[HttpPost]
		public IActionResult FindBook(string keyword)
		{
			try
			{
				Debug.WriteLine("Debugging: Keyword received - " + keyword);
				List<Book> ls = new List<Book>();
				if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
				{
					return PartialView("ListBooksSearchPartial", null);
				}

				ls = _context.Books.AsNoTracking()
									  .Include(a => a.Ca)
									  .Where(x => x.BookName.Contains(keyword))
									  .OrderByDescending(x => x.BookName)
									  .Take(10)
									  .ToList();
				if (ls == null)
				{
					return PartialView("ListBooksSearchPartial", null);
				}
				else
				{
					return PartialView("ListBooksSearchPartial", ls);
				}

			}
			catch (Exception ex)
			{
				Debug.WriteLine("Debugging: Error - " + ex.ToString()); // Log the full exception details
				return PartialView("ListBooksSearchPartial", null);
			}

		}
	}
}