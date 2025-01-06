using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuanLyChiTieuCaNhan.Models
{
    public class ExpenseTransaction
    {
        public int ExpenseTransactionId { get; set; }

        [Required(ErrorMessage ="Vui lòng nhập ngày thực hiện giao dịch này")]
        public DateTime TransactionDate { get; set; } 

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; } = 0;

        public string Note { get; set; }
        public bool IsDelete { get; set; } = false;
        public int CategoryId { get; set; }
        public  Category Category { get; set; }

        public string UserId { get; set; }
        public  User User { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
    }
}
