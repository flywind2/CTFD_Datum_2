using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;

namespace CTFD.View.Help
{
    /// <summary>
    /// HelpView.xaml 的交互逻辑
    /// </summary>
    public partial class HelpView : UserControl
    {
        public HelpView()
        {
            InitializeComponent();
            this.ReadDocx($"{Environment.CurrentDirectory} \\Global\\Resource\\Document\\Help.xps");
        }
        private void ReadDocx(string path)
        {
            try
            {
                this.DocumentViewer_Help.Document = new XpsDocument(path, FileAccess.Read).GetFixedDocumentSequence();
                this.DocumentViewer_Help.FitToWidth();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}