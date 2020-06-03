using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LazerScannerUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            if (emailTextField.Text == "" || passwordTextField.Password == "")
            {
                Msgbox.Show("Please enter email, and password");
            }
            else
            {
                using (SqlConnection myConnection = new SqlConnection(Globals.SQL_DATA_CONNECTION))
                {
                    string oString = $"SELECT (SELECT UserID FROM Users WHERE email = '{emailTextField.Text}' AND password='{passwordTextField.Password}')";
                    SqlCommand oCmd = new SqlCommand(oString, myConnection);
                    myConnection.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            try
                            {
                                Globals.uid = (string)oReader.GetValue(0);
                                
                                //emailTextField.Text = Globals.uid;
                                emailTextField.Text = "";
                                passwordTextField.Password = "";
                                Frame.Navigate(typeof(ViewItemPage));
                            }
                            catch (Exception)
                            {
                                throw;//YOLO
                            }
                        }
                    }
                }
            }
        }

        private void CreateAcctButton_Click(object sender, RoutedEventArgs e)
        {
            string email = emailTextField.Text;
            string pwd = passwordTextField.Password;
            string userID = CreateAccountOnSQLServer(email, pwd);

            Globals.uid = userID;

            emailTextField.Text = string.Empty;
            passwordTextField.Password = string.Empty;
            Frame.Navigate(typeof(AddItemPage));

        }

        private void ForgotPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.uid = string.Empty;
            Msgbox.Show("Signed out, Goodbye!");
        }
        private string CreateAccountOnSQLServer(string email, string pwd)
        {
            using (SqlConnection myConnection = new SqlConnection(Globals.SQL_DATA_CONNECTION))
            {
                string oString = $"INSERT INTO Users(email,password)OUTPUT inserted.UserID VALUES('{email}','{pwd}')";
                SqlCommand oCmd = new SqlCommand(oString, myConnection);
                myConnection.Open();
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        return (string)oReader.GetValue(0);
                    }
                }
                return null;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            emailTextField.Focus(FocusState.Keyboard);
        }
    }
    public static class Msgbox
    {
        static public async void Show(string m)
        {
            var dialog = new MessageDialog(m);
            await dialog.ShowAsync();
        }
    }
}
