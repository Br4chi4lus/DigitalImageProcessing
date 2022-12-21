using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DigitalImageProcessing
{
    /// <summary>
    /// Logika interakcji dla klasy InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        public InfoWindow(Status status, double ms)
        {
            InitializeComponent();
            switch (status)
            {
                case Status.Error:
                    this.info.Text = "Wrong input data!";
                    this.time.Text = "";
                    break;
                case Status.Done:
                    this.info.Text = "File has been processed!";
                    this.time.Text = "Processing has taken " + ms.ToString() + "s";
                    break;
                case Status.Error2:
                    this.info.Text = "Nice try, but Magic Number is wrong :-)";
                    this.time.Text = "";
                    break;
                default:
                    break;
            }

        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
