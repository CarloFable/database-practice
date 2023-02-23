using System.Text;
using System.Windows;
using System.Security.Cryptography;
using System.Globalization;


namespace LabSoftwareAdminSys
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly AdminSys labadmin = new();
        public MainWindow()
        {
            InitializeComponent();
        }

        private static string Sha256Password(string pwd)
        {
            byte[] p = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(pwd));
            string shapass = string.Empty;
            
            foreach (byte i in p)
            {
                shapass += i.ToString("x2", new CultureInfo("zh-cn"));
            }

            return shapass;
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            string id_value = id.Text;
            string pwd_value = password.Password;
            if(AdminSys.LoginCheck(id_value, Sha256Password(pwd_value)))
            {
                AdminSys.SetOnState(id_value);
                if(id_value[0] == 'A')
                {
                    AdminWindow admin = new(id_value);
                    admin.Show();
                    Close();
                }
            }
        }
    }
}
