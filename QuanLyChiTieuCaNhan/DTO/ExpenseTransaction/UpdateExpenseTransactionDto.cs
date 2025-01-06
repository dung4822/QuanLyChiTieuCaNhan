namespace QuanLyChiTieuCaNhan.DTO.ExpenseTransaction
{
    public class UpdateExpenseTransactionDto
    {
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string Note { get; set; }
        public int CategoryId { get; set; }
    }
}
