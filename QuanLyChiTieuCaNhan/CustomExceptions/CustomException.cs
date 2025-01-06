namespace QuanLyChiTieuCaNhan.CustomExceptions
{
    public class CustomException : Exception
    {

        public int StatusCode { get; } // Mã HTTP tương ứng với lỗi

        protected CustomException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
