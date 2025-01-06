using System.ComponentModel.DataAnnotations;

namespace QuanLyChiTieuCaNhan.DTO.Budget
{
    public class CreateBudgetDto
    {
        [Required]
        public decimal AmountLimit { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int? CategoryId { get; set; }
    }
}
