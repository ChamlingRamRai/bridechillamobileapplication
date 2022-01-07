using BrideChillaPOC;
using BrideChillaPOC.Viewmodels;
using BrideChillaPOC.ViewModels;
using BrideChillaPOC.Views;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace BrideChilla.Viewmodels
{
    public class GuestListHomeViewModel : BaseViewModel
    {
        private Double countStart;
        public Double TotalGuest
        {
            get { return countStart; }
            set { countStart = value; }
        }
        // To Guest list page command
        public ICommand ToGuestListCommand { get; set; }
        // to Landing page
        public ICommand BackToLanding { get; set; } 
        public GuestListHomeViewModel(INavigation navigation) : base(navigation)
        {
            ToGuestListCommand = new Command(OnNavToGuestList);
            BackToLanding = new Command(OnNavToLandingPage);
            ShowTotalGuest();
        }

        // Method to navigate to guest list
        public void OnNavToGuestList()
        {
            Navigation.PushAsync(new GuestList());
        }


        // Method to navigate to landing page
        public void OnNavToLandingPage()
        {
            Navigation.PushAsync(new LandingPage());
        }

        public void ShowTotalGuest() // method to show total guests
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
                    string get_query = "SELECT COUNT(*) FROM Guest WHERE Wedding_key = " + LandingPageViewModel.ActiveWedding + "";
                    connection.Open();
                    using (var command = new SqlCommand(get_query, connection))
                    {
                       countStart = (Int32)command.ExecuteScalar();
                        Console.WriteLine("Total row count: " + countStart.ToString());
                    }                    
                    connection.Close();
                }
                Console.WriteLine(LandingPageViewModel.ActiveWedding);
            }
                catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
