using Microsoft.Maui.Controls;

namespace AplicacionAuto
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new DatosCoche());
        }

        protected override void OnStart()
        {
            // Optional: Handle app start logic
            base.OnStart();
        }

        protected override void OnSleep()
        {
            // Optional: Handle app going to background
            base.OnSleep();
        }

        protected override void OnResume()
        {
            // Optional: Handle app resuming from background
            base.OnResume();
        }
    }
}