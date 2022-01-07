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
using BrideChillaPOC.Models;

namespace BrideChilla.Viewmodels
{
    public class DatePageViewModel : BaseViewModel
    {
        private DateTime _Date;
        public DateTime WeddingDate
        {
            get { return _Date; }
            set { _Date = value; }
        }
        //  navigation back to RolePage command
        public ICommand ToNamePageCommand { get; set; }

        //  navigation to TypePage and insert date
        public ICommand ToTypePageCommand { get; set; }


        public DatePageViewModel(INavigation navition) : base(navition)
        {
            WeddingDate = DateTime.Now; // show current date
            ToNamePageCommand = new Command(async () => await OnNavToNamePage());
            ToTypePageCommand = new Command( OnNavToTypePage);
        }
        // Method to navigate to NamePage
        private async Task OnNavToNamePage()
        {
            await Navigation.PushAsync(new NamePage());
        }

        // Method to navigate to TypePage and also save date at same time
        public void OnNavToTypePage()
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
                    string update_query = "UPDATE [dbo].[Wedding] SET Ringdate = '" + WeddingDate + "' WHERE Wedding_key = '"+ WeddingNameViewModel.ActiveWedding + "'";
                    connection.Open();
                    SqlCommand command = new SqlCommand(update_query, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                Console.WriteLine(WeddingDate);
                Console.WriteLine(WeddingNameViewModel.ActiveWedding);
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }
           
            Navigation.PushAsync(new TypePage()); // navigate to next page
        }


    }
}
