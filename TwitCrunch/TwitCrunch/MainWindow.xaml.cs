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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TwitCrunch.data;

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

        private User _user = User.Singleton;

        public MainWindow()
        {
            InitializeComponent();

            _user.ConsumerKey = _consumerKey;
            _user.ConsumerSecret = _consumerSecret;
            _user.AccessToken = _accessToken;
            _user.AccessTokenSecret = _accessTokenSecret;

            //_user.Connect();
        }
    }
}
