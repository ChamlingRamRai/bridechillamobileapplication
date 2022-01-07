using BrideChillaPOC.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using BrideChillaPOC.Models;
using System.Linq;
using BrideChillaPOC.Services;
using BrideChilla.Views;

namespace BrideChillaPOC.Viewmodels
{
    public class UserViewModel : BaseViewModel
    {
        public List<Religion> ReligionList { get; set; } // religion dropdown list
        public ICommand ShowAlertCommand { get; set; } // alert command for empty field
        public ICommand ShowAlertSuccess { get; set; } // alert command for seccessful signup
        public ICommand NavigateToRolePage { get; set; } // navigate to Role Page
        public ICommand BackToSplash { get; set; }  // to splash page


        private string _username;
        public string FirstName
        {
            get { return _username; }
            set { _username = value; }
        }

        private string _userlastname;
        public string LastName
        {
            get { return _userlastname; }
            set { _userlastname = value; }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private string _religionType;
        public string ReligionType
        {
            get { return _religionType; }
            set { _religionType = value; }
        }

        private object _selectReligion;
        public object SelectReligion
        {
            get { return _selectReligion; }
            set { _selectReligion = value; }
        }
        public static int ActiveWedding;
        public static int ActiveUser;
        public ICommand RegisterCommand { get; set; }

        public UserViewModel(INavigation navigation) : base(navigation)
        {
            ShowAlertCommand = new Command(get => MakeAlter()); // Alert
            ShowAlertSuccess = new Command(get => MakeSuccessAlter()); // Alert for successful signup
            ReligionList = GetReligions().OrderBy(t => t.ReligionType).ToList(); // Dropdown for religion
            RegisterCommand = new Command(OnRegister); // User Registration
            NavigateToRolePage = new Command(OnNavToRolePage); // navitation command
            BackToSplash = new Command(OnNavToSplashPage);
        }
        // For unsuccessful sign up
        void MakeAlter()
        {
            Application.Current.MainPage.DisplayAlert("Alert", "One of the fields is empty", "ok");
        }
        // For successful sign up
        void MakeSuccessAlter()
        {
            Application.Current.MainPage.DisplayAlert("Congratulation", "Sign up Successful", "ok");
        }

        // Back to Splash page
        public void OnNavToSplashPage()
        {
            Navigation.PushAsync(new SplashPage());
        }
        public List<Religion> GetReligions()
        {
            List<Religion> religions = new List<Religion>()
            {
                new Religion(){ReligionId = 1, ReligionType="Christian"},
                new Religion(){ReligionId = 2, ReligionType="Muslim"},
                new Religion(){ReligionId = 3, ReligionType="Other"}
            };
            return religions;

        }

        // Navigation to Role page
        public void OnNavToRolePage()
        {
            //await Navigation.PushAsync(new RolePage());
            Application.Current.MainPage.Navigation.PushAsync(new RolePage(), true);
        }
        // User Registration method
        public void OnRegister()
        {
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) 
                || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                Console.WriteLine("One of the fields is empty");
                MakeAlter(); // Alert method
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
                        string insert_query = "insert into [dbo].[User] (FirstName,LastName,Email,Password) values ('" + FirstName + "','" + LastName + "', '" + Email + "', '" + Hashing.hashPassword(Password) + "')";
                        string select_query = "SELECT MAX(User_key) FROM [dbo].[User]"; // To select and get latest User ID
                        connection.Open();
                        SqlCommand command = new SqlCommand(insert_query, connection);
                        SqlCommand command2 = new SqlCommand(select_query, connection);
                        command.ExecuteNonQuery();
                        using (SqlDataReader reader = command2.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                try
                                {
                                    ActiveUser = reader.GetInt32(0); // Read table index 0
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                }
                            }
                        }
                        connection.Close();
                    }
                    //Console.WriteLine("Done .....");
                    MakeSuccessAlter();
                    OnNavToRolePage();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            

        }
       
    }
}
