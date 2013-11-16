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
using TwitCrunch.data;
using TwitCrunch.Tools;


namespace TwitCrunch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow : Window
    {
       

        private string _consumerKey = "c7g4rHrnerkTPBjdba0Kw";
        private string _consumerSecret = "1IO7eiGYSLaKKpErzfbgRYAEF5KUe7LZrtMz2FAgSFI";
        private string _accessToken = "2159075078-cRSaEeEgFWThXBx49tykpGyILtkxlhefdFcisk8";
        private string _accessTokenSecret = "r9XobK34W2OL5UUVdyDLq5E34t8Xe0AVEljRWdju6m491";

        private TextBlock _sbStatus;

        private ApplicationCredentials _appCred = ApplicationCredentials.Singleton;

        public MainWindow()
        {
            InitializeComponent();

            Title += " "+Diagnosis.GetVersionNumber();

            _appCred.ConsumerKey = _consumerKey;
            _appCred.ConsumerSecret = _consumerSecret;
            _appCred.AccessToken = _accessToken;
            _appCred.AccessTokenSecret = _accessTokenSecret;

            // To get the statusbar that is coded into the style template (TwitterCrunchWindow) we must first load it before we initialise this window
            this.ApplyTemplate();
            _sbStatus = this.GetTemplateChild("tbStatus") as TextBlock;

            UpdateStatusBarInfo();
        }


        private void UpdateStatusBarInfo()
        {
            if (_sbStatus != null)
            {
                string memory = Diagnosis.GetMemoryUsage();
                string threadCount = Diagnosis.GetThreadCount();
                _sbStatus.Text = string.Format("{0} Memory used \t {1} Threads", memory, threadCount);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchWord = txtSearchTerm.Text;
            txtSearchTerm.Text = String.Empty;
            AddTwitterCrunchTabItem(searchWord);
        }

        private void AddTwitterCrunchTabItem(string searchWord)
        {
            TwitterCrunchInfoControl crunch = new TwitterCrunchInfoControl();
            ArrayList a = _appCred.ApiTest();

            foreach (var i in a)
            {
                string element = (string)i;
                crunch.addElementToAccordion(element);
            }

            tcCrunches.Items.Add(new TabItem() { Header = "#" + searchWord, Content = crunch });
            if (tcCrunches.Items.Count > 0) (tcCrunches.Items[0] as TabItem).IsSelected = true;
            UpdateStatusBarInfo();
        }
    }
}
