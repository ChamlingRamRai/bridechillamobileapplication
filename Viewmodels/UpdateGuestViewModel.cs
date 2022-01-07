using BrideChillaPOC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace BrideChillaPOC.Viewmodels
{
    public class UpdateGuestViewModel : BaseViewModel
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _guestname;
        public string Name
        {
            get { return _guestname; }
            set { _guestname = value; }
        }

        private string _guestphone;
        public string Phone
        {
            get { return _guestphone; }
            set { _guestphone = value; }
        }

        //To select in order to remove/delete
        private Guest _guest;
        public Guest SelectedGuest
        {

            get { return _guest; }
            set { _guest = value; }
        }

      
        public ICommand EditGuestCommand { get; set; }  // to Update Guest
        public ICommand EditSuccess { get; set; } // alert command for seccessful Edit

        public UpdateGuestViewModel(INavigation navigation) : base(navigation)
        {
            Name = GuestListViewModel.EditGuest.Name;
            Phone = GuestListViewModel.EditGuest.Phone.ToString();
            EditGuestCommand = new Command(UpdateGuest);
            EditSuccess = new Command(get => EditSuccessAlter()); // Alert for successful Update
        }

        //Alert for successful Update
        void EditSuccessAlter()
        {
            Application.Current.MainPage.DisplayAlert("Your data is Updated", "", "ok");
        }

        public void UpdateGuest()
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
                    string update_query = "UPDATE [dbo].[Guest] SET Name = '" + _guestname + "', Phone = '" + _guestphone + "' WHERE Guest_key = '" + GuestListViewModel.EditGuest.Id + "' ";
                    connection.Open();
                    SqlCommand command = new SqlCommand(update_query, connection);

                    command.ExecuteNonQuery();
                    //Console.WriteLine(SelectedContact.Id);
                    connection.Close();
                }
                EditSuccessAlter(); // Alert method called
                Navigation.PopAsync(); // navigate to DataGrid view page
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
