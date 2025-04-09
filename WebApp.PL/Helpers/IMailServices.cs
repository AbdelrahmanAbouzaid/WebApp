using WebApp.PL.Dtos;

namespace WebApp.PL.Helpers
{
    public interface IMailServices
    {
        bool SendEmail(Email email);
    }
}
