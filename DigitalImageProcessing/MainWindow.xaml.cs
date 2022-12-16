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

namespace DigitalImageProcessing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private String fileName = "";
        private String path = "";
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
            if (path == "" || fileName == "" || name.Text == "" || methods.SelectedValue==null)
            {
                Error error = new Error();
                error.ShowDialog();
                return;
            }
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
                default:
                    return;
            }
            
            Processor processor = new Processor(fileName, name.Text, method,path);
            processor.ProcessImage();
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
    }
}
