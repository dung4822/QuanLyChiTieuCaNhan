namespace QuanLyChiTieuCaNhan.DTO.Budget
{
    public class BudgetDto
    {
        public int Id { get; set; }
        public decimal AmountLimit { get; set; }
        public decimal ActualSpending { get; set; } // Chi tiêu thực tế
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsOverBudget { get; set; } // Cờ báo vượt ngân sách
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
