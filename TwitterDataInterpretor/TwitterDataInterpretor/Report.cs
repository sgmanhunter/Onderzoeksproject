using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterDataInterpretor
{
    class Report
    {
        private Database dbInstance = Database.GetInstance();
        private DateTime from;
        private DateTime until;
        private string report;
        private ArrayList tags;

        public Report(DateTime from, DateTime until)
        {
            this.from = from;
            this.until = until;
            tags = new ArrayList();
            AddTitle();
            AddTags();
            AddTagResultsToReport();
        }

        private void AddTitle()
        {
            this.report += "Datamining report\n\n";
        }

        private void AddTags()
        {
            this.report += "data collected of following Twitter tags: ";

            foreach (string tag in dbInstance.GetAllTagsFromDatabase())
            {
                this.report += tag + ", ";
                tags.Add(new Tag(this.from, this.until, tag));
            }

            //remove last comma
            report = report.Remove(report.Length - 2);
            report += "\n";
        }

        private void AddTagResultsToReport()
        {
            foreach (Tag tag in this.tags)
            {
                report += tag.ToString();
            }
        }
        public override string ToString()
        {
            return report;
        }
    }
}
