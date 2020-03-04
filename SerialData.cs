using System;
using System.Collections.Generic;
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
        public string DateAndTime
        {
            get { return _dateAndTime; }
            set { _dateAndTime = value; }
        }
        
        #endregion
    }
}
