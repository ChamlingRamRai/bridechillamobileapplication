using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using BrideChillaPOC.ViewModels;
using System.Windows.Input;
using System.Threading.Tasks;
using BrideChillaPOC.Models;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using BrideChillaPOC.Views;
using BrideChilla.Views;

namespace BrideChillaPOC.Viewmodels
{
    class LandingPageViewModel : BaseViewModel
    {
        private Double _Budget;
        public Double TotalBudget
        {
            get { return _Budget; }
            set { _Budget = value; }
        }
        public int WeddingID;
        public DateTime CurrentDate;
        public static int CurrencyID;
        private string _CurrencyType;
        public static int ActiveWedding;
        public int FinishedTodos_;
        public int FinishedTodos
        {
            get { return FinishedTodos_; }
            set { FinishedTodos_ = value; }
        }
        public int TotalTodos_;
        public int TotalTodos
        {
            get { return TotalTodos_; }
            set { TotalTodos_ = value; }
        }

        public double TodoProgress_;
        public double TodoProgress
        {
            get { return TodoProgress_; }
            set { TodoProgress_ = value; }
        }

        public string CurrencyType
        {
            get { return _CurrencyType; }
            set { _CurrencyType = value; }
        }
        private int _DaysBeforeWedding;
        public int TotalDaysBeforeWedding
        {
            get { return _DaysBeforeWedding; }
            set { _DaysBeforeWedding = value; }
        }
        public ObservableCollection<BudgetModel> Budgets { get; set; }
        public ICommand CreateContactCommand { get; set; }
        public ICommand GuestListClicked { get; set; }
        public ICommand ToDoBtnClicked { get; set; }
        public ICommand BudgetClicked { get; set; }
        public ICommand NavitageToCreateWedding { get; set; }
        public LandingPageViewModel(INavigation navigation) : base(navigation)
        {
            GetRoleData();
            CurrentDate = DateTime.Now.Date;
            CalculateTotalBudget();
            GetWeddingData();
            GetAllFinishedTodo();
            CountTodos();
            SetProgressBar();

            ToDoBtnClicked = new Command(async () => await GotoToDoPage());

            BudgetClicked = new Command(async () => await GotoBudgetPage());

            CreateContactCommand = new Command(async () => await OnContactListPage());// for Contact List view

            GuestListClicked = new Command(async () => await GoToGuestListPage());// for Guest List view

            NavitageToCreateWedding = new Command(async () => await GoToWeddingPage());


        }

        private async Task GotoToDoPage()
        {
            await Navigation.PushAsync(new ToDoView());

        }
        private async Task GotoBudgetPage()
        {
            await Navigation.PushAsync(new BudgetListView());
        }
        private async Task GoToWeddingPage()
        {
            await Navigation.PushAsync(new WeddingView());
        }

        public void GetWeddingData()
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
                    string statement = "SELECT * FROM [dbo].[Wedding] WHERE Wedding_key = " + ActiveWedding;
                    connection.Open();
                    using (var command = new SqlCommand(statement, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DateTime WeddingDate = reader.GetDateTime(2);
                                TimeSpan daysBeforeWedding = WeddingDate - CurrentDate;
                                TotalDaysBeforeWedding = daysBeforeWedding.Days;
                                CurrencyID = reader.GetInt32(4);                            
                            }
                        }
                    }
                    connection.Close();
                    GetCurrencyType();
                }
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
                    string statement = "SELECT * FROM [dbo].[Currency] WHERE Currency_key = " + CurrencyID;
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
                    string statement = "SELECT [dbo].[TotalCostOfWeddingPlan]("+ ActiveWedding + ")";
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
                //Console.WriteLine("Done .....");
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        // Method to navigate to Contact List page
        private async Task OnContactListPage()
        {
            await Navigation.PushAsync(new ContactList());
        }

        // Method to navigate to Guest List page
        private async Task GoToGuestListPage()
        {
            await Navigation.PushAsync(new GuestListHome());
        }

        // Method to load User Role data for showing data in the grid view
        public void GetRoleData()
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
                    string statement = "SELECT * FROM [dbo].[UserRole] WHERE User_key = " + LoginPageViewModel.CurrentUserID;
                    connection.Open();
                    using (var command = new SqlCommand(statement, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ActiveWedding = reader.GetInt32(2);
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
        public void GetAllFinishedTodo()
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
                    string statement = "SELECT COUNT(*) FROM [dbo].[ToDo] WHERE WeddingPlan_key = " + ActiveWedding + " AND Completed = 1";
                    connection.Open();
                    using (var command = new SqlCommand(statement, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                FinishedTodos = reader.GetInt32(0);
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
        public void CountTodos()
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
                    string statement = "SELECT COUNT(*) FROM [dbo].[ToDo] WHERE WeddingPlan_key = " + ActiveWedding ;
                    connection.Open();
                    using (var command = new SqlCommand(statement, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TotalTodos = reader.GetInt32(0);
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
        public void SetProgressBar()
        {
            TodoProgress = (double)FinishedTodos / (double)TotalTodos;
        }
    }
}
