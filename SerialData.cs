using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wonder_Appliances
{
   public class SerialData
    {
        #region private properties

        private int _srNo;
        public int SrNo
        {
            get { return _srNo; }
            set { _srNo = value; }
        }
        private string _readings;
        public string Readings
        {
            get { return _readings; }
            set { _readings = value; }
        }
        private string _dateAndTime;
        public string Date_And_Time
        {
            get { return _dateAndTime; }
            set { _dateAndTime = value; }
        }
        
        #endregion
    }

   public static class ConvertListToDataTable
    {
        public static DataTable ToDataTable<T>(this IList<T> data)
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
