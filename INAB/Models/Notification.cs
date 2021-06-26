
namespace INAB.Models
{
    public class Notification
    {
        public string Message { get; private set; }
        public Notification(string _msg = "")
        {
            Message = _msg;
        }
    }
}
