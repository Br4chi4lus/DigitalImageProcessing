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
    /// Logika interakcji dla klasy SelectFilterValue.xaml
    /// </summary>
    public partial class SelectFilterValue : Window
    {
        private Method method;
        private int numberOf3x3;
        private int numberOf5x5;
        private int[][] matrices3x3;
        private int[][] matrices5x5;
        public int[]? matrix;
        public int matrixSize;
        public SelectFilterValue(Method method)
        {
            InitializeComponent();
            numberOf3x3 = 4;
            numberOf5x5 = 1;
            this.method = method;
            if (method == Method.Highpass)
            {
                filter5.Visibility=System.Windows.Visibility.Hidden;
            }
            matrix = null;
            matrices3x3 = new int[8][];
            matrices3x3[0] = new int[9] { 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            matrices3x3[1] = new int[9] { 1, 1, 1, 1, 2, 1, 1, 1, 1 };
            matrices3x3[2] = new int[9] { 0, 1, 0, 1, 4, 1, 0, 1, 0 };
            matrices3x3[3] = new int[9] { 1, 2, 1, 2, 4, 2, 1, 2, 1 };
            matrices3x3[4] = new int[9] { -1, -1, -1, -1, 9, -1, -1, -1, -1 };
            matrices3x3[5] = new int[9] { 0, -1, 0, -1, 5, -1, 0, -1, 0 };
            matrices3x3[6] = new int[9] { 1, -2, 1, -2, 5, -2, 1, -2, 1 };
            matrices3x3[7] = new int[9] { -1, -1, -1, -1, 14, -1, -1, -1, -1 };
            matrices5x5 = new int[1][];
            matrices5x5[0] = new int[25] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int mul = 1;
            if (method == Method.Lowpass)
            {
                mul = 0;
            }
            matrixSize = 3;
            if (filter1.IsSelected)
            {
                matrix = matrices3x3[mul * numberOf3x3];
            }
            else if (filter2.IsSelected)
            {
                matrix = matrices3x3[mul * numberOf3x3 + 1];
            }
            else if (filter3.IsSelected)
            {
                matrix = matrices3x3[mul * numberOf3x3 + 2];
            }
            else if (filter4.IsSelected)
            {
                matrix = matrices3x3[mul * numberOf3x3 + 3];
            }
            else if (filter5.IsSelected)
            {
                matrixSize = 5;
                matrix = matrices5x5[mul * numberOf5x5];
            }
            this.Close();
        }
    }
}
