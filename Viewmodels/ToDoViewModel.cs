using BrideChillaPOC.Models;
using BrideChillaPOC.ViewModels;
using BrideChillaPOC.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BrideChillaPOC.Viewmodels
{
    class ToDoViewModel : BaseViewModel
    {
        private ObservableCollection<Todo> _Todos;
        public ObservableCollection<Todo> Todos
        {
            get
            {
                return _Todos;
            }
            set
            {
                _Todos = value;
                OnPropertyChanged("Todos");
            }
        }

        private Todo _SelectedTodo;
        public Todo SelectedTodo
        {
            get
            {
                return _SelectedTodo;
            }
            set
            {
                _SelectedTodo = value;
                OnPropertyChanged("SelectedTodo");
            }
        }

        public ICommand ShowNewPopupCreate { get; set; }
        public ICommand ShowNewPopupModify { get; set; }
        public ICommand TodoSelected { get; set; }
        public ICommand SortTask { get; set; }
        public ICommand SortDate { get; set; }

        public bool SortedTaskAsc;
        public bool SortedDateAsc;

        public static Todo TodoToChange
        {
            get;
            set;
        }
        public int CurrentWedding = 1;
        //Initalize view model
        public ToDoViewModel(INavigation navigation) : base(navigation)
        {
            populateTable();
            ShowNewPopupCreate = new Command(async () => await showPopup());
            ShowNewPopupModify = new Command(async () => await showChange());
            SortTask = new Command(SortTableByTask);
            SortDate = new Command(SortTableByDate);
        }
        //Method to sort the todo table by current tasks completed.
        void SortTableByTask()
        {
            if (SortedTaskAsc)
            {
                SortedTaskAsc = false;
                Todos = new ObservableCollection<Todo>(Todos.OrderBy(x => x.TaskState));
            }
            else if (!SortedTaskAsc)
            {
                SortedTaskAsc = true;
                Todos = new ObservableCollection<Todo>(Todos.OrderByDescending(x => x.TaskState));
            }
        }

        //Sort by date and put any todos completed at the buttom, so the user only searches by date for tasks still to do.
        void SortTableByDate()
        {
            if (SortedDateAsc)
            {
                SortedDateAsc = false;
                Todos = new ObservableCollection<Todo>(Todos.OrderBy(x => x.Date).OrderBy(x => x.TaskState));
            }
            else if (!SortedDateAsc)
            {
                SortedDateAsc = true;
                Todos = new ObservableCollection<Todo>(Todos.OrderByDescending(x => x.Date).OrderBy(x => x.TaskState));
            }
        }
        //Navigation method to open ToDoPopup
        private async Task showPopup()
        {
            await Navigation.PushAsync(new ToDoPopupView());
            
        }   
        //Navigation method to open ToDoChange, checks if an item is selected and then updates a static variable
        //This is needed for ToDoChange to get the information about the item selected
        private async Task showChange()
        {
            if (SelectedTodo != null)
            {
                TodoToChange = SelectedTodo;
                await Navigation.PushAsync(new TodoChangeView());
            }
            else { ShowSelectAnItem(); }
        }

        void ShowSelectAnItem()
        {
            Application.Current.MainPage.DisplayAlert("Alert", "Please select a task to continue", "Ok");
        }

        //Populate the table with the required information from the ToDoList table in the database
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
                    string statement = "SELECT * FROM [dbo].[ToDoList] WHERE WeddingID = '"+ LoginPageViewModel.ActiveWedding + "'";
                    connection.Open();
                    using (var command = new SqlCommand(statement, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Todos = new ObservableCollection<Todo>();
                            while (reader.Read())
                            {
                                bool CurrentTaskState = ConvertToBool(reader.GetInt32(6));
                                DateTime TaskDate = reader.GetDateTime(5).Date;
                                Todos.Add(new Todo() { ID = reader.GetInt32(0), WeddingID = reader.GetInt32(1), Title = reader.GetString(2).Trim(), Description = reader.GetString(3).Trim(), Category = reader.GetString(4).Trim(), Date = TaskDate, TaskState = CurrentTaskState }); ;
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

        //Convert a given number of bit (0/1) to false or true
        //Used for the checkboxes to know if to be on or off.
        public bool ConvertToBool(int StateToConvert)
        {
            bool StateOf = false;
            if(StateToConvert == 0)
            {
                StateOf = false;
            }
            else if(StateToConvert == 1)
            {
                StateOf = true;
            }
            return StateOf;
        }
    }
}

