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
using Visifire.Charts;

namespace TwitCrunch.CustomControls
{
    /// <summary>
    /// Interaction logic for DayStats.xaml
    /// </summary>
    public partial class DayStats : UserControl
    {
        private DateTime? from;
        private DateTime? until;

        public DayStats()
        {
            InitializeComponent();
        }

        public DayStats(DateTime? from, DateTime? until, Dictionary<DateTime?, int?> datapoints = null)
        {
            this.from = from;
            this.until = until;
            InitializeComponent();
            CreateStats(datapoints);
        }

        private void CreateStats(Dictionary<DateTime?, int?> datapoints)
        {
            if (datapoints != null)
            {
                // Create new DataSeries
                DataSeries dataSeries = new DataSeries();

                DateTime currentDateTime = DateTime.Now;

                foreach (KeyValuePair<DateTime?, int?> point in datapoints)
                {
                    //ToDo: datapoint mag enkel toegevoegd worden als die tussen het bereik van de gekozen datums ligt
                    DateTime fromToCompare = (DateTime)this.from;
                    DateTime untilToCompare = (DateTime)this.until;
                    DateTime currentDateToCompare = (DateTime)point.Key;

                    /*
                     * < 0 earlier; datum moet  later of gelijk aan from zijn
                     * > 0 later
                    */
                    if ((DateTime.Compare(currentDateToCompare, fromToCompare) >= 0) && (DateTime.Compare(currentDateToCompare, untilToCompare) <= 0))
                    {
                        DataPoint dataPoint = new DataPoint()
                        {
                            // Setting XValue as DateTime will create DateTime Axis

                            XValue = point.Key,
                            YValue = (double)point.Value
                        };
                        dataSeries.DataPoints.Add(dataPoint);
                    }
                }
                dataSeries.RenderAs = RenderAs.Spline;
                dataSeries.XValueType = ChartValueTypes.Date;
                dataSeries.ShowInLegend = false;
                // Add DataSeries to Chart
                chTrend.AxesX[0].IntervalType = IntervalTypes.Days;
                chTrend.Series.Clear();
                chTrend.Series.Add(dataSeries);
            }
        }
    }
}
