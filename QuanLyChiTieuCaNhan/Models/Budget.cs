using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuanLyChiTieuCaNhan.Models
{
    public class Budget
    {
        
        public int BudgetId { get; set; }

        public decimal AmountLimit { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsDeleted { get; set; } = false;

        public string UserId { get; set; }
        public User User { get; set; }

        public int? CategoryId { get; set; }
        public  Category Category { get; set; }
    }
}
