using System;
using System.Windows;

namespace LabSoftwareAdminSys
{
    /// <summary>
    /// AdminWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AdminWindow : Window
    {
        private readonly string id;
        private static readonly SoftAdminPage sa = new();
        private static readonly LabAdminPage la = new();
        private static readonly CourseAdminPage ca = new();
        private static readonly UserAdminPage ua = new();

        public AdminWindow(string id)
        {
            InitializeComponent();
            id_display.Text = "实验室管理员" + id;
            this.id = id;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            AdminSys.Logout(id);
            MainWindow main = new();
            main.Show();
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            AdminSys.Logout(id);
        }

        private void SoftAdmin_Click(object sender, RoutedEventArgs e)
        {
            admin_display.Text = "本次操作：软件管理";
            AdminPage.Content = sa;
            sa.PageDisplay();
        }

        private void LabAdmin_Click(object sender, RoutedEventArgs e)
        {
            admin_display.Text = "本次操作：实验室管理";
            AdminPage.Content = la;
            la.PageDisplay();
        }

        private void CourseAdmin_Click(object sender, RoutedEventArgs e)
        {
            admin_display.Text = "本次操作：课程管理";
            AdminPage.Content = ca;
        }

        private void UserAdmin_Click(object sender, RoutedEventArgs e)
        {
            admin_display.Text = "本次操作：人员管理";
            AdminPage.Content = ua;
        }
    }
}
