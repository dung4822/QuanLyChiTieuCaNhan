namespace QuanLyChiTieuCaNhan.CustomExceptions.ResourceExceptions
{
    public class NotFoundException : CustomException
    {
        public NotFoundException(string resourceName, object resourceId)
            : base($"{resourceName} with ID {resourceId} was not found.", 404) { }
    }


    public class DuplicateResourceException : CustomException
    {
        public DuplicateResourceException(string resourceName)
            : base($"{resourceName} already exists.",404) { }
    }
    public class NotFoundEmailException : CustomException
    {
        public NotFoundEmailException(string Email)
            : base($"Not Found with Email {Email} was not found.", 404) { }
    }
}
