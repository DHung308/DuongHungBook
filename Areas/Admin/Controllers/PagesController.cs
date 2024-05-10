using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DHungBooks.Data;
using DHungBooks.Models;
using AspNetCoreHero.ToastNotification.Notyf;
using AspNetCoreHero.ToastNotification.Abstractions;
using PagedList.Core;
using DHungBooks.Helper;
using NuGet.Packaging.Signing;
using DHungBooks.Extensions;

namespace DHungBooks.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PagesController : Controller
    {
        private readonly NguyenDuongHungBookContext _context;
		public INotyfService _notyfService { get; }

		public PagesController(NguyenDuongHungBookContext context, INotyfService notyfService)
        {
            _context = context;
			_notyfService = notyfService;
		}

		// GET: Admin/Pages
		public IActionResult Index(int? page)
		{
			var pageNumber = page == null || page <= 0 ? 1 : page.Value;
			var pageSize = 20; //Show 10 rows every page
			var lsPage = _context.Pages
				.AsNoTracking()
				.OrderByDescending(x => x.PageId);
			PagedList<Page> models = new PagedList<Page>(lsPage, pageNumber, pageSize);
			ViewBag.CurrentPage = pageNumber;
			return View(models);
		}

		// GET: Admin/Pages/Details/5
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Pages == null)
            {
                return NotFound();
            }

            var page = await _context.Pages
                .FirstOrDefaultAsync(m => m.PageId == id);
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        // GET: Admin/Pages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Pages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PageId,PageName,Contents,Thumb,Published,Title,MetaDesc,MetaKey,Alias,CreateAt,Ordering")] Page page, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
			if (ModelState.IsValid)
			{
				if (fThumb != null)
				{
					string extension = Path.GetExtension(fThumb.FileName);
					string image = Ext.ToUrlFriendly(page.PageName) + extension;
					page.Thumb = await Utilities.UploadFile(fThumb, @"pages", image.ToLower());
				}
				if (string.IsNullOrEmpty(page.Thumb))
				{
					page.Thumb = "default.jpg";
				}
				page.Alias = Ext.ToUrlFriendly(page.PageName);
				_context.Add(page);
				await _context.SaveChangesAsync();
				_notyfService.Success("Thêm thành công");

				return RedirectToAction(nameof(Index));
			}
			return View(page);
		}

        // GET: Admin/Pages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pages == null)
            {
                return NotFound();
            }

            var page = await _context.Pages.FindAsync(id);
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        // POST: Admin/Pages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PageId,PageName,Contents,Thumb,Published,Title,MetaDesc,MetaKey,Alias,CreateAt,Ordering")] Page page, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
			if (id != page.PageId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					if (fThumb != null)
					{
						string extension = Path.GetExtension(fThumb.FileName);
						string image = Ext.ToUrlFriendly(page.PageName) + extension;
						page.Thumb = await Utilities.UploadFile(fThumb, @"pages", image.ToLower());
					}
					if (string.IsNullOrEmpty(page.Thumb))
					{
						page.Thumb = "default.jpg";
					}
					page.Alias = Ext.ToUrlFriendly(page.PageName);
					_context.Update(page);
					_notyfService.Success("Cập nhật thành công");
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!PageExists(page.PageId))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			return View(page);
		}

		// GET: Admin/Pages/Delete/5
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pages == null)
            {
                return NotFound();
            }

            var page = await _context.Pages
                .FirstOrDefaultAsync(m => m.PageId == id);
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        // POST: Admin/Pages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pages == null)
            {
                return Problem("Entity set 'NguyenDuongHungBookContext.Pages'  is null.");
            }
            var page = await _context.Pages.FindAsync(id);
            if (page != null)
            {
                _context.Pages.Remove(page);
            }

			_notyfService.Success("Xóa thành công");
			await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PageExists(int id)
        {
          return (_context.Pages?.Any(e => e.PageId == id)).GetValueOrDefault();
        }
    }
}
