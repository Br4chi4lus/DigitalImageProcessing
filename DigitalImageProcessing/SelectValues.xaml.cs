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
    /// Logika interakcji dla klasy SelectValues.xaml
    /// </summary>
    public partial class SelectValues : Window
    {   
        public bool saveClicked = false;
        public SelectValues(int mSize, double sValue)
        {
            InitializeComponent();
            this.mSize.Value= mSize;
            this.sSize.Value= sValue;
        }

        private void mSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mValue.Text = mSize.Value.ToString();
        }

        private void sSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sValue.Text = sSize.Value.ToString();
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            saveClicked= true;
            this.Close();
        }
    }
}
