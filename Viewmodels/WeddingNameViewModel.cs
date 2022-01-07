using BrideChilla.Views;
using BrideChillaPOC;
using BrideChillaPOC.Viewmodels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BrideChilla.Viewmodels
{
    public class WeddingNameViewModel : BaseViewModel
    {
        public static int ActiveWedding;
        public string WeddingName { get; set; }
        //  navigate back to RolePage 
        public ICommand ToRolePage { get; set; }

        // navigate to DatePage
        public ICommand ToDatePage { get; set; }

        public ICommand ShowAlertCommand { get; set; } // alert command for empty field
        public WeddingNameViewModel(INavigation navition) : base(navition)
        {
            ToRolePage = new Command(async () => await OnNavToRolePage());
            ToDatePage = new Command(OnNavToDatePage);
            ShowAlertCommand = new Command(get => MakeAlter()); // Alert
        }

        // Method to navigate back to Role page
        private async Task OnNavToRolePage()
        {
            await Navigation.PushAsync(new RolePage());
        }

        // For empty fields
        void MakeAlter()
        {
            Application.Current.MainPage.DisplayAlert("Alert", "Fields is empty", "ok");
        }

        // Method to navigate to Date page and also save wedding name 
        public void OnNavToDatePage()
        {
            if (string.IsNullOrEmpty(WeddingName))
            {
                Console.WriteLine("Fields is empty");
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
                        string insert_query = "insert into [dbo].[Wedding] (WeddingName) values ('" + WeddingName + "')";
                        string select_query = "SELECT MAX(Wedding_key) FROM [dbo].[Wedding]"; // To select and get latest user ID
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
                                    ActiveWedding = reader.GetInt32(0); // Read table index 0
                                }
                                catch (Exception ex)    
                                {
                                    Console.WriteLine(ex);
                                }
                            }
                        }

                        connection.Close();
                    }
                    Console.WriteLine(WeddingName);
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                Navigation.PushAsync(new DatePage()); // navigate to next page
            }
        }
    }
}
