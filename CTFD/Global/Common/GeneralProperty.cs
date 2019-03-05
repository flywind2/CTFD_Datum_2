using CTFD.Model.Base;
using CTFD.Model.RuntimeData;
using CTFD.View.Monitor;
using log4net;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Communication;
using System.Net;
using System.Windows.Data;

namespace CTFD.Global.Common
{
    public static partial class General
    {
        public static event EventHandler<GlobalEventArgs> GlobalHandler;

        public static ILog Log { get; } = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static WorkingData WorkingData { get; set; } = new WorkingData();

        public static SolidColorBrush GrayColor => General.FindResource(Properties.Resources.GrayColor) as SolidColorBrush;

        public static SolidColorBrush GreenColor => General.FindResource(Properties.Resources.GreenColor) as SolidColorBrush;

        public static SolidColorBrush BlueColor => General.FindResource(Properties.Resources.BlueColor) as SolidColorBrush;

        public static SolidColorBrush BlueColor2 => General.FindResource(Properties.Resources.BlueColor2) as SolidColorBrush;

        public static SolidColorBrush WathetColor => General.FindResource(Properties.Resources.WathetColor) as SolidColorBrush;

        public static SolidColorBrush WathetColor2 => General.FindResource(Properties.Resources.WathetColor2) as SolidColorBrush;

        public static SolidColorBrush WathetColor3 => General.FindResource(Properties.Resources.WathetColor3) as SolidColorBrush;

        public static SolidColorBrush ChartColor1 => General.FindResource(Properties.Resources.ChartColor1) as SolidColorBrush;

        public static SolidColorBrush ChartColor2 => General.FindResource(Properties.Resources.ChartColor2) as SolidColorBrush;

        public static SolidColorBrush ChartColor3 => General.FindResource(Properties.Resources.ChartColor3) as SolidColorBrush;

        public static PathGeometry DoubleLeft => General.FindPathResource(nameof(DoubleLeft));

        public static PathGeometry DoubleRight => General.FindPathResource(nameof(DoubleRight));

        public static SolidColorBrush ChartColor4 = new SolidColorBrush(Colors.Bisque);

        public static SolidColorBrush Transparent => new SolidColorBrush(Colors.Transparent);

        public static SocketClient TcpClient { get; set; }

        public static DataBaseOperation DataBaseOperation { get; set; }

        public static string LoaclIPAddress => "127.0.0.1";//Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault(o => o.AddressFamily== System.Net.Sockets.AddressFamily.InterNetwork).ToString();

        public static Status Status { get; set; } = Status.Stop;

        public static object Stop { get; set; }

        public static object Run { get; set; }

        public static object CoolDown { get; set; }

        public static bool IsShowRawData { get; set; } = true;

        public static bool IsShowSmoothData { get; set; } = true;
    }
}
