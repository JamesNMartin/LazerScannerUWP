using System;
using System.Collections.Generic;
using System.Data;
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
    public sealed partial class WelcomePage : Page
    {

        private string SQL_DATA_CONNECTION = "Data Source=tcp:73.118.249.57;Initial Catalog=LazerScanner;Persist Security Info=False;User ID=sa;Password=nothingtoseehere";


        public WelcomePage()
        {
            this.InitializeComponent();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            if (emailTextField.Text == "" || passwordTextField.Password == "")
            {
                Msgbox.Show("Please enter email, and password");
            }
        }
        
        private void CreateAcctButton_Click(object sender, RoutedEventArgs e)
        {
            string email = emailTextField.Text;
            string pwd = passwordTextField.Password;
            string userID = CreateAccountOnSQLServer(email, pwd);//TODO Save this ID somehow for the session for later when we need to ge the items
            
            emailTextField.Text = string.Empty;
            passwordTextField.Password = string.Empty;
        }

        private void ForgotPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            Msgbox.Show("Not ready");
        }
        private string CreateAccountOnSQLServer(string email, string pwd)
        {
            using (SqlConnection myConnection = new SqlConnection(SQL_DATA_CONNECTION))
            {
                string oString = $"INSERT INTO Users(email,password)OUTPUT inserted.UserID VALUES('{email}', '{pwd}')";
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
