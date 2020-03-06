using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Wonder_Appliances
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : System.Windows.Window
    {  
        public MainWindow()
        {
            InitializeComponent();            
            txtReferenceValue.Focus();                        

            port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            if (!this.port.IsOpen)
            {
                this.port.Open();
            }
        }

        #region variable declarations       
        private SerialPort port = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
        private Microsoft.Office.Interop.Excel.Application excel;
        private Workbook workBook = null;
        private Worksheet workSheet = null;
        private Range cellRange = null;
        private List<SerialData> MyList = null;

        #endregion

        #region private Methods

        private void BtnGetData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtReferenceValue.Text.Trim()))
                {
                    this.SetWindowSize();
                    // Show all the incoming data in the port's buffer                                     
                    //MessageBox.Show(this.port.ReadExisting());
                    MyList = new List<SerialData>();
                    for (int i = 1; i < 100; i++)
                    {
                        MyList.Add(new SerialData()
                        {
                            SrNo = i,
                            Readings = txtReferenceValue.Text.Trim(),
                            Date_And_Time = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt")
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

        void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
           // Show all the incoming data in the port's buffer
            MessageBox.Show(port.ReadExisting());
        }

        private void SetWindowSize()
        {
            if (App.Current.MainWindow.WindowState == WindowState.Normal)
            {
                this.grdSerialData.MaxHeight = 250;
            }
            else if (App.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                this.grdSerialData.MaxHeight = 850;
            }
        }       

        private void BtnExportData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MyList != null && MyList.Count > 0)
                {
                    GenerateExcel(MyList.ToDataTable());
                    workBook.SaveAs(Path.Combine(@"D:\Amol Important Code\March - 2020\Wonder Appliances\", "Test.xlsx"));
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
       
        private void TxtReferenceValue_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnGetData_Click(sender, e);
            }
        }

        private void TxtReferenceValue_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (this.txtReferenceValue.Text.Trim() == string.Empty)
            {
                this.grdSerialData.ItemsSource = null;
                this.MyList.Clear();
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            this.SetWindowSize();
        }

        #endregion
    }    
}