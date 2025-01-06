using System.ComponentModel.DataAnnotations;

namespace QuanLyChiTieuCaNhan.DTO.ExpenseTransaction
{
    public class CreateExpenseTransactionDto
    {
        [Required(ErrorMessage = "Ngày thực hiện giao dịch là bắt buộc.")]
        public DateTime TransactionDate { get; set; }

        [Required(ErrorMessage = "Số tiền là bắt buộc.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0.")]
        public decimal Amount { get; set; }

        public string Note { get; set; }
        [Required(ErrorMessage = "CategoryId là bắt buộc.")]
        public int CategoryId { get; set; }
    }
}
