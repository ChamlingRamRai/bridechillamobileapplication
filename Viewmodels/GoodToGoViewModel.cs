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
    public class GoodToGoViewModel : BaseViewModel
    {
        // navigate to home page page command
        public ICommand BackToHomeCommand { get; set; }
        public GoodToGoViewModel(INavigation navition) : base(navition)
        {
            BackToHomeCommand = new Command(async () => await OnNavToHomePage());
            SetReligionKey();
            SetKeysIntoUserRoleTable();
        }

        // Method to navigate to Home (Splash) page
        private async Task OnNavToHomePage()
        {
            await Navigation.PushAsync(new SplashPage());
        }

        // Method to to set Religion key into Wedding table
        public void SetReligionKey()
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
                    string update_query = "UPDATE [dbo].[Wedding] SET Religion_key = '" + TypeViewModel.ActiveReligion + "' WHERE Wedding_key = '" + WeddingNameViewModel.ActiveWedding + "'";
                    connection.Open();
                    SqlCommand command = new SqlCommand(update_query, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                Console.WriteLine(TypeViewModel.ActiveReligion);            
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        // Method to to set different current keys into User Role table
        public void SetKeysIntoUserRoleTable()
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
                    string update_query = "insert into [dbo].[UserRole] (User_key,Role_key,Wedding_key) values ('" + UserViewModel.ActiveUser + "', '" + RolePageViewModel.ActiveRole + "', '" + WeddingNameViewModel.ActiveWedding + "')";
                    connection.Open();
                    SqlCommand command = new SqlCommand(update_query, connection);  
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                Console.WriteLine("Data inserted");
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
