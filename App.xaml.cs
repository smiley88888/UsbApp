namespace UsbApp
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; private set; }

        public App()
        {
            InitializeComponent();

            var mauiApp = MauiProgram.CreateMauiApp();
            Services = mauiApp.Services;

            MainPage = new MainPage();
        }
    }
}
