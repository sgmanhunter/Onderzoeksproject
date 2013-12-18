using System;
using System.Collections.Generic;
using System.Configuration;
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
using TwitCrunch.SQL;

namespace TwitCrunch.CustomControls
{
    /// <summary>
    /// Interaction logic for TwitterCrunchTabItem.xaml
    /// </summary>
    public partial class TwitterCrunchInfoControl : UserControl
    {
        private string keyword;
        private DateTime? from;
        private DateTime? until;
        public TwitterCrunchInfoControl(string keyword, DateTime? from, DateTime? until)
        {
            this.keyword = keyword;
            this.from = from;
            this.until = until;
            InitializeComponent();
            InitCharts();
        }

        public void InitCharts()
        {
            //deleted the check on emptiness of the keywoard, because it's being checked before
            //see MainWindow
            TwitCrunchStatsContext stats = new TwitCrunchStatsContext(ConfigurationManager.ConnectionStrings["TwitCrunchDataBase"].ConnectionString);
            DayStats daystats = new DayStats(from, until, stats.GetDayStatsFromKeyWord(keyword));
            gStats.Children.Add(daystats);
        }

    }
}
