namespace QuanLyChiTieuCaNhan.CustomExceptions.ValidationExceptions
{
    public class UnauthorizedException : CustomException
    {
        public UnauthorizedException()
            : base("You are not authorized to perform this action.", 401) { }
    }


}
