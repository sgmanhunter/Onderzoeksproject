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
using Visifire.Commons;

namespace TwitCrunch.CustomControls
{
    /// <summary>
    /// Interaction logic for Chart.xaml
    /// </summary>
    public partial class Chart : UserControl
    {
        public Chart()
        {
            InitializeComponent();

            CreateChart();
            
        }

        private void CreateChart()
        {
            //ToDo: this is a default chart, adjust it to our data

            // Create a Chart element
            Visifire.Charts.Chart chart = new Visifire.Charts.Chart();

            // Set chart width and height
            chart.Width = 900;
            chart.Height = 300;

            // Create new DataSeries
            Visifire.Charts.DataSeries dataSeries = new Visifire.Charts.DataSeries();

            // Number of DataPoints to be generated
            int numberOfDataPoints = 10;

            // To set the YValues of DataPoint
            Random random = new Random();

            // Loop and add a few DataPoints
            for (int loopIndex = 0; loopIndex < numberOfDataPoints; loopIndex++)
            {
                // Create a DataPoint
                Visifire.Charts.DataPoint dataPoint = new Visifire.Charts.DataPoint();

                // Set the YValue using random number
                dataPoint.YValue = random.Next(1, 100);

                // Add DataPoint to DataSeries
                dataSeries.DataPoints.Add(dataPoint);
            }

            // Add DataSeries to Chart
            chart.Series.Add(dataSeries);

            
            // Add chart to the LayoutRoot for display
            LayoutRoot.Children.Add(chart);
        }
    }
}
