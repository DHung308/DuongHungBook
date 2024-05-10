using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DHungBooks.ModelViews
{
	public class RegisterVM
	{
		[Key]
		public int CustomerId { get; set; }

		[Display(Name = "Họ và tên")]
		[Required(ErrorMessage = "Vui lòng nhập họ tên")]
		public string FullName { get; set; }


		[Required(ErrorMessage = "Vui lòng nhập Email")]
		[MaxLength(150)]
		[DataType(DataType.EmailAddress)]
		[Remote(action: "ValidateEmail", controller: "Account")]
		public string Email { get; set; }

		[MaxLength(11)]
		[Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
		[Display(Name = "Điện thoại")]
		[DataType(DataType.PhoneNumber)]
		[Remote(action: "ValidatePhone", controller: "Account")]
		public string Phone { get; set; }

		[Display(Name = "Mật khẩu")]
		[Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
		[MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiếu 5 kí tự")]
		public string Password { get; set; }

		[MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiếu 5 kí tự")]
		[Display(Name = "Nhập lại mật khẩu")]
		[Compare("Password", ErrorMessage = "Mật khẩu không khớp. Vui lòng nhập lại")]
		public string ConfirmPassword { get; set; }
	}
}