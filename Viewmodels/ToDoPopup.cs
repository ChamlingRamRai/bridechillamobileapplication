using BrideChillaPOC.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using BrideChillaPOC.Models;
using System.Linq;

namespace BrideChillaPOC.Viewmodels
{
    public class ToDoPopup : BaseViewModel
    {
        public ICommand SaveBtnCmd { get; set; }
        public ICommand DelBtnCommand { get; set; }

        public ToDoPopup(INavigation navigation) : base(navigation)
        {
            ToDoDate = DateTime.Now;
            SaveBtnCmd = new Command(InsertToDo);
            DelBtnCommand = new Command(async () => await CloseWindow());
        }

        public void SaveTodo()
        {
            Console.WriteLine(ToDoTitle);
            Console.WriteLine(ToDoDesc);
        }
        private async Task CloseWindow()
        {
            await Navigation.PopAsync();
        }
        private string _Title;
        public string ToDoTitle
        {
            get { return _Title; }
            set { _Title = value; }
        }
        private string _Description;
        public string ToDoDesc
        {
            get { return _Description; }
            set { _Description = value; }
        }
        private string _Cost;
        public string ToDoCost
        {
            get { return _Cost; }
            set { _Cost = value; }
        }
        private DateTime _Date;
        public DateTime ToDoDate
        {
            get { return _Date; }
            set { _Date = value; }
        }
        private string _Category;
        public string ToDoCategory
        {
            get { return _Category; }
            set { _Category = value; }
        }
        //Method to show an alert when the title is missing input
        void ShowEmptyAlert()
        {
            Application.Current.MainPage.DisplayAlert("Alert", "Title is empty", "ok");
        }
        //Method to show an alert when item is succesfully posted
        void ShowSuccessAlert()
        {
            Application.Current.MainPage.DisplayAlert("Congratulation", "Item posted to todo", "ok");
        }
        // Insert entries into a new item within the DB in ToDoList
        public void InsertToDo()
        {
            if (string.IsNullOrEmpty(ToDoCategory))
            {
                ToDoCategory = "Other";
            }
            if (string.IsNullOrEmpty(ToDoTitle))
            {
                Console.WriteLine("There needs to be a title for this todo");
                ShowEmptyAlert(); // Alert method
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

                    Console.WriteLine(ToDoTitle);
                    Console.WriteLine(ToDoDesc);
                    Console.WriteLine(ToDoCategory);
                    Console.WriteLine(ToDoDate.ToShortDateString());
                    using (var connection = new SqlConnection(cb.ConnectionString))
                    {
                        string insert_query = "insert into [dbo].[ToDoList] (WeddingID,Title,Description,Category,Date,TaskState) values ('" + LoginPageViewModel.ActiveWedding + "','" + ToDoTitle + "','" + ToDoDesc + "','" + ToDoCategory + "','" + ToDoDate + "','  0  ')";
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
        }
    }
}
