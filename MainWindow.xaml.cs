using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Wonder_Appliances
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //Set your custome Title            
            txtWindowTitle.Text = "COMPANY NAME";
            txtReferenceValue.Focus();
        }

        #region variable declarations

        private Microsoft.Office.Interop.Excel.Application excel;
        private Workbook workBook = null;
        private Worksheet workSheet = null;
        private Range cellRange = null;
        private List<SerialData> MyList = null;
        
        #endregion

        private void BtnGetData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtReferenceValue.Text.Trim()))
                {
                    MyList = new List<SerialData>();
                    for (int i = 1; i < 10; i++)
                    {
                        MyList.Add(new SerialData()
                        {
                            SrNo = i,
                            Readings = txtReferenceValue.Text.Trim(),
                            DateAndTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt")
                        });
                    }
                    grdSerialData.ItemsSource = MyList;
                }
                else
                {
                    MessageBox.Show("Please enter reference value ?", "Requesting", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtReferenceValue.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButton.OK, MessageBoxImage.Question);
            }
        }

        private void BtnExportData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MyList != null && MyList.Count > 0)
                {
                    GenerateExcel(MyList.ToDataTable());
                    workBook.SaveAs(Path.Combine(@"D:\Amol Important Code\Wonder Appliances\", "Test.xlsx"));
                    workBook.Close();
                    excel.Quit();
                    MessageBox.Show("Data Exported Successfully..!!", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Please get the data First ?", "Requesting", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtReferenceValue.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateExcel(System.Data.DataTable dt)
        {
            try
            {
                excel = new Microsoft.Office.Interop.Excel.Application
                {
                    DisplayAlerts = false,
                    Visible = false
                };
                workBook = excel.Workbooks.Add(Type.Missing);
                workSheet = (Worksheet)workBook.ActiveSheet;
                workSheet.Name = "ExportedData From USB";
                System.Data.DataTable tempDt = dt;

                grdSerialData.ItemsSource = tempDt.DefaultView;

                workSheet.Cells.Font.Size = 12;
                workSheet.Cells.Style.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                workSheet.Range[workSheet.Cells[1, 1], workSheet.Cells[1, 3]].Merge();
                workSheet.Range[workSheet.Cells[1, 1], workSheet.Cells[1, 4]].Font.Bold = true;
                workSheet.Cells[1, 1] = "Reference Value : " + txtReferenceValue.Text;

                int rowcount = 1;
                //Set Worksheet Coloum Headers.
                for (int i = 1; i <= tempDt.Columns.Count; i++)
                {
                    workSheet.Cells[2, i] = tempDt.Columns[i - 1].ColumnName;
                }

                //Set Worksheet each row data
                foreach (DataRow row in tempDt.Rows)
                {
                    rowcount += 1;
                    //Set worksheet each column data
                    for (int i = 0; i < tempDt.Columns.Count; i++)
                    {
                        workSheet.Cells[rowcount + 1, i + 1] = row[i].ToString();
                    }
                }
                cellRange = workSheet.Range[workSheet.Cells[1, 1], workSheet.Cells[rowcount, tempDt.Columns.Count]];
                cellRange.EntireColumn.AutoFit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DockPanel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App.Current.MainWindow.DragMove();
        }

        private void BdrMinimize_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App.Current.MainWindow.WindowState = WindowState.Minimized; 
        }

        private void BdrClose_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App.Current.MainWindow.Close();
        }
    }
    public static class ConvertListToDataTable
    {
        public static System.Data.DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            System.Data.DataTable dt = new System.Data.DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (T item in data)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyDescriptor pdt in properties)
                {
                    row[pdt.Name] = pdt.GetValue(item) ?? DBNull.Value;
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
    }    
}