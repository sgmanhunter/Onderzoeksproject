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

        public DayStats()
        {
            InitializeComponent();
        }

        public DayStats(Dictionary<DateTime?, int?> datapoints = null)
        {
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
                    DataPoint dataPoint = new DataPoint()
                    {
                        // Setting XValue as DateTime will create DateTime Axis

                        XValue = point.Key,
                        YValue = (double)point.Value
                    };
                    dataSeries.DataPoints.Add(dataPoint);
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
