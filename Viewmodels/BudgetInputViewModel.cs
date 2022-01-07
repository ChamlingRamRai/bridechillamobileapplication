using BrideChillaPOC.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace BrideChillaPOC.Viewmodels
{
    class BudgetInputViewModel : BaseViewModel
    {


        private string _Title;
        public string BudgetTitle
        {
            get { return _Title; }
            set { _Title = value; }
        }
        private string _Cost;
        public string BudgetCost
        {
            get { return _Cost; }
            set { _Cost = value; }
        }
        private string _Currency;
        public string BudgetCurrency
        {
            get { return _Currency; }
            set { _Currency = value; }
        }
        private string _Category;
        public string BudgetCategory
        {
            get { return _Category; }
            set { _Category = value; }
        }
        private Currency _curid;
        public Currency SelectedCurrency
        {
            get { return _curid; }
            set { _curid = value; }
        }
        public ObservableCollection<Currency> Currencies { get; set; } // for currencies
        public bool IsValidCur = false;
        public ICommand SaveBtnCmd { get; set; }

        public int curID = 0;
        public bool isChecked = false;
        public BudgetInputViewModel(INavigation navigation) : base(navigation)
        {
            FillCurrencyPicker();
            SaveBtnCmd = new Command(InsertBudget);
        }
        public void SaveBudget()
        {
            Console.WriteLine(BudgetTitle);
            Console.WriteLine(BudgetCost);
            Console.WriteLine(BudgetCurrency);
            Console.WriteLine(BudgetCategory);
            Console.WriteLine(curID);
        }

        //Method to show an alert when the title is missing input
        void ShowEmptyAlert()
        {
            Application.Current.MainPage.DisplayAlert("Alert", "One or more fields is missing input", "ok");
        }
        void ShowWrongCurrencyAlert()
        {
            Application.Current.MainPage.DisplayAlert("Alert", "Wrong currency type", "ok");
        }
        //Method to show an alert when item is succesfully posted
        void ShowSuccessAlert()
        {
            Application.Current.MainPage.DisplayAlert("Congratulation", "Budget is updated", "ok");
        }
        public void InsertBudget()
        {
            if (string.IsNullOrEmpty(BudgetCategory))
            {
                BudgetCategory = "Other";
            }
            if (string.IsNullOrEmpty(BudgetTitle) || string.IsNullOrEmpty(BudgetCost) || SelectedCurrency == null)
            {
                Console.WriteLine("One or more fields are missing input");
                isChecked = true;
                ShowEmptyAlert(); // Alert method
            }
            else
            {
                isChecked = false;
            }
            if (IsValidCur)
            {
                SaveBudget();
                try
                {
                    var cb = new SqlConnectionStringBuilder();
                    cb.DataSource = DBParameters.DataSource;
                    cb.UserID = DBParameters.UserID;
                    cb.Password = DBParameters.DBPass;
                    cb.InitialCatalog = DBParameters.InitCatalog;

                    using (var connection = new SqlConnection(cb.ConnectionString))
                    {
                        string insert_query = "insert into [dbo].[BudgetList] (Title,Category,Cost,WeddingId,CurrencyId) values ('" +  BudgetTitle + "','" + BudgetCategory + "','" + BudgetCost + "','" + LoginPageViewModel.ActiveWedding + "','" + SelectedCurrency.ID + "')";
                        connection.Open();
                        SqlCommand command = new SqlCommand(insert_query, connection);
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    //Console.WriteLine("Done .....");
                    ShowSuccessAlert();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            else if(!isChecked)
            {
                Console.WriteLine("Wrong currency type");
                ShowWrongCurrencyAlert(); // Alert method
            }

        }
        public void FillCurrencyPicker() // method to load currency
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
    }
}
