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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TwitCrunch.CustomControls
{
    /// <summary>
    /// Interaction logic for TwitterCrunchTabItem.xaml
    /// </summary>
    public partial class TwitterCrunchInfoControl : UserControl
    {
        public TwitterCrunchInfoControl()
        {
            InitializeComponent();
        }

        public void addElementToAccordion(string element)
        {
            AccordionItem ai = new AccordionItem();
            ai.Header = element;
            ai.Content = element;
            accTweets.Items.Add(ai);
        }
    }
}
