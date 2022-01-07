using BrideChilla.Views;
using BrideChillaPOC.Services;
using BrideChillaPOC.ViewModels;
using BrideChillaPOC.Views;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BrideChillaPOC.Viewmodels
{
    public class LoginPageViewModel : BaseViewModel
    {
        public ICommand LoginAlertCommand { get; set; } // alert command for empty field
        public ICommand WrongUserCommand { get; set; } // alert command for wrong data
        public ICommand BackToSplash { get; set; }  // to splash page

        private string _userloginname;
        public string UserName
        {
            get { return _userloginname; }
            set { _userloginname = value; }
        }
        public static int ActiveWedding;
        public static int CurrentUserID;
        private string _userlastname;
        public string LoginPassword
        {
            get { return _userlastname; }
            set { _userlastname = value; }
        }
       
        // User Login command
        public ICommand LoginCommand { get; set; }

        public LoginPageViewModel(INavigation navigation) : base(navigation)
        {
            LoginAlertCommand = new Command(get => MakeLoginAlert()); // Alert for empty field
            WrongUserCommand = new Command(get => MakeWrongUserAlert()); // Alert wrong user data
            LoginCommand = new Command(OnLogin);
            BackToSplash = new Command(OnNavToSplashPage);
        }

        // For empty field alert
        void MakeLoginAlert()
        {
            Application.Current.MainPage.DisplayAlert("Alert", "One of the field is empty", "ok");
        }

        // For empty field alert
        void MakeWrongUserAlert()
        {
            Application.Current.MainPage.DisplayAlert("Alert", "Wrong User name or Password ", "ok");
        }

        // Back to Splash page
        public void OnNavToSplashPage()
        {
            Navigation.PushAsync(new SplashPage());
        }
        // Login method
        public void OnLogin()
        {

            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(LoginPassword))
            {
                Console.WriteLine("One of the field is empty");
                MakeLoginAlert(); // Alert method
            }
            else
           {
                try
                {
                    var cb = new SqlConnectionStringBuilder();
                    cb.DataSource = DBParameters.DataSource;
                    cb.UserID = DBParameters.UserID;
                    cb.Password = DBParameters.DBPass;
                    cb.InitialCatalog = DBParameters.InitCatalog;

                    using (var connection = new SqlConnection(cb.ConnectionString))
                    {
                        string fetch_query = "SELECT * FROM [dbo].[User] WHERE Email ='" + UserName.Trim() + "'AND Password ='" + Hashing.hashPassword(LoginPassword.Trim()) + "'";
                        connection.Open();
                        //SqlCommand command = new SqlCommand(fetch_query, connection);
                        //command.ExecuteNonQuery();
                        //connection.Close();
                        using (var command = new SqlCommand(fetch_query, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if(reader.Read())
                                {
                                    CurrentUserID = reader.GetInt32(0);
                                    ActiveWedding = reader.GetInt32(0);
                                    Console.WriteLine(UserName);
                                    //Navigation.PushModalAsync(new LandingPage()); // navigate to Landing page without back navigation page
                                    //Navigation.PushAsync(new LandingPage()); // navigate to Landing page
                                    
                                        Navigation.PushAsync(new LandingPage()); //navigate to Landing page                                  
                                }
                                else 
                                { MakeWrongUserAlert(); } // alert method called
                            }
                        }
                    }
                    
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
