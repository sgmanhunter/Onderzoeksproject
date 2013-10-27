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

        private string _consumerKey = "T3TaQWzzZiVrihJW0GLBg";
        private string _consumerSecret = "OJo28FZzFfXZtFcVOF1FDJpiOpatQMQSvI4aetQ";
        private string _accessKey = "2159075078-0NdpXHjpvi46rFpBw3iAx2SGhU0i8LunCGsJCsd";
        private string _accessSecret = "fg4E3KIi0wJy8kmu8RR2lI2WhuS5VoIuy5AwVtPxPvjua";
        private string _pin = "YU2HFbPJe2LES8wmKvKn";

        private User _user = User.Singleton;

        public MainWindow()
        {
            InitializeComponent();

            _user.ConsumerKey = _consumerKey;
            _user.ConsumerSecret = _consumerSecret;
            _user.AccessKey = _accessKey;
            _user.ConsumerSecret = _accessSecret;
            _user.Pin = _pin;

            _user.Connect();
        }
    }
}
