namespace QuanLyChiTieuCaNhan.DTO.Auth
{
    public class UserResponseDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string ? PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
