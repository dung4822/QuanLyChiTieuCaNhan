using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace QuanLyChiTieuCaNhan.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage ="Tên Người Dùng Bắt Buộc")]
        [StringLength(50,MinimumLength = 5, ErrorMessage ="Tên người dùng phải có ít nhất từ 5-50 ký tự")]
        public string FullName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        override
        public  string? PhoneNumber{ get; set; }
        public  ICollection<Category>? Categories { get; set; }

        public  ICollection<ExpenseTransaction>? ExpenseTransactions { get; set; }
        public  ICollection<Budget> Budgets { get; set; }
        public ICollection<RefreshToken> refreshTokens { get; set; }
    }
}
