using BrideChilla.Viewmodels;
using BrideChillaPOC.Views;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace BrideChillaPOC.Viewmodels
{
    public class CreateGuestPageViewModel : BaseViewModel
    {

        public ICommand ShowAlertCommand { get; set; } // alert command for empty field
        public ICommand ShowAlertSuccess { get; set; } // alert command for seccessful saving
        public ICommand BackToGuestList { get; set; } // back to guest list  

        private string _contactname;
        public string Name
        {
            get { return _contactname; }
            set { _contactname = value; }
        }

        private string _contactphone;
        public string Phone
        {
            get { return _contactphone; }
            set { _contactphone = value; }
        }

        public ICommand SaveGuestCommand { get; set; }
        public CreateGuestPageViewModel(INavigation navigation) : base(navigation)
        {
            SaveGuestCommand = new Command(OnSaveGuest); // Save guest
            ShowAlertCommand = new Command(get => MakeAlter()); // Alert for empty fields
            ShowAlertSuccess = new Command(get => MakeSuccessAlter()); // Alert for successful saving
            BackToGuestList = new Command(ToNavGuestList); // to guest list
        }
        // For empty fields
        void MakeAlter()
        {
            Application.Current.MainPage.DisplayAlert("Alert", "One of the fields is empty", "ok");
        }
        // For successful saving
        void MakeSuccessAlter()
        {
            Application.Current.MainPage.DisplayAlert("successfully", "Saved", "ok");
        }
        public void ToNavGuestList()
        {
            Navigation.PushAsync(new GuestList());
        }
        // Method to save contacts
        public void OnSaveGuest()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Phone))
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
                        //string insert_query = "insert into [dbo].[Guest] (Name,Phone,WeddingId) values ('" + Name + "','" + Phone + "','" + LoginPageViewModel.ActiveWedding + "')";
                        //string insert_query = "insert into [dbo].[Guest] (Name,Phone,Role_key, Wedding_key) values ('" + Name + "','" + Phone + "','" + LoginPageViewModel.CurrentUserID + "','" + LoginPageViewModel.ActiveWedding + "')";
                        string insert_query = "insert into [dbo].[Guest] (Name,Phone,Role_key, Wedding_key) values ('" + Name + "','" + Phone + "','" + 36 + "','" + LandingPageViewModel.ActiveWedding + "')";
                        connection.Open();
                        SqlCommand command = new SqlCommand(insert_query, connection);
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    Console.WriteLine("Done .....");
                    MakeSuccessAlter();
                    Navigation.PopAsync(); // navigate to DataGrid view page
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
