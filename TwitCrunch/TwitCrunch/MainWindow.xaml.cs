using System;
using System.Collections;
using System.Collections.Generic;
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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            // ToDo: validation
            string searchWord = txtSearchTerm.Text;
            bool everythingOK = false;

            if (searchWord != "")
            {
                if (dateFrom.SelectedDate != null && dateUntil.SelectedDate != null)
                {
                    if (dateUntil.SelectedDate <= DateTime.Now)
                    {
                        AddTwitterCrunchTabItem(searchWord);
                        txtSearchTerm.Text = String.Empty;
                        dateFrom.SelectedDate = null;
                        dateUntil.SelectedDate = null;
                        everythingOK = true;
                    }
                }
            }

            if (!everythingOK)
            {
                MessageBox.Show("Error: input is uncorrect");
            }

        }

        private void AddTwitterCrunchTabItem(string searchWord)
        {
            TwitterCrunchInfoControl crunch = new TwitterCrunchInfoControl();
            
            TabItem currentItem = new TabItem() { Header = "#" + searchWord.ToUpper(), Content = crunch };

            //ToDo: query each tag, using tag-class

            //ToDo: pass data to chart
            currentItem.Content = new CustomControls.Chart();


            tcCrunches.Items.Add(currentItem);
            currentItem.IsSelected = true;

           

            UpdateStatusBarInfo();
        }
    }
}
