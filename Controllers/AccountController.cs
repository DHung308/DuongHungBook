using AspNetCoreHero.ToastNotification.Abstractions;
using DHungBooks.Helper;
using DHungBooks.Models;
using DHungBooks.ModelViews;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using DHungBooks.Extensions;
using DHungBooks.Data;


namespace DHungBooks.Controllers
{
    [Authorize]
	public class AccountController : Controller
	{
		private readonly NguyenDuongHungBookContext _context;
		public INotyfService _notyfService { get; }

		public AccountController(NguyenDuongHungBookContext context, INotyfService notyfService)
		{
			_context = context;
			_notyfService = notyfService;
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ValidatePhone(string Phone)
		{
			try
			{
				var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Phone.ToLower() == Phone.ToLower());
				if (khachhang != null)
					return Json(data: "Số điện thoại : " + Phone + "Đã được sử dụng ");
				return Json(data: true);
			}
			catch
			{
				return Json(data: true);
			}
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ValidateEmail(string Email)
		{
			try
			{
				var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.ToLower() == Email.ToLower());
				if (khachhang != null)
					return Json(data: "Email : " + Email + "Đã được sử dụng ");
				return Json(data: true);
			}
			catch
			{
				return Json(data: true);
			}
		}

		[Authorize]
		[Route("tai-khoan-cua-toi.html", Name = "Dashboard")]
		public IActionResult Dashboard()
		{
			var taikhoanID = HttpContext.Session.GetString("CustomerId");
			if (taikhoanID != null)
			{
				var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
				if (khachhang != null)
				{
					var lsDonHang = _context.Orders
						.Include(x => x.TransactStatus)
						.AsNoTracking()
						.Where(x => x.CustomerId == khachhang.CustomerId)
						.OrderByDescending(x => x.OrderDate).ToList();
					ViewBag.DonHang = lsDonHang;
					return View(khachhang);
				}
			}
			return RedirectToAction("Login");
		}

		[HttpGet]
		[AllowAnonymous]
		[Route("dang-ky.html", Name = "DangKy")]
		public IActionResult DangKyTaiKhoan()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[Route("dang-ky.html", Name = "DangKy")]
		public async Task<IActionResult> DangKyTaiKhoan(RegisterVM taikhoan)
		{
			try
			{
				if (ModelState.IsValid)
				{
					string salt = Utilities.GetRandomKey();
					Customer khachhang = new Customer
					{
						FullName = taikhoan.FullName,
						Phone = taikhoan.Phone,
						Email = taikhoan.Email,
						Password = (taikhoan.Password + salt.Trim()).ToMD5(),
						Active = true,
						Salt = salt,
					};
					try
					{
						_context.Add(khachhang);
						await _context.SaveChangesAsync();

						HttpContext.Session.SetString("CustomerId", khachhang.CustomerId.ToString());
						var taiKhoanID = HttpContext.Session.GetString("CustomerId");

						var claims = new List<Claim>
						{
							new Claim(ClaimTypes.Name, khachhang.FullName),
							new Claim("CustomerId", khachhang.CustomerId.ToString())
						};
						ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
						ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
						await HttpContext.SignInAsync(claimsPrincipal);
						_notyfService.Success("Đăng ký tài khoản thành công ");
						return RedirectToAction("Dashboard", "Account");
					}
					catch (Exception ex)
					{
						return RedirectToAction("DangKyTaiKhoan", "Account");
					}
				}
				else
				{
					return View(taikhoan);
				}
			}
			catch
			{
				return View(taikhoan);
			}
		}


		[AllowAnonymous]
		[Route("dang-nhap.html", Name = "DangNhap")]
		public IActionResult Login(string returnUrl = null)
		{
			var taikhoanID = HttpContext.Session.GetString("CustomerId");
			if (taikhoanID != null)
			{
				return RedirectToAction("Dashboard", "Account");
			}
			return View(new LoginViewModel { ReturnUrl = returnUrl });
		}

		[HttpPost]
		[AllowAnonymous]
		[Route("dang-nhap.html", Name = "DangNhap")]
		public async Task<IActionResult> Login(LoginViewModel customer, string returnUrl = null)
		{
			try
			{
				bool isEmail = Utilities.IsValidEmail(customer.UserName);
				if (!isEmail)
				{
					return View(customer);
				}

				var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == customer.UserName);

				if (khachhang == null)
				{
					return RedirectToAction("DangKyTaiKhoan");
				}
				string pass = (customer.Password + khachhang.Salt.Trim()).ToMD5();
				if (khachhang.Password != pass)
				{
					_notyfService.Warning("Thông tin đăng nhập không chính xác");
					return View(customer);
				}

				//Kiểm tra tài khoản có bị disable không?
				if (khachhang.Active == false)
				{
					return RedirectToAction("Index", "Home");
				}

				//Lưu session vào MaKH
				HttpContext.Session.SetString("CustomerId", khachhang.CustomerId.ToString());
				var taikhoanID = HttpContext.Session.GetString("CustomerId");
				//Identity
				var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, khachhang.FullName),
						new Claim("CustomerId", khachhang.CustomerId.ToString())
					};
				ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
				ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
				await HttpContext.SignInAsync(claimsPrincipal);
				_notyfService.Success("Đăng nhập thành công");
				return RedirectToAction("Dashboard", "Account");
			}
			catch
			{
				return RedirectToAction("DangKyTaiKhoan", "Account");
			}
			_notyfService.Warning("Lỗi");
			return View(customer);
		}

		[HttpGet]
		[Route("dang-xuat.html", Name = "Logout")]
		public IActionResult Logout()
		{
			HttpContext.SignOutAsync();
			HttpContext.Session.Remove("CustomerId");
			return RedirectToAction("Index", "Home");
		}


		[HttpPost]
		public IActionResult ChangePassword(ChangePasswordViewModel model)
		{
			try
			{
				var taikhoanID = HttpContext.Session.GetString("CustomerId");
				if (taikhoanID == null)
				{
					return RedirectToAction("Login", "Account");
				}
				if (ModelState.IsValid)
				{
					var taikhoan = _context.Customers.Find(Convert.ToInt32(taikhoanID));
					if (taikhoan == null)
					{
						return RedirectToAction("Login", "Account");
					}
					var pass = (model.PasswordNow.Trim() + taikhoan.Salt.Trim()).ToMD5();
					if (pass == taikhoan.Password)
					{
						string passnew = (model.Password.Trim() + taikhoan.Salt.Trim()).ToMD5();
						taikhoan.Password = passnew;
						_context.Update(taikhoan);
						_context.SaveChanges();
						_notyfService.Success("Cập nhật tài khoản thành công");
						return RedirectToAction("Dashboard", "Account");
					}
				}

			}
			catch
			{
				_notyfService.Error("Cập nhật tài khoản không thành công");
				return RedirectToAction("Dashboard", "Account");
			}
			_notyfService.Error("Cập nhật tài khoản không thành công");
			return RedirectToAction("Dashboard", "Account");
		}
	}
}