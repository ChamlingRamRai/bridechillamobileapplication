using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using BrideChillaPOC.Views;
using BrideChillaPOC.Models;
using System.Collections.ObjectModel;
using System.Data.SqlClient;

namespace BrideChillaPOC.Viewmodels
{
    class BudgetListViewModel : BaseViewModel
    {
        private Double _Budget;
        public Double TotalBudget
        {
            get { return _Budget; }
            set { _Budget = value; }
        }
        private string _CurrencyType;
        public string CurrencyType
        {
            get { return _CurrencyType; }
            set { _CurrencyType = value; }
        }
        public ObservableCollection<BudgetModel> Budgets { get; set; }
        public ICommand ShowNewViewCreate { get; set; }
        public BudgetListViewModel(INavigation navigation) : base(navigation)
        {
            populateTable();
            CalculateTotalBudget();
            ShowNewViewCreate = new Command(async () => await showInputView());
        }
        //Push to create a new budget item
        private async Task showInputView()
        {
            await Navigation.PushAsync(new BudgetInputView());
        }

        //Populate the table with the required information from the BudgetList table in the database
        public void populateTable()
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
                    string statement = "SELECT * FROM [dbo].[BudgetList] WHERE WeddingId = "+ LoginPageViewModel.ActiveWedding +"";
                    connection.Open();
                    using (var command = new SqlCommand(statement, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Budgets = new ObservableCollection<BudgetModel>();
                            while (reader.Read())
                            {
                                Budgets.Add(new BudgetModel() { BudgetId = reader.GetInt32(0), Title = reader.GetString(1).Trim(), Category = reader.GetString(2).Trim(), Cost = reader.GetInt32(3), WeddingId = reader.GetInt32(4), CurrencyId = reader.GetInt32(5), CurrencyName = ConvertToCurString(reader.GetInt32(5)) }); ;
                            }
                        }
                    }
                    connection.Close();
                }
                GetCurrencyType();
                //Console.WriteLine("Done .....");
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public void GetCurrencyType()
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
                    string statement = "SELECT * FROM [dbo].[Currency] WHERE Id = " + LandingPageViewModel.CurrencyID;
                    connection.Open();
                    using (var command = new SqlCommand(statement, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CurrencyType = reader.GetString(1);
                                Console.WriteLine("Type " + reader.GetString(1));
                            }
                        }
                    }
                    connection.Close();
                }
                //Console.WriteLine("Done .....");
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //Run stored procedure from the database, to calculate total budget for the wedding.
        public void CalculateTotalBudget()
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
                    string statement = "SELECT [dbo].[TotalCostInWeddingCurrency]("+LoginPageViewModel.ActiveWedding+")";
                    connection.Open();
                    using (var command = new SqlCommand(statement, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TotalBudget = Convert.ToDouble(reader.GetValue(0));
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
        public string ConvertToCurString(int currencyID)
        {
            string CurrencyName = "";
            switch (currencyID)
            {
                case 1:
                    CurrencyName = "EUR";
                    break;
                case 2:
                    CurrencyName = "DKK";
                    break;
                case 3:
                    CurrencyName = "USD";
                    break;
                case 4:
                    CurrencyName = "PLN";
                    break;
                default:
                    CurrencyName = "--";
                    break;

            }
            Console.WriteLine(CurrencyName);
            Console.WriteLine(currencyID);
            return CurrencyName;
        }
    }
}
