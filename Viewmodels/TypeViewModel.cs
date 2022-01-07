using BrideChilla.Views;
using BrideChillaPOC;
using BrideChillaPOC.Models;
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
    public class TypeViewModel : BaseViewModel
    {
        public static int ActiveReligion;
        public string WeddingType { get; set; }    
        //  navigate back to DatePage 
        public ICommand ToDatePageCommand { get; set; }

        // navigate to GoodToGoPage
        public ICommand ToCurrencyPageCommand { get; set; }
        public ICommand NavToGoodToGoPageCommand { get; set; }

        public ICommand ShowAlertCommand { get; set; } // alert command for empty field
        public TypeViewModel(INavigation navition) : base(navition)
        {
            ToDatePageCommand = new Command(async () => await OnNavToDatePage());
            ToCurrencyPageCommand = new Command(OnNavCurrencyPage);
            ShowAlertCommand = new Command(get => MakeAlter()); // Alert
            NavToGoodToGoPageCommand = new Command(NavToCurrencyPage);
        }
        
        public void NavToCurrencyPage()
        {
            Navigation.PushAsync(new CurrencyTypePage()); // navigate to next page
        }

        // Method to navigate back to Date page
        private async Task OnNavToDatePage()
        {
            await Navigation.PushAsync(new DatePage());
        }

        // For empty fields
        void MakeAlter()
        {
            Application.Current.MainPage.DisplayAlert("Alert", "Fields is empty", "ok");
        }

        // Method to navigate to GoodToGo page and also save religion same time
        public void OnNavCurrencyPage()
        {
            if (string.IsNullOrEmpty(WeddingType))
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
                        string insert_query = "insert into [dbo].[Religion] (Description) values ('" + WeddingType + "')";
                        string select_query = "SELECT MAX(Religion_key) FROM [dbo].[Religion]"; // To select and get latest Religion ID
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
                                    ActiveReligion = reader.GetInt32(0); // Read table index 0
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                }
                            }
                        }
                        connection.Close();
                    }
                    Console.WriteLine(WeddingType);
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                NavToCurrencyPage();
            }              
        }
    }
}
