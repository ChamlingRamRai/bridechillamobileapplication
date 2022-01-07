using BrideChilla.Views;
using BrideChillaPOC.ViewModels;
using BrideChillaPOC.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

//This is for custom font to use and font's properties must be Embedded resource (exporting)
[assembly: ExportFont("Raleway-Black.ttf", Alias ="Raleway")]
[assembly: ExportFont("Raleway-Thin.ttf", Alias = "Raleway-thin")]
[assembly: ExportFont("Courgette-Regular.ttf", Alias = "Courgette-Regular")]
[assembly: ExportFont("Shrikhand-Regular.ttf", Alias = "Shrikhand-Regular")]
namespace BrideChillaPOC
{
    public partial class App : Application
    {
        internal static readonly object current;

        public App()
        {
            InitializeComponent();

          MainPage = new NavigationPage(new SplashPage());
         //MainPage = new NavigationPage(new DatePage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
