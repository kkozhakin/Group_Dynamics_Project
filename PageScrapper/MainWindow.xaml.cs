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
            manager.skippedChanges = updateStat;
            manager.endOfSearch = killSearch;
            Set.start_button_enable = st_bt_en;
        }

        public void st_bt_en()
        {
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (ThreadStart)delegate ()
                    {
                        StartButton.IsEnabled = true;
                    }
                );
            }
        }

        public void updateStat()
        {
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (ThreadStart)delegate ()
                    {
                        DoneTextNumber.Text = manager.done.ToString();
                        BrokenTextNumber.Text = manager.bad.ToString();
                    }
                );
            }
        }

        public void killSearch()
        {
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (ThreadStart)delegate ()
                    {
                        pbStatus.IsIndeterminate = false;
                        pbText.Text = "Готово!";
                        MessageBox.Show($" Просмотренно: {manager.done}\n Повреждённых: {manager.bad}");

                        SettingsButton.IsEnabled = true;
                        SaveButton.IsEnabled = true;
                        StartButton.IsEnabled = false;
                        EraseButton.IsEnabled = true;
                        PauseButton.IsEnabled = false;
                    }
                );
            }
        }

        /*   Event add broken link to table
         */
        void AddBrokenLink(Link link)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    if (!link.ErrorComments.Contains("503"))
                    {
                        ListBoxItem itm = new ListBoxItem();
                        itm.Content = link.ToString();
                        References.Items.Add(itm);
                    }
                }
            );
        }

        private void settings_click(object sender, RoutedEventArgs e)
        {
                Settings set = new Settings();
                set.Owner = this;
                set.ShowDialog();
        }
        private void start_click(object sender, RoutedEventArgs e)
        {
            manager.end = true;
            
            manager.SetURL(Set.URL.ToString());
            if (Set.dom == "")
                manager.setRestriction(Set.URL.MainDomen, Set.subdom, Set.way);
             else
                manager.setRestriction(Set.dom, Set.subdom, Set.way);
            
            manager_tr = new Thread(manager.BeginProcess);
            manager_tr.Start(Set.thread_num);

            pbStatus.IsIndeterminate = true;
            pbText.Text = "В процессе...";
            
            SettingsButton.IsEnabled = false;
            SaveButton.IsEnabled = false;
            StartButton.IsEnabled = false;
            EraseButton.IsEnabled = false;
            PauseButton.IsEnabled = true;
        }
        private void erase_click(object sender, RoutedEventArgs e)
        {
            manager.end = true;
            manager.Clear();

            References.Items.Clear();
            pbText.Text = "Очищено";

            SettingsButton.IsEnabled = true;
            SaveButton.IsEnabled = false;
            StartButton.IsEnabled = false;
            EraseButton.IsEnabled = false;
            PauseButton.IsEnabled = false;
            References.Items.Clear();

        }
        private void pause_click(object sender, RoutedEventArgs e)
        {
            manager.end = true;

            pbStatus.IsIndeterminate = false;
            pbText.Text = "Остановлено";

            StartButton.IsEnabled = true;
            EraseButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
            SaveButton.IsEnabled = true;
        }
        private void save_click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".text"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                using (StreamWriter writetext = new StreamWriter(filename))
                {
                    writetext.WriteLine(manager.GetBadLinks());
                }
            }

        }
        private void help_click(object sender, RoutedEventArgs e)
        {
            Help help = new Help();
            help.Owner = this;
            help.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            manager.end = true;
            manager.Clear();
        }
    }
}
