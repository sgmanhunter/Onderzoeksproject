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

namespace TwitCrunch.CustomControls
{
    /// <summary>
    /// Interaction logic for TwitterCrunchTabItem.xaml
    /// </summary>
    public partial class TwitterCrunchInfoControl : UserControl
    {

        private string _searchKeyword;
        private Grid grid;
        private Accordion accordion;



        public TwitterCrunchInfoControl(string searchWord)
        {
            InitializeComponent();
            InitializeThis();
            _searchKeyword = searchWord;
            addElementToAccordion(_searchKeyword);
        }

        private void InitializeThis()
        {
            //grid
            grid = new Grid();
            ColumnDefinition width1 = new ColumnDefinition();
            width1.Width = new GridLength(137);
            ColumnDefinition width2 = new ColumnDefinition();
            width2.Width = new GridLength(663);

            grid.ColumnDefinitions.Add(width1);
            grid.ColumnDefinitions.Add(width2);

            //accordion
            accordion = new Accordion();

            //add accordion to grid
            grid.Children.Add(accordion);

            //add grid to this controll
            this.AddChild(grid);
        }

        private void addElementToAccordion(string element)
        {
            AccordionItem ai = new AccordionItem();
            ai.Header = element;
            ai.Content = element;
            accordion.Items.Add(ai);
             
        }
    }
}
