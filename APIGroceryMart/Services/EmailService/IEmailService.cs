using GroceryStore.Data;

namespace GroceryStore.Services.EmailService
{
    public interface IEmailService
    {
        void SendEmail(Email request);
    }
}
