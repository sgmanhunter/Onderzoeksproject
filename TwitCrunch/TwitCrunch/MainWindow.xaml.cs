using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TwitCrunch.CustomControls;
using TwitCrunch.Tools;
using Visifire.Charts;

using TwitCrunch.SQL;


namespace TwitCrunch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow : Window
    {
        private TextBlock sbStatus;

        public MainWindow()
        {
            InitializeComponent();

            Title += " " + Diagnosis.GetVersionNumber();

            // To get the statusbar that is coded into the style template (TwitterCrunchWindow) 
            // we must first load it before we initialise this window
            this.ApplyTemplate();
            sbStatus = this.GetTemplateChild("tbStatus") as TextBlock;

            UpdateStatusBarInfo();
        }


        private void UpdateStatusBarInfo()
        {
            if (sbStatus != null)
            {
                string memory = Diagnosis.GetMemoryUsage();
                string threadCount = Diagnosis.GetThreadCount();
                sbStatus.Text = string.Format("{0} Memory used \t {1} Threads", memory, threadCount);
            }
        }

        private void ResetInput()
        {
            txtSearchTerm.Text = String.Empty;
            dateFrom.SelectedDate = null;
            dateUntil.SelectedDate = null;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            // ToDo: validation
            string searchWord = txtSearchTerm.Text;
            bool everythingOK = false;

            if (!string.IsNullOrEmpty(searchWord))
            {
                if (dateFrom.SelectedDate != null && dateUntil.SelectedDate != null)
                {
                    //Only show until previous day, because today is still being counted
                    if (dateUntil.SelectedDate < DateTime.Now)
                    {
                        AddTwitterCrunchTabItem(searchWord);
                        ResetInput();
                        everythingOK = true;
                    }
                    else
                    {
                        MessageBox.Show("Until date cannot be in the future","Input error",MessageBoxButton.OK,MessageBoxImage.Warning);
                    }
                }
            }

            if (!everythingOK)
            {
                MessageBox.Show("Input is uncorrect", "Input error");
            }

        }

        private void AddTwitterCrunchTabItem(string searchWord)
        {
            TwitterCrunchInfoControl crunch = new TwitterCrunchInfoControl(searchWord);

            TabItem currentItem = new TabItem() { Header = "#" + searchWord.ToUpper(), Content = crunch };

            //ToDo: query each tag, using tag-class     
            tcCrunches.Items.Add(currentItem);

            //select current tab
            currentItem.IsSelected = true;

            UpdateStatusBarInfo();
        }


    }
}
