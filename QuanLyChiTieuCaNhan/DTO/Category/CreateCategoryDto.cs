﻿using System.ComponentModel.DataAnnotations;

namespace QuanLyChiTieuCaNhan.DTO.Category
{
    public class CreateCategoryDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public bool IsIncome { get; set; }
        public string Description { get; set; }
    }
}
