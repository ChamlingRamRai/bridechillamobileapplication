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
    public class RolePageViewModel : BaseViewModel
    {
        public static int ActiveRole;
        // To Date page command
        public ICommand NextPageCommand { get; set; }
        public ICommand ShowAlertCommand { get; set; } // alert command for empty field
        public string RoleType { get; set; }

        public RolePageViewModel(INavigation navition) : base(navition)
        {
            NextPageCommand = new Command(OnNavToDatePage);
            ShowAlertCommand = new Command(get => MakeAlter()); // Alert
        }

        // For empty fields
        void MakeAlter()
        {
            Application.Current.MainPage.DisplayAlert("Alert", "Fields is empty", "ok");
        }

        // Method to navigate to Name page by saving Role in the DB
        public void OnNavToDatePage()
        {
            if (string.IsNullOrEmpty(RoleType))
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
                        string insert_query = "insert into [dbo].[Role] (Description) values ('" + RoleType + "')";
                        string select_query = "SELECT MAX(Role_key) FROM [dbo].[Role]"; // To select and get latest Role ID
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
                                    ActiveRole = reader.GetInt32(0); // Read table index 0
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                }
                            }
                        }
                        connection.Close();
                    }
                    Console.WriteLine(RoleType);
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                Navigation.PushAsync(new WeddingName()); // navigate to next page
            }
        }
       
    }
}