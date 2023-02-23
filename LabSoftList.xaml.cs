using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LabSoftwareAdminSys
{
    /// <summary>
    /// LabSoftList.xaml 的交互逻辑
    /// </summary>
    public partial class LabSoftList : Window
    {
        private readonly string addr;
        private static List<string[]> SourceData;
        private static readonly ObservableCollection<string[]> DisplayData = new();
        private static readonly List<string[]> DeleteData = new();
        public enum Op { delete };
        private static readonly List<Op> history = new();

        public LabSoftList(string addr)
        {
            InitializeComponent();
            this.addr = addr;
        }

        private static void Synchronize()
        {
            foreach (string[] data in SourceData)
            {
                DisplayData.Add(data);
            }
        }

        private static void Clear()
        {
            DisplayData.Clear();
            DeleteData.Clear();
            history.Clear();
        }

        public void Display()
        {
            SourceData = AdminSys.LabSoftView(addr);
            Clear();
            Synchronize();
            SoftList.ItemsSource = DisplayData;
        }

        private void Update()
        {
            SourceData = AdminSys.LabSoftView(addr);
            Clear();
            Synchronize();
        }

        private void AllChoose_Click(object sender, RoutedEventArgs e)
        {
            foreach (object item in SoftList.Items)
            {
                DataGridTemplateColumn templeColumn = SoftList.Columns[0] as DataGridTemplateColumn;
                FrameworkElement s = SoftList.Columns[0].GetCellContent(item);
                CheckBox cb = templeColumn.CellTemplate.FindName("Choose", s) as CheckBox;
                cb.IsChecked = true;
            }
        }

        private void ChooseBack_Click(object sender, RoutedEventArgs e)
        {
            foreach (object item in SoftList.Items)
            {
                DataGridTemplateColumn templeColumn = SoftList.Columns[0] as DataGridTemplateColumn;
                FrameworkElement s = SoftList.Columns[0].GetCellContent(item);
                CheckBox cb = templeColumn.CellTemplate.FindName("Choose", s) as CheckBox;
                cb.IsChecked = false;
            }
        }

        private void ChooseRev_Click(object sender, RoutedEventArgs e)
        {
            foreach (object item in SoftList.Items)
            {
                DataGridTemplateColumn templeColumn = SoftList.Columns[0] as DataGridTemplateColumn;
                FrameworkElement s = SoftList.Columns[0].GetCellContent(item);
                CheckBox cb = templeColumn.CellTemplate.FindName("Choose", s) as CheckBox;
                cb.IsChecked = !cb.IsChecked;
            }
        }

        private void DelCheckRow_Click(object sender, RoutedEventArgs e)
        {
            Stack<int> idxs = new();
            foreach (object item in SoftList.Items)
            {
                DataGridTemplateColumn templeColumn = SoftList.Columns[0] as DataGridTemplateColumn;
                FrameworkElement s = SoftList.Columns[0].GetCellContent(item);
                CheckBox cb = templeColumn.CellTemplate.FindName("Choose", s) as CheckBox;
                if (cb.IsChecked == true)
                {
                    int idx = SoftList.Items.IndexOf(item);
                    idxs.Push(idx);
                    DeleteData.Add(DisplayData[idx]);
                    history.Add(Op.delete);
                }
            }
            foreach (int idx in idxs)
            {
                DisplayData.RemoveAt(idx);
            }
        }

        private void EditBack_Click(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void SaveEdit_Click(object sender, RoutedEventArgs e)
        {
            AdminSys.LabSoftUpdate(DeleteData, history);
            Update();
            Lasttime.Text = "最近保存时间：" + DateTime.Now.ToString();
        }
    }
}
