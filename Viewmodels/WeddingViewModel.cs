using BrideChillaPOC.Models;
using BrideChillaPOC.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace BrideChillaPOC.Viewmodels
{
    public class WeddingViewModel : BaseViewModel
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private int _currencyid;
        public int CurrencyId
        {
            get { return _currencyid; }
            set { _currencyid = value; }
        }
        private DateTime _Date;
        public DateTime WeddingDate
        {
            get { return _Date; }
            set { _Date = value; }
        }

        private Currency _curid;
        public Currency SelectedCurrency
        {
            get { return _curid; }
            set { _curid = value; }
        }

        private Religion _regid;
        public Religion SelectReligion
        {
            get { return _regid; }
            set { _regid = value; }
        }
        public ICommand CreateWeddingCommand { get; set; } // Create wedding command
        public ObservableCollection<Currency> Currencies { get; set; } // for currencies
        public ObservableCollection<Religion> ReligionList { get; set; } // for religion
        public ICommand ShowAlertCommand { get; set; } // alert command for empty field
        public WeddingViewModel(INavigation navigation) : base(navigation)
        {
            WeddingDate = DateTime.Now;
            CreateWeddingCommand = new Command(CreateWedding);
            LoadPage();
            ReligionLoadPage();
            ShowAlertCommand = new Command(get => MakeAlert()); // Alert
        }
        // For empty fields
        void MakeAlert()
        {
            Application.Current.MainPage.DisplayAlert("Alert", "One of the fields is empty", "ok");
        }

        // creating wedding
        public void CreateWedding()
        {
            if (SelectedCurrency == null || SelectReligion == null)
            {
                Console.WriteLine("One of the fields is empty");
                MakeAlert(); // Alert method
            }
            else
            {
                try
                {
                    var cb = new SqlConnectionStringBuilder();
                    cb.DataSource = "bridechillapoc.database.windows.net";
                    cb.UserID = "BrideChillaPOCDBUser";
                    cb.Password = "A#$GWpqP$TSRj4qv";
                    cb.InitialCatalog = "BrideChilla_poc";

                    using (var connection = new SqlConnection(cb.ConnectionString))
                    {
                        string insert_query = "insert into [dbo].[Wedding] (Date, CurrencyId, Religion) values ('" + WeddingDate + "', '" + SelectedCurrency.ID + "', '" + SelectReligion.ReligionId + "')";
                        connection.Open();
                        SqlCommand command = new SqlCommand(insert_query, connection);
                        command.ExecuteNonQuery();
                        string select_query = "SELECT MAX(ID) FROM [dbo].[Wedding]";
                        using (var commandSelc = new SqlCommand(select_query, connection))
                        {
                            using (SqlDataReader reader = commandSelc.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    LoginPageViewModel.ActiveWedding = reader.GetInt32(0);
                                }
                            }
                        }
                        string update_query = "UPDATE [dbo].[User] set ActiveWedding ='" + LoginPageViewModel.ActiveWedding + "' where ID = '" + LoginPageViewModel.CurrentUserID + "'";
                        SqlCommand updateCommand = new SqlCommand(update_query, connection);
                        updateCommand.ExecuteNonQuery();
                        connection.Close();
                    }
                    Console.WriteLine("Done .....");

                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                Navigation.PushAsync(new LandingPage()); // navigate to Landing page
            }          
        }

        public void LoadPage() // method to load currency
        {
            // Connection 
            // Here when navigating to the page reads data form currency table and display them
            try
            {
                var cb = new SqlConnectionStringBuilder();
                cb.DataSource = "bridechillapoc.database.windows.net";
                cb.UserID = "BrideChillaPOCDBUser";
                cb.Password = "A#$GWpqP$TSRj4qv";
                cb.InitialCatalog = "BrideChilla_poc";

                using (var connection = new SqlConnection(cb.ConnectionString))
                {
                    string get_query = "SELECT * FROM [dbo].[Currency]";
                    connection.Open();
                    using (var command = new SqlCommand(get_query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Currencies = new ObservableCollection<Currency>(); // initialising Currencies
                            while (reader.Read())
                            {
                                try
                                {
                                    Console.WriteLine(reader.GetString(1));
                                    Currencies.Add(new Currency() { ID = reader.GetInt32(0), CurrencyType = reader.GetString(1) });
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
        public void ReligionLoadPage() // method to load religion
        {
            // Connection 
            // Here when navigating to the page reads data form religion table and display them
            try
            {
                var cb = new SqlConnectionStringBuilder();
                cb.DataSource = "bridechillapoc.database.windows.net";
                cb.UserID = "BrideChillaPOCDBUser";
                cb.Password = "A#$GWpqP$TSRj4qv";
                cb.InitialCatalog = "BrideChilla_poc";

                using (var connection = new SqlConnection(cb.ConnectionString))
                {
                    string get_query = "SELECT * FROM [dbo].[Religion]";
                    connection.Open();
                    using (var command = new SqlCommand(get_query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            ReligionList = new ObservableCollection<Religion>(); // initialising Religion
                            while (reader.Read())
                            {
                                try
                                {
                                    Console.WriteLine(reader.GetString(1));
                                    ReligionList.Add(new Religion() { ReligionId = reader.GetInt32(0), ReligionType = reader.GetString(1) });
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
    }
}
