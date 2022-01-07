using BrideChillaPOC.Viewmodels;
using BrideChillaPOC.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BrideChilla.Viewmodels
{
    public class SplashPageViewModel : BaseViewModel
    {
        // User signup navigation page command
        public ICommand SignupCommand { get; set; }

        // User Log in navigation page command
        public ICommand NavToLoginCommand { get; set; }
        public SplashPageViewModel(INavigation navition) : base(navition)
        {
            SignupCommand = new Command(async () => await OnNavSignupPage());
            NavToLoginCommand = new Command(async () => await OnNavLoginPage());
        }

        // Method to navigate to sign up page
        private async Task OnNavSignupPage()
        {
            await Navigation.PushAsync(new SignUp());
        }

        // Method to navigate to sign up page
        private async Task OnNavLoginPage()
        {
            await Navigation.PushAsync(new MainPage());
        }
    }
}

