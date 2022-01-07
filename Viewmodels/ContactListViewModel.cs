using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BrideChillaPOC.Models;
using BrideChillaPOC.ViewModels;
using BrideChillaPOC.Views;
using Xamarin.Forms;

namespace BrideChillaPOC.Viewmodels
{
    public class ContactListViewModel : BaseViewModel
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

        //To select in order to remove/delete
        private Contact _contact;
        public Contact SelectedContact {

            get { return _contact; }
            set { _contact = value; }
        }

        //To get Name and Phone for update
        public static Contact EditContact
        {
            get;
            set;
        }

        public ICommand DeletetSuccess { get; set; } // alert command for seccessful Delete
        public ICommand RowSelectionValidate { get; set; } // alert for row selection
        public ICommand AddListCommand { get; set; } // for Navigation
        //public ICommand SelectedContact { get; set; } //
        public ICommand DeleteContactCommand { get; set; }  // for Delete
        public ICommand NavigateToSavePage { get; set; } // navigate to save page when button is clicked
        public ICommand BackToLandingPage { get; set; } // back to landing page
        public ObservableCollection<Contact> Contacts { get; set; }

        public ContactListViewModel(INavigation navigation) : base(navigation)
        {
            NavigateToSavePage = new Command(async () => await OnPopupUpdateContact());
            AddListCommand = new Command(async () => await OnPopupCreateContact());
            LoadPage();
            DeleteContactCommand = new Command(DeleteContact);// Delete contact
            DeletetSuccess = new Command(get => SuccessDelete());
            RowSelectionValidate = new Command(get => SelectRow());
            BackToLandingPage = new Command(OnNavToLandingPage);
        }


        void SuccessDelete()
        {
            Application.Current.MainPage.DisplayAlert("Your data is Deleted", "", "ok");
        }

        void SelectRow() // alert for row selection
        {
            Application.Current.MainPage.DisplayAlert("Please select an item", "", "ok");
        }

        // Method to navigate to landing page
        public void OnNavToLandingPage()
        {
            Navigation.PushAsync(new LandingPage());
        }
        //Delete contact
        public void DeleteContact()
        {
            if (SelectedContact != null)
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
                        string delete_query = "DELETE FROM [dbo].[Contact] WHERE Contact_key = " + SelectedContact.Id + "";
                        connection.Open();
                        SqlCommand command = new SqlCommand(delete_query, connection);

                        command.ExecuteNonQuery();
                        Console.WriteLine(SelectedContact.Id);
                        connection.Close();
                    }
                    Contacts.Remove(Contacts.SingleOrDefault(i => i.Id == SelectedContact.Id)); // Refresh grid view after delete
                    SuccessDelete(); //Method called            
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }else
            {
                SelectRow();
            }
                
        }

        public void LoadPage() // method to show data in grid view
        {
            // Connection 
            // Here when navigating to the page reads data form contactlist table and display them
            try
            {
                var cb = new SqlConnectionStringBuilder();
                cb.DataSource = DBParameters.DataSource;
                cb.UserID = DBParameters.UserID;
                cb.Password = DBParameters.DBPass;
                cb.InitialCatalog = DBParameters.InitCatalog;

                using (var connection = new SqlConnection(cb.ConnectionString))
                {
                    string get_query = "SELECT * FROM [dbo].[Contact] WHERE Wedding_key = " + LandingPageViewModel.ActiveWedding + "";
                    connection.Open();
                    using (var command = new SqlCommand(get_query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Contacts = new ObservableCollection<Contact>(); // initialising Contacts
                            while (reader.Read())
                            {
                                try
                                {
                                    Console.WriteLine(reader.GetString(1));
                                    Console.WriteLine(reader.GetString(2));
                                    Console.WriteLine(reader.GetString(3));
                                    Contacts.Add(new Contact() { Id = reader.GetInt32(0), Name = reader.GetString(1), Phone = reader.GetString(2), Email = reader.GetString(3) });
                                }
                                catch (NullReferenceException ex)
                                {
                                    Console.WriteLine(ex);
                                }                             
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private async Task OnPopupCreateContact()
        {
            await Navigation.PushAsync(new CreatContactPage());
           
        }
        private async Task OnPopupUpdateContact()
        {
            if (SelectedContact != null) // this validate not selecting any row
            {
                EditContact = SelectedContact;
                await Navigation.PushAsync(new UpdateContactPage());
            }
            else
            {
                SelectRow();
            }

        }      
    }
}
