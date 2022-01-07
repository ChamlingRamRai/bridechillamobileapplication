using BrideChilla.Views;
using BrideChillaPOC;
using BrideChillaPOC.Models;
using BrideChillaPOC.Viewmodels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace BrideChilla.Viewmodels
{
    public class CurrencyViewModel : BaseViewModel
    {
        public static int CurrencyID;

        private string _currencyType;
        public string CurrencyType
        {
            get { return _currencyType; }
            set { _currencyType = value; }
        }

        private Currency _selectCurrency;
        public Currency SelectCurrency
        {
            get { return _selectCurrency; }
            set { _selectCurrency = value; }
        }
        private int _selectId;
        public int ID
        {
            get { return _selectId; }
            set { _selectId = value; }
        }
        public ICommand ToTypePageCommand { get; set; } // to previous page
        public ICommand ToNextPageCommand { get; set; } // to next page
        public ICommand ShowAlertCommand { get; set; } // alert command for empty field
        public List<Currency> CurrencyList { get; set; }
        public CurrencyViewModel(INavigation navition) : base(navition)
        {
            CurrencyList = GetCurrency().OrderBy(t => t.CurrencyType).ToList();
            ToNextPageCommand = new Command(OnToNextPage);
            ShowAlertCommand = new Command(get => MakeAlter()); // Alert
            ToTypePageCommand = new Command(OnNavToPreviousPage);
        }
        public List<Currency> GetCurrency()
        {
            List<Currency> currencies = new List<Currency>()

            {
                new Currency(){ID = 1, CurrencyType="EUR"},
                new Currency(){ID = 2, CurrencyType="DKK"},
                new Currency(){ID = 3, CurrencyType="USD"},
                new Currency(){ID = 4, CurrencyType="PLN"}
            };
            return currencies;
        }

        // For empty fields
        void MakeAlter()
        {
            Application.Current.MainPage.DisplayAlert("Alert", "Fields is empty", "ok");
        }

        // method to navigate prevous page
        public void OnNavToPreviousPage()
        {
            Navigation.PushAsync(new TypePage());
        }
        // Method to navigate to GoodToGo page and also set currency key
        public void OnToNextPage()
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
                    string update_query = "UPDATE [dbo].[Wedding] SET Currency_key = '" + SelectCurrency.ID + "' WHERE Wedding_key = '" + WeddingNameViewModel.ActiveWedding + "'";
                    connection.Open();
                    SqlCommand command = new SqlCommand(update_query, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                    }
                     Console.WriteLine(SelectCurrency);
                     Console.WriteLine(SelectCurrency.ID);
                     Console.WriteLine(WeddingNameViewModel.ActiveWedding);
            }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.ToString());          
                }
               Navigation.PushAsync(new GoodToGoPage());           
        }
    }
}
