using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using BrideChillaPOC.ViewModels;
using System.Data.SqlClient;

namespace BrideChillaPOC.Viewmodels
{
    public class CreateContactPageViewModel :BaseViewModel
    {     
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
        private string _contactemail;
        public string Email
        {
            get { return _contactemail; }
            set { _contactemail = value; }
        }

        public ICommand ShowAlertCommand { get; set; } // alert command for empty field
        public ICommand ShowAlertSuccess { get; set; } // alert command for seccessful saving
        public ICommand SaveContactCommand { get; set; }
        public ICommand BackToContactList { get; set; }    
        public CreateContactPageViewModel(INavigation navigation) : base(navigation)
        {
            SaveContactCommand = new Command(OnSaveContact); // Save contact
            ShowAlertCommand = new Command(get => MakeAlter()); // Alert
            ShowAlertSuccess = new Command(get => MakeSuccessAlter()); // Alert for successful signup
            BackToContactList = new Command(OnNavToContactList); // To contact list page
        }
        // For empty fields
        void MakeAlter()
        {
            Application.Current.MainPage.DisplayAlert("Alert", "One of the fields is empty", "ok");
        }

        //back to contact list
        public void OnNavToContactList()
        {
            Navigation.PushAsync(new ContactList());
        }
        // For successful saving
        void MakeSuccessAlter()
        {
            Application.Current.MainPage.DisplayAlert("successfully", "Saved", "ok");
        }
        // Method to save contacts
        public void OnSaveContact()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Phone) || string.IsNullOrEmpty(Email))
            {
                Console.WriteLine("One of the fields is empty");
                MakeAlter(); // Alert method
            }else
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
                        //string insert_query = "insert into [dbo].[Contact] (Name,Phone,Email,Wedding_key) values ('" + Name + "','" + Phone + "','" + Email + "','" + LoginPageViewModel.ActiveWedding + "')";
                        string insert_query = "insert into [dbo].[Contact] (Name,Phone,Email,Wedding_key) values ('" + Name + "','" + Phone + "','" + Email + "','" + LandingPageViewModel.ActiveWedding + "')";
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
