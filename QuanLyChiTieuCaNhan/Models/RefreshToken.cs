namespace QuanLyChiTieuCaNhan.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } // Chuỗi Refresh Token (random)
        public string UserId { get; set; } // Liên kết đến bảng User
        public DateTime Expiration { get; set; } // Thời gian hết hạn
        public bool IsActive { get; set; } = true; // Trạng thái token (còn hiệu lực)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Ngày tạo token
        public string CreatedByIp { get; set; } // IP tạo token
    }

}
