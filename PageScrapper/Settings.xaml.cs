﻿using System;
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
using logic;
using PageScrapper;

namespace PageScrapper
{
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
            Domen.Text = Set.dom;
            SubDomen.Text = Set.subdom;
            Way.Text = Set.way;
            URLBox.Text = Set.URL?.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Set.thread_num = (int)slider.Value;
                Set.URL = new Link(URLBox.Text);
                Set.dom = Domen.Text;
                Set.subdom = SubDomen.Text;
                Set.way = Way.Text;
            }
            catch (Exception ) {
                MessageBox.Show("Некорректные данные!");
                return;
            }
            Set.start_button_enable();
            this.Close();
        }
    }
}
