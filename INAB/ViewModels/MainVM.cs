using INAB.ViewModels;

namespace INAB.ViewModels
{
    public class MainVM
    {
        public MainVM()
        {
            StatusVM = new MainPageViewModel();
            DataVM = new DataViewModel();
        }

        public MainPageViewModel StatusVM { get; set; }
        public DataViewModel DataVM { get; set; }
    }
}
