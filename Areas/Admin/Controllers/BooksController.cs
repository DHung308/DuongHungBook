using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DHungBooks.Data;
using DHungBooks.Models;
using PagedList.Core;
using Azure;
using DHungBooks.Helper;
using DHungBooks.Extensions;
using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;


namespace DHungBooks.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BooksController : Controller
    {
        private readonly NguyenDuongHungBookContext _context;
        public INotyfService _notyfService { get; }
        public dynamic CaId { get; private set; }

        public BooksController(NguyenDuongHungBookContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/Books
        public IActionResult Index(int page = 1, int CatID = 0)
        {
            var pageNumber = page;
            var pageSize = 20;

            List<Book> IsBook = new List<Book>();

            if (CatID != 0)
            {
                IsBook = _context.Books
                    .AsNoTracking()
                    .Where(x=>x.CaId == CatID)
                    .Include(x =>x.Ca)
                    .OrderByDescending(x => x.BookId).ToList();
            }
            else
            {
                IsBook = _context.Books
                    .AsNoTracking()
                    .Include(x => x.Ca)
                    .OrderByDescending(x => x.BookId).ToList();
            }

            PagedList<Book> models = new PagedList<Book>(IsBook.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentCateID = CatID;
            ViewBag.CurrentPage = pageNumber;
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CaId", "CaName", CatID);
            return View(models);
        }
        public IActionResult Filtter(int CatID = 0)
        {
            var url = $"/Admin/Books?CatID={CatID}";
            if (CatID == 0)
            {
                url = $"/Admin/Books";
            }

            return Json(new { status = "success", redirectUrl = url });
        }

        // GET: Admin/Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Ca)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Admin/Books/Create
        public IActionResult Create()
        {
            ViewData["CaId"] = new SelectList(_context.Categories, "CaId", "CaName");
            return View();
        }

        // POST: Admin/Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,BookName,ShortDesc,Description,CaId,Price,Discount,Thumb,DateCreated,DateModified,BestSellers,HomeFlag,Active,Tags,Title,Alias,MetaDesc,MetaKey,UnitslnStock")] Book book, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {
                book.BookName = Ext.ToTitleCase(book.BookName);
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Ext.ToUrlFriendly(book.BookName) + extension;
                    book.Thumb = await Utilities.UploadFile(fThumb, @"book", image.ToLower());
                }
                if (string.IsNullOrEmpty(book.Thumb))
                {
                    book.Thumb = "default.jpg";
                }
                book.Alias = Ext.ToUrlFriendly(book.BookName);
                book.DateModified = DateTime.Now;
                book.DateCreated = DateTime.Now;

                _context.Add(book);
                await _context.SaveChangesAsync();
                _notyfService.Success("Thêm sản phẩm thành công");
                return RedirectToAction(nameof(Index));
            }
            ViewData["CaId"] = new SelectList(_context.Categories, "CaId", "CaName", book.CaId);
            return View(book);
        }

        // GET: Admin/Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["CaId"] = new SelectList(_context.Categories, "CaId", "CaName", book.CaId);
            return View(book);
        }

        // POST: Admin/Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,BookName,ShortDesc,Description,CaId,Price,Discount,Thumb,DateCreated,DateModified,BestSellers,HomeFlag,Active,Tags,Title,Alias,MetaDesc,MetaKey,UnitslnStock")] Book book, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    book.BookName = Ext.ToTitleCase(book.BookName);
                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string image = Ext.ToUrlFriendly(book.BookName) + extension;
                        book.Thumb = await Utilities.UploadFile(fThumb, @"book", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(book.Thumb)) book.Thumb = "default.jpg";
                    book.Alias = Ext.ToUrlFriendly(book.BookName);
                    book.DateModified = DateTime.Now;

                    _context.Update(book);
                    _notyfService.Success("Lưu thành công");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
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
            ViewData["CaId"] = new SelectList(_context.Categories, "CaId", "CaName", book.CaId);
            return View(book);
        }

        // GET: Admin/Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Ca)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Admin/Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'NguyenDuongHungBookContext.Books'  is null.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }
            
            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
          return (_context.Books?.Any(e => e.BookId == id)).GetValueOrDefault();
        }
    }
}
