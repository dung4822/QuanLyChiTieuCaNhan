using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuanLyChiTieuCaNhan.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage ="Vui lòng không được để trống Tên Loại Chi Tiêu")]
        [MaxLength(100)]
        public string Name { get; set; }

        public bool IsIncome { get; set; } = false;
        public bool IsDelete { get; set; } = false;
        public string Description { get; set; }

        public string UserId { get; set; }
        public  User User { get; set; }

        public  ICollection<ExpenseTransaction> ? ExpenseTransactions { get; set; }
    }
}
