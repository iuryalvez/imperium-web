namespace backend.Interfaces
{
    public interface ICookieService
    {
        void SetCookieToken(HttpContext context, string token);
    }
}