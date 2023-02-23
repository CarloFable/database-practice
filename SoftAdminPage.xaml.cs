using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace LabSoftwareAdminSys
{
    /// <summary>
    /// SoftAdminPage.xaml 的交互逻辑
    /// </summary>
    public partial class SoftAdminPage : Page
    {
        private static List<string[]> SourceData;
        private static readonly ObservableCollection<string[]> DisplayData = new();
        private static readonly List<string[]> InsertData = new();
        private static readonly List<string[]> DeleteData = new();
        private static readonly List<string[]> UpdateData = new();
        public enum Op { insert, delete, update };
        private static readonly List<Op> history = new();
        private static int id;

        public SoftAdminPage()
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
            SourceData = AdminSys.SoftView();
            Clear();
            Synchronize();
            SoftList.ItemsSource = DisplayData;
            if (SourceData.Count > 0)
                id = Convert.ToInt32(SourceData[^1][0]);
        }

        private void PageUpdate()
        {
            SourceData = AdminSys.SoftView(SearchInfo());
            Clear();
            Synchronize();
            List<string[]> tmp = AdminSys.SoftView();
            if (tmp.Count > 0)
                id = Convert.ToInt32(tmp[^1][0]);
        }

        private string[] SearchInfo()
        {
            string[] info = new string[8];
            info[4] = string.Empty;
            info[6] = Sname.Text;
            info[1] = Scate.Text;
            info[7] = Sver.Text;
            info[0] = Sneed.Text;
            info[2] = Sconf.Text;
            info[3] = Senv.Text;
            info[5] = Sintro.Text;
            return info;
        }

        private void Sname_TextChanged(object sender, TextChangedEventArgs e)
        {
            PageUpdate();
        }

        private void Scate_TextChanged(object sender, TextChangedEventArgs e)
        {
            PageUpdate();
        }

        private void Sver_TextChanged(object sender, TextChangedEventArgs e)
        {
            PageUpdate();
        }

        private void Sneed_TextChanged(object sender, TextChangedEventArgs e)
        {
            PageUpdate();
        }

        private void Sconf_TextChanged(object sender, TextChangedEventArgs e)
        {
            PageUpdate();
        }

        private void Senv_TextChanged(object sender, TextChangedEventArgs e)
        {
            PageUpdate();
        }

        private void Sintro_TextChanged(object sender, TextChangedEventArgs e)
        {
            PageUpdate();
        }

        private void AddNewRow_Click(object sender, RoutedEventArgs e)
        {
            string[] newrow = new string[8];
            newrow[0] = (++id).ToString();
            DisplayData.Add(newrow);
            InsertData.Add(newrow);
            history.Add(Op.insert);
        }

        private void SoftList_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
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
            AdminSys.SoftUpdate(InsertData, DeleteData, UpdateData, history);
            PageUpdate();
            Lasttime.Text = "最近保存时间：" + DateTime.Now.ToString() + " " + SaveEdit.Content;
        }

        private void AllChoose_Click(object sender, RoutedEventArgs e)
        {
            foreach(object item in SoftList.Items)
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
                if(cb.IsChecked == true)
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

        private void Add2Lab_Click(object sender, RoutedEventArgs e)
        {
            AdminSys.SoftUpdate(InsertData, DeleteData, UpdateData, history);
            foreach (object item in SoftList.Items)
            {
                DataGridTemplateColumn templeColumn = SoftList.Columns[0] as DataGridTemplateColumn;
                FrameworkElement s = SoftList.Columns[0].GetCellContent(item);
                CheckBox cb = templeColumn.CellTemplate.FindName("Choose", s) as CheckBox;
                if (cb.IsChecked == true)
                {
                    int idx = SoftList.Items.IndexOf(item);
                    AdminSys.AddLabSoftware(DisplayData[idx][0], LabAddr.Text);
                }
            }
            PageUpdate();
            Lasttime.Text = "最近保存时间：" + DateTime.Now.ToString() + " " + Add2Lab.Content;
        }

        private void Add2Course_Click(object sender, RoutedEventArgs e)
        {
            AdminSys.SoftUpdate(InsertData, DeleteData, UpdateData, history);
            foreach (object item in SoftList.Items)
            {
                DataGridTemplateColumn templeColumn = SoftList.Columns[0] as DataGridTemplateColumn;
                FrameworkElement s = SoftList.Columns[0].GetCellContent(item);
                CheckBox cb = templeColumn.CellTemplate.FindName("Choose", s) as CheckBox;
                if (cb.IsChecked == true)
                {
                    int idx = SoftList.Items.IndexOf(item);
                    AdminSys.AddCourseSoftware(DisplayData[idx][0], Cname.Text);
                }
            }
            PageUpdate();
            Lasttime.Text = "最近保存时间：" + DateTime.Now.ToString() + " " + Add2Course.Content;
        }

        private void LabAddr_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(LabAddr.Text.Length == 0)
            {
                LabAddr.ItemsSource = null;
            }
            else if(LabAddr.Text.Length > 0)
            {
                Stack<string> result = AdminSys.SearchLab(LabAddr.Text);
                LabAddr.ItemsSource = result;
                LabAddr.IsDropDownOpen = true;
            }
        }

        private void Cname_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Cname.Text.Length == 0)
            {
                Cname.ItemsSource = null;
            }
            else if (Cname.Text.Length > 0)
            {
                Stack<string> result = AdminSys.SearchCourse(Cname.Text);
                Cname.ItemsSource = result;
                Cname.IsDropDownOpen = true;
            }
        }

        private void LabAddr_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LabAddr.IsDropDownOpen = true;
        }

        private void Cname_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Cname.IsDropDownOpen = true;
        }
    }
}
