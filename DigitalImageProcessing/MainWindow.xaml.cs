using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Forms;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using System.Threading;
using ComboBox = System.Windows.Controls.ComboBox;
using Button = System.Windows.Controls.Button;

namespace DigitalImageProcessing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public delegate void CallInfoWindowDel(Status status, double ms);
    public delegate void CallUpdateProgress(double progress);
    public delegate void CallEnableProcessButton();
    public partial class MainWindow : Window
    {
        private String fileName = "";
        private String path = "";
        private int mSize = 23;
        private double sValue = 5;
        private int[]? matrix1 = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Bitmap (*.bmp)|*.bmp";
            dialog.ShowDialog();
            if (dialog.FileName != "")
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri(dialog.FileName);
                img.EndInit();
                imgviewer.Source = img;
                fileName = dialog.FileName;
            }
        }

        private void process_Click(object sender, RoutedEventArgs e)
        {
            if (path == "" || fileName == "" || name.Text == "" || methods.SelectedValue==null || ((lowpass.IsSelected || highpass.IsSelected) && matrix1==null))
            {
                OpenInfoWindow(Status.Error, 0);
                return;
            }
            process.IsEnabled = false;
            String m = methods.SelectedValue.ToString();
            Method method = Method.Roberts;
            switch (m)
            {
                case "Roberts Cross":
                    method = Method.Roberts;
                    break;
                case "Prewitt":
                    method = Method.Prewitt;
                    break;
                case "Sobel":
                    method = Method.Sobel;
                    break;
                case "Scharr":
                    method = Method.Scharr;
                    break;
                case "Low-pass filter":
                    method = Method.Lowpass;
                    break;
                case "High-pass filter":
                    method = Method.Highpass;
                    break;
                case "Median filter":
                    method = Method.Median;
                    break;
                case "Gauss filter":
                    method = Method.Gauss;
                    break;
                case "RGB to Grayscale(24bit)":
                    method = Method.Grayscale;
                    break;
                case "RGB to Grayscale(8bit)":
                    method = Method.Grayscale2;
                    break;
                default:
                    return;
            }

            Processor processor = new Processor(this, fileName, name.Text, method, path, mSize, sValue, matrix1);
            Thread thread = new Thread(processor.ProcessImage);
            thread.Start();           
        }

        private void directory_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.ShowDialog();
            if (folderBrowser.SelectedPath != "")
            {
                path = folderBrowser.SelectedPath;
            }
        }
        public static void OpenInfoWindow(Status status, double ms)
        {
            InfoWindow info = new InfoWindow(status, ms);
            info.ShowDialog();
        }
        public void UpdateProgress(double prog)
        {
            progress.Value = prog;
        }

        private void methods_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (gauss.IsSelected)
            {
                values.Visibility = System.Windows.Visibility.Visible;
                flters.Visibility = System.Windows.Visibility.Hidden;
            }
            else if(lowpass.IsSelected || highpass.IsSelected)
            {
                matrix1 = null;
                flters.Visibility = System.Windows.Visibility.Visible;
                values.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                flters.Visibility = System.Windows.Visibility.Hidden;
                values.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void values_Click(object sender, RoutedEventArgs e)
        {
            SelectValues selectValues = new SelectValues(mSize,sValue);
            selectValues.ShowDialog();
            if (selectValues.saveClicked)
            {
                mSize = (int)selectValues.mSize.Value;
                sValue = selectValues.sSize.Value;
            }
        }
        public void EnableProcessButton()
        {
            process.IsEnabled = true;
        }

        private void flters_Click(object sender, RoutedEventArgs e)
        {
            matrix1 = null;
            Method m;
            if (lowpass.IsSelected)
            {
                m = Method.Lowpass;
            }
            else
            {
                m = Method.Highpass;
            }
            SelectFilterValue selectFilter = new SelectFilterValue(m);
            selectFilter.ShowDialog();
            matrix1 = selectFilter.matrix;
            mSize = selectFilter.matrixSize;
        }
    }
}
