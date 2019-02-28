using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using System.IO;
using logic;


namespace PageScrapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool running = false;
        Thread manager_tr;
        private Manager manager = Manager.GetObj();
        public MainWindow()
        {
            InitializeComponent();
            manager.anouncer = AddBrokenLink;
           

        }
        /*   Event add broken link to table
         */
        void AddBrokenLink(Link link)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    ListBoxItem itm = new ListBoxItem();
                    itm.Content = link.ToString();
                    References.Items.Add(itm);
                }
                );
        }

        private void settings_click(object sender, RoutedEventArgs e)
        {
            Settings set = new Settings();
            set.Owner = this;
            set.Show();
            
            
            
        }
        private void start_click(object sender, RoutedEventArgs e)
        {
            if(running)
            manager.Clear();
            manager.end = true;
            
            manager_tr = new Thread(manager.BeginProcess);
            manager_tr.Start(Set.thread_num);
            
        }
        private void stop_click(object sender, RoutedEventArgs e)
        {
            manager.end = true;
            
        }
        private void pause_click(object sender, RoutedEventArgs e)
        {

        }
        private void save_click(object sender, RoutedEventArgs e)
        {

        }
        private void help_click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            manager.end = true;
            manager.Clear();
        }
    }
}
