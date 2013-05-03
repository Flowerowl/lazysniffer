using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpPcap.AirPcap;
using SharpPcap.LibPcap;
using SharpPcap.WinPcap;
using SharpPcap;
namespace Lazy_Sniffer
{
    class ClsComboboxItem
    {
        private string sText;
        public string Text
        {
            get { return sText; }
            set { sText = value; }
        }
        private ICaptureDevice sValue;
        public ICaptureDevice Value
        {
            get { return sValue; }
            set { sValue = value; }
        }
    }
}
