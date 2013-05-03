using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharpPcap;
using PacketDotNet;
using SharpPcap.AirPcap;
using SharpPcap.LibPcap;
using SharpPcap.WinPcap;
using System.Threading;
namespace Lazy_Sniffer
{
    public partial class Form1 : Form
    {
        BackgroundWorker bgworker = new BackgroundWorker();
        private delegate void _CrossSetDataTable(Int64 iSeqe,DateTime dtTime,string sSource,ushort sSourcePort,string sDestination,ushort sDesPort,string sProtocaol);
        bool isRun = false;
        PackageCache clsCache = new PackageCache();
        object objLock = new object();
        Int64 iCount;
        DataTable dtPackage = new DataTable();
        struct StrPackakge
        {
            public DateTime Time;
            public ushort SourcePort;
            public ushort DesPort;
            public int Length;
            public Packet Packet;
            public LazyIpPacket ipPacket;
            public LazyTcpPacket tcpPacket;
            public LazyUdpPacket udpPacket;
            public string Protocols;
        }
        Dictionary<Int64, StrPackakge> lstPackage = new Dictionary<Int64, StrPackakge>();
        public Form1()
        {
            InitializeComponent();
            dtPackage.Columns.Add("序号",typeof(Int64));
            dtPackage.Columns.Add("时间",typeof(DateTime));
            dtPackage.Columns.Add("源地址");
            dtPackage.Columns.Add("源端口");
            dtPackage.Columns.Add("目的地址");
            dtPackage.Columns.Add("目的端口");
            dtPackage.Columns.Add("协议");
            dataGridView1.DataSource=dtPackage;
        }
        private void CrossSetDataTable(Int64 iSqe, DateTime dtTime, string sSource, ushort sSourcePort,string sDestination, ushort sDesPort, string sProtocol)
        {
            _CrossSetDataTable SetText = delegate(Int64 Sqe, DateTime Time, string Source, ushort SourcePort, string Destination, ushort DesPort, string Protocol)
            {
                this.dtPackage.Rows.Add(new object[] { Sqe, Time, Source, SourcePort, Destination, DesPort, Protocol });
            };
            this.Invoke(SetText,new object[]{iSqe,dtTime,sSource,sSourcePort,sDestination,sDesPort,sProtocol});
        }
        private void Init()
        {
            iCount = 0;
            lstPackage.Clear();
            dtPackage.Rows.Clear();
            packageDataTxt.Text = string.Empty;
            hexEditor.LoadFromFile(new byte[0]);
            
            
        }
        private void EnableControl(bool state)
        {
            devicesComboBox.Enabled = !isRun;
            filterComboBox.Enabled = !isRun;
            stopBtn.Enabled = isRun;
            startBtn.Enabled = !isRun;
        }
        //初始化，寻找网络设备
        private void Form1_Load(object sender, EventArgs e)
        {
            devicesComboBox.ComboBox.DisplayMember = "Text";
            devicesComboBox.ComboBox.ValueMember = "Value";
            var devices = CaptureDeviceList.Instance;
            foreach (ICaptureDevice dev in devices)
            {
                if (dev is AirPcapDevice)
                {
                    Console.WriteLine(dev.ToString());
                }
                else if (dev is WinPcapDevice)
                {
                    ClsComboboxItem clsCbxItem = new ClsComboboxItem();
                    clsCbxItem.Text = ((WinPcapDevice)dev).Interface.FriendlyName + " " + dev.Description;
                    clsCbxItem.Value = dev;
                    devicesComboBox.ComboBox.Items.Add(clsCbxItem);
                }
                else if (dev is LibPcapLiveDevice)
                {
                    Console.WriteLine(dev.ToString());
                }
                
            }
            filterComboBox.Items.Add("tcp and ip");
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            if (devicesComboBox.SelectedItem != null)
            {
                Init();
                ICaptureDevice Dev = ((ClsComboboxItem)devicesComboBox.SelectedItem).Value;
                Dev.OnPacketArrival += new PacketArrivalEventHandler(Dev_OnPacketArrival);
                Dev.Open();
                Dev.Filter = filterComboBox.SelectedItem.ToString();
                Dev.StartCapture();
                isRun = true;
                bgworker.RunWorkerAsync();
                EnableControl(true);
            }
        }

        private void stopBtn_Click(object sender, EventArgs e)
        {
                ICaptureDevice Device = ((ClsComboboxItem)devicesComboBox.SelectedItem).Value;
                Device.StartCapture();
                Device.Close();
                isRun = false;
                stopBtn.Enabled = false;
        }
        void Dev_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            lock (objLock)
            {
                clsCache.AddItem(e.Packet);
            }
        }
        private void bgworker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (isRun || clsCache.GetCount() > 0)
            {
                PackageAna();
                Thread.Sleep(50);
            }
        }
        private void bgworker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            EnableControl(false);
        }
        void PackageAna()
        {
            RawCapture rawCapture = clsCache.GetAndRemoveFirst();
            if (rawCapture == null)
            {
                return;
            }
            iCount++;
            StrPackakge strPackage = new StrPackakge();
            strPackage.Time = rawCapture.Timeval.Date.ToLocalTime();
            strPackage.Length = rawCapture.Data.Length;
            PacketDotNet.Packet tempPacket = PacketDotNet.Packet.ParsePacket(rawCapture.LinkLayerType,rawCapture.Data);
            strPackage.Packet = tempPacket;
            IpPacket ipPack = PacketDotNet.IpPacket.GetEncapsulated(tempPacket);
            if (ipPack != null)
            {
                strPackage.Protocols += ipPack.Protocol.ToString();
                strPackage.ipPacket = new LazyIpPacket(ipPack);
                if (ipPack.Protocol.ToString() == "TCP")
                {
                    PacketDotNet.TcpPacket tcppack = TcpPacket.GetEncapsulated(tempPacket);
                    if (tcppack != null)
                    {
                        strPackage.tcpPacket = new LazyTcpPacket(tcppack);
                        strPackage.DesPort = tcppack.DestinationPort;
                    }
                }
                else if (ipPack.Protocol.ToString() == "UDP")
                {
                    PacketDotNet.UdpPacket udppack = UdpPacket.GetEncapsulated(tempPacket);
                    if (udppack != null)
                    {
                        strPackage.udpPacket = new LazyUdpPacket(udppack);
                        strPackage.SourcePort = udppack.SourcePort;
                        strPackage.DesPort = udppack.DestinationPort;
                    }
                }
                CrossSetDataTable(iCount,strPackage.Time,ipPack.SourceAddress.ToString(),strPackage.SourcePort,ipPack.DestinationAddress.ToString(),strPackage.DesPort,ipPack.Protocol.ToString());
                lstPackage.Add(iCount,strPackage);
            }

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Init();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell != null)
            {
                StrPackakge sp;
                lstPackage.TryGetValue((Int64)dataGridView1[0, dataGridView1.CurrentCell.RowIndex].Value, out sp);
                
                if (sp.Packet != null)
                {
                    hexEditor.LoadFromFile(sp.Packet.Bytes);
                    packageDataTxt.Text = sp.Packet.PrintHex();
                }
            }
        }




    }
}
