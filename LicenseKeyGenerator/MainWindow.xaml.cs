using LicenseKeyGenerator.Core;
using System;
using System.Windows;

namespace LicenseKeyGenerator
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Create_License_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string productKey = ProductKey.Text;
                string password = Password.Password;
                string expiryDateText = ExpiryDate.Text;
                if (string.IsNullOrEmpty(expiryDateText))
                    throw new Exception("Debe seleccionar una fecha");
                DateTime expiryDate = Convert.ToDateTime(expiryDateText);
                LicenseKeyManager.CreateKeyFile(productKey, expiryDate, password);
                MessageBox.Show("Archivo de licencia generado correctamente");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void New_Key_Click(object sender, RoutedEventArgs e)
        {
            ProductKey.Text = Guid.NewGuid().ToString().ToUpper();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ProductKey.Text = string.Empty;
            ExpiryDate.Text = string.Empty;
            Password.Password = string.Empty;
        }
    }
}
