using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace DigitalImageProcessing
{
    /// <summary>
    /// Logika interakcji dla klasy Done.xaml
    /// </summary>
    public partial class Done : Window
    {
        public Done(double ms)
        {
            
            InitializeComponent();
            time.Text = "Processing has taken " + ms.ToString() + "s";

        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }
    }
}
