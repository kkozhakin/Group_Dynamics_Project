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

namespace PageScrapper
{
    static class Set
    {
        public static string URL= "";
        public static string subdom = "";
        public static string dom = "";
        public static string way = "";
        public static int thread_num = 4;
       
    }
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            //Window.ResizeModeProperty = NoR
            InitializeComponent();
            slider.Value = 4;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Set.thread_num = (int)slider.Value;
            Set.URL = URLBox.Text;
            Set.dom = Domen.Text;
            Set.subdom = SubDomen.Text;
            Set.way = Way.Text;
            this.Close();
        }
    }
}
