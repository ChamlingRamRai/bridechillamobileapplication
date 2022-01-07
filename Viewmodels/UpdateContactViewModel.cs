using BrideChillaPOC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace BrideChillaPOC.Viewmodels
{
    public class UpdateContactViewModel : BaseViewModel
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
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
        //To select in order to remove/delete/update
        private Contact _contact;
        public Contact SelectedContact
        {
            get { return _contact; }
            set { _contact = value; }
        }
      
        public ICommand EditContactCommand { get; set; }  // to Update
        public ICommand EditSuccess { get; set; } // alert command for seccessful Edit
        public UpdateContactViewModel(INavigation navigation) : base(navigation)
        {
            Name = ContactListViewModel.EditContact.Name;
            Phone = ContactListViewModel.EditContact.Phone;
            Email = ContactListViewModel.EditContact.Email;
            EditContactCommand = new Command(UpdateContact);
            EditSuccess = new Command(get => EditSuccessAlter()); // Alert for successful Update

        }

        //Alert for successful Update
        void EditSuccessAlter()
        {
            Application.Current.MainPage.DisplayAlert("Your data is Updated", "", "ok");
        }

        // Update Contact
        public void UpdateContact()
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
                    string update_query = "UPDATE [dbo].[Contact] SET Name = '" + _contactname + "', Phone = '" + _contactphone + "', Email = '" + _contactemail + "' WHERE Contact_key = '" + ContactListViewModel.EditContact.Id + "' ";
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


