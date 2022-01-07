using BrideChilla.Views;
using BrideChillaPOC.Models;
using BrideChillaPOC.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BrideChillaPOC.Viewmodels
{
    public class GuestListViewModel : BaseViewModel
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

            get { return _guest;  }
            set { _guest = value; }
        }

        //To get Name and Phone for update
        public static Guest EditGuest
        {
            get;
            set;
        }

        public ICommand ToAddGuestListPage { get; set; } // for Navigation to add Guest 
        public ICommand NavigateToEditPage { get; set; } // navigate to save page after edit
        public ICommand DeleteGuestCommand { get; set; } // Delete guest
        public ICommand DeletetSuccess { get; set; } // alert command for seccessful Delete
        public ICommand RowSelectionValidate { get; set; } // alert for row selection
        public ObservableCollection<Guest> Guests { get; set; }
        public ICommand BackToGuestlistHome { get; set; } // navigate to Guest list home page 
        public GuestListViewModel(INavigation navigation) : base(navigation)
        {
            ToAddGuestListPage = new Command(async () => await OnPopUpCreateGuest());
            NavigateToEditPage = new Command(async () => await OnPopUpUpdateGuest());
            LoadPage(); // method called
            DeletetSuccess = new Command(get => SuccessDelete());
            DeleteGuestCommand = new Command(DeleteGuest);
            RowSelectionValidate = new Command(get => SelectRow());
            BackToGuestlistHome = new Command(ToNavGuestlistHome);
        }

        //This navigate to Add Guest page
        private async Task OnPopUpCreateGuest()
        {
            await Navigation.PushAsync(new CreateGuestPage());

        }
        public void ToNavGuestlistHome()
        {
            Navigation.PushAsync(new GuestListHome());
        }
        void SelectRow() // alert for row selection
        {
            Application.Current.MainPage.DisplayAlert("Please select an item", "", "ok");
        }

        //This navigate to Guest Edit page
        private async Task OnPopUpUpdateGuest()
        {
            if (SelectedGuest != null)
            {
                EditGuest = SelectedGuest;
                await Navigation.PushAsync(new UpdateGuestPage());
            } else
            {
                SelectRow();
            }
        }
        // Delete Alert function
        void SuccessDelete()
        {
            Application.Current.MainPage.DisplayAlert("Your data is Deleted", "", "ok");
        }

        public void LoadPage() // method to show data in grid view
        {
            // Connection 
            // Here when navigating to the page reads data form guesttlist table and display them
            try
            {
                var cb = new SqlConnectionStringBuilder();
                cb.DataSource = DBParameters.DataSource;
                cb.UserID = DBParameters.UserID;
                cb.Password = DBParameters.DBPass;
                cb.InitialCatalog = DBParameters.InitCatalog;

                using (var connection = new SqlConnection(cb.ConnectionString))
                {
                    string get_query = "SELECT * FROM [dbo].[Guest] WHERE Wedding_key = " + LandingPageViewModel.ActiveWedding + "";
                    connection.Open();
                    using (var command = new SqlCommand(get_query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Guests = new ObservableCollection<Guest>(); // initialising Guests
                            while (reader.Read())
                            {
                                try
                                {
                                    Console.WriteLine(reader.GetString(1));
                                    Console.WriteLine(reader.GetString(2));
                                    Guests.Add(new Guest() { Id = reader.GetInt32(0), Name = reader.GetString(1), Phone = reader.GetString(2) });  
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

        //Delete Guest Function
        public void DeleteGuest()
        {
            if (SelectedGuest != null)
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
                        string delete_query = "DELETE FROM [dbo].[Guest] WHERE Guest_key = " + SelectedGuest.Id + "";
                        connection.Open();
                        SqlCommand command = new SqlCommand(delete_query, connection);

                        command.ExecuteNonQuery();
                        Console.WriteLine(SelectedGuest.Id);
                        connection.Close();
                    }
                    Guests.Remove(Guests.SingleOrDefault(i => i.Id == SelectedGuest.Id)); // Refresh grid view after delete
                    SuccessDelete(); //Method called
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            else
            {
                SelectRow();
            }
        }
    }
}
