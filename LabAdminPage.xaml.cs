using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace LabSoftwareAdminSys
{
    /// <summary>
    /// LabAdminPage.xaml 的交互逻辑
    /// </summary>
    public partial class LabAdminPage : Page
    {
        private static List<string[]> SourceData;
        private static readonly ObservableCollection<string[]> DisplayData = new();
        private static readonly List<string[]> InsertData = new();
        private static readonly List<string[]> DeleteData = new();
        private static readonly List<string[]> UpdateData = new();
        public enum Op { insert, delete, update };
        private static readonly List<Op> history = new();

        public LabAdminPage()
        {
            InitializeComponent();
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
            InsertData.Clear();
            DeleteData.Clear();
            UpdateData.Clear();
            history.Clear();
        }

        public void PageDisplay()
        {
            SourceData = AdminSys.LabView();
            Clear();
            Synchronize();
            LabList.ItemsSource = DisplayData;
        }

        private void PageUpdate()
        {
            SourceData = AdminSys.LabView();
            string[] info = SearchInfo();
            Clear();
            foreach (string[] d in SourceData)
            {
                bool add = true;
                for(int i = 0; i < 6; ++i)
                {
                    if (!(d[i]??"").ToLower().Contains(info[i].ToLower()))
                        add = false;
                }
                if (add)
                    DisplayData.Add(d);
            }
        }

        private string[] SearchInfo()
        {
            string[] info = { Addr.Text, Lname.Text, Aname.Text, Cap.Text, Config.Text, Software.Text };
            return info;
        }

        private void AddNewRow_Click(object sender, RoutedEventArgs e)
        {
            string[] newrow = new string[6];
            DisplayData.Add(newrow);
            InsertData.Add(newrow);
            history.Add(Op.insert);
        }

        private void LabList_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            string[] newrow = e.Row.Item as string[];
            UpdateData.Add(newrow);
            history.Add(Op.update);
        }

        private void EditBack_Click(object sender, RoutedEventArgs e)
        {
            PageUpdate();
        }

        private void SaveEdit_Click(object sender, RoutedEventArgs e)
        {
            AdminSys.LabUpdate(InsertData, DeleteData, UpdateData, history);
            PageUpdate();
            Lasttime.Text = "最近保存时间：" + DateTime.Now.ToString();
        }

        private void AllChoose_Click(object sender, RoutedEventArgs e)
        {
            foreach(object item in LabList.Items)
            {
                DataGridTemplateColumn templeColumn = LabList.Columns[0] as DataGridTemplateColumn;
                FrameworkElement s = LabList.Columns[0].GetCellContent(item);
                CheckBox cb = templeColumn.CellTemplate.FindName("Choose", s) as CheckBox;
                cb.IsChecked = true;
            }
        }

        private void ChooseBack_Click(object sender, RoutedEventArgs e)
        {
            foreach (object item in LabList.Items)
            {
                DataGridTemplateColumn templeColumn = LabList.Columns[0] as DataGridTemplateColumn;
                FrameworkElement s = LabList.Columns[0].GetCellContent(item);
                CheckBox cb = templeColumn.CellTemplate.FindName("Choose", s) as CheckBox;
                cb.IsChecked = false;
            }
        }

        private void ChooseRev_Click(object sender, RoutedEventArgs e)
        {
            foreach (object item in LabList.Items)
            {
                DataGridTemplateColumn templeColumn = LabList.Columns[0] as DataGridTemplateColumn;
                FrameworkElement s = LabList.Columns[0].GetCellContent(item);
                CheckBox cb = templeColumn.CellTemplate.FindName("Choose", s) as CheckBox;
                cb.IsChecked = !cb.IsChecked;
            }
        }

        private void DelCheckRow_Click(object sender, RoutedEventArgs e)
        {
            Stack<int> idxs = new();
            foreach (object item in LabList.Items)
            {
                DataGridTemplateColumn templeColumn = LabList.Columns[0] as DataGridTemplateColumn;
                FrameworkElement s = LabList.Columns[0].GetCellContent(item);
                CheckBox cb = templeColumn.CellTemplate.FindName("Choose", s) as CheckBox;
                if(cb.IsChecked == true)
                {
                    int idx = LabList.Items.IndexOf(item);
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

        private void Addr_TextChanged(object sender, TextChangedEventArgs e)
        {
            PageUpdate();
        }

        private void Lname_TextChanged(object sender, TextChangedEventArgs e)
        {
            PageUpdate();
        }

        private void Aname_TextChanged(object sender, TextChangedEventArgs e)
        {
            PageUpdate();
        }

        private void Cap_TextChanged(object sender, TextChangedEventArgs e)
        {
            PageUpdate();
        }

        private void Config_TextChanged(object sender, TextChangedEventArgs e)
        {
            PageUpdate();
        }

        private void Software_TextChanged(object sender, TextChangedEventArgs e)
        {
            PageUpdate();
        }

        private void SoftList_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            string addr = b.Tag.ToString();
            LabSoftList labSoftList = new(addr);
            labSoftList.Show();
            labSoftList.Display();
        }
    }
}
