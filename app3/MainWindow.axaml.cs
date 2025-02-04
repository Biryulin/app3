using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using Tmds.DBus.Protocol;
using Npgsql;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using static app3.MainWindow;
using Npgsql.TypeMapping;

namespace app3
{
    public class Student
    {
        public Student(string n, string p)
        {
            Name = n;
            Password = p;
        }
        public string Name { get; set; }
        public string Password { get; set; }

        public string View()
        {
            return Name + " " + Password + " ���";
        }
    }
    public class User
    {

        public User(string n, string p)
        {
            Name = n;
            Password = p;
        }
        public string Name { get; set; }
        public string Password { get; set; }
    }

    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
        }
        public string connstr = "Host=localhost;Username=postgres;Password=admin;Database=04.02";

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var username = this.FindControl<TextBox>("Username").Text;
            var password = this.FindControl<TextBox>("password").Text;

            List<Student> students = new List<Student>();
            List<User> users = new List<User>();
            await using var dataSource = NpgsqlDataSource.Create(connstr);
            await using (var cmd = dataSource.CreateCommand("SELECT * FROM student"))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {

                    students.Add(new Student(reader.GetString(1), reader.GetInt32(2).ToString()));
                }
            }
            await using (var cmd = dataSource.CreateCommand("SELECT * FROM users"))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {

                    users.Add(new User(reader.GetString(0), reader.GetString(1)));
                }
            }

            if (IsValidUser(users, username, password))
            {              
                if (username == "admin")
                {
                    string mes = "";
                    foreach (var student in students)
                    {
                        mes += student.View() + "\n";
                    }
                    await ShowMessage(mes, "�����!");
                }
                else
                {
                    await ShowMessage("�� ����� ��� �������", "�����!");
                }
                
            }
            else
            {
                await ShowMessage("�������� ��� ������������ ��� ������.", "������");
            }
        }
        private bool IsValidUser(List<User> users, string username, string password)
        {
            foreach (var user in users)
            {
                if (username == user.Name && password == user.Password)
                {
                    return true;
                }
            }
            return false;
        }
        private async System.Threading.Tasks.Task ShowMessage(string message, string title)
        {
            var messageBox = new Window
            {
                Title = title,
                Width = 300,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            var textbb = new TextBlock { Text = message, Margin = new Thickness(10) };
            var closeButton = new Button { Content = "�������", Margin = new Thickness(10) };
            closeButton.Click += (s, e) => messageBox.Close();

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(textbb);
            stackPanel.Children.Add(closeButton);
            messageBox.Content = stackPanel;

            await messageBox.ShowDialog(this);
        }
    }
}