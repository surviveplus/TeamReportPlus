using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamReportPlus.Models
{
    public class TestResultForImport  : INotifyPropertyChanged
    {

        public string SuiteId { get; set; }
        public int SuiteIdForSave { get; set; }

        public string TestCaseId { get; set; }
        public int TestCaseIdForSave { get; set; }

        public string TestCaseTitle { get; set; }

        public string StepNo { get; set; }
        public int StepNoForSave { get; set; }

        public string Action { get; set; }

        public string ExpectedResult { get; set; }

        public string Outcome { get; set; }
        
        public string Comment { get; set; }

        public string Attachments { get; set; }

        public string DateStarted { get; set; }
        public DateTime? DateStartedForSave { get; set; }

        public string DateCompleted { get; set; }
        public DateTime? DateCompletedForSave { get; set; }

        /// <summary>
        /// Backing field of HasFormatError property.
        /// </summary>
        private bool valueOfHasFormatError;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the value of something.
        /// </summary>
        public bool HasFormatError
        {
            get
            {
                return this.valueOfHasFormatError;
            } // end get
            set
            {
                this.valueOfHasFormatError = value;
                if (this.PropertyChanged != null) { this.PropertyChanged(this, new PropertyChangedEventArgs("HasFormatError")); }
            } // end set
        } // end property

        /// <summary>
        /// Backing field of HasImportError property.
        /// </summary>
        private bool valueOfHasImportError;

        /// <summary>
        /// Gets or sets the value of something.
        /// </summary>
        public bool HasImportError
        {
            get
            {
                return this.valueOfHasImportError;
            } // end get
            set
            {
                this.valueOfHasImportError = value;
                if (this.PropertyChanged != null) { this.PropertyChanged(this, new PropertyChangedEventArgs("HasImportError")); }
            } // end set
        } // end property

        /// <summary>
        /// Backing field of ImportResult property.
        /// </summary>
        private string valueOfImportResult;

        /// <summary>
        /// Gets or sets the value of something.
        /// </summary>
        public string ImportResult
        {
            get
            {
                return this.valueOfImportResult;
            } // end get
            set
            {
                this.valueOfImportResult = value;
                if (this.PropertyChanged != null) { this.PropertyChanged(this, new PropertyChangedEventArgs("ImportResult")); }
            } // end set
        } // end property

        public bool IsHeader
        {
            get
            {
                return (this.SuiteId == "Suite");
            }
        }

        public void CheckFormat()
        {
            try
            {
                this.HasFormatError = false;

                this.SuiteIdForSave = int.Parse(this.SuiteId);
                this.TestCaseIdForSave = int.Parse(this.TestCaseId);
                this.StepNoForSave = int.Parse(this.StepNo);

                if (string.IsNullOrWhiteSpace(this.DateStarted) == false){
                    this.DateStartedForSave = DateTime.ParseExact(this.DateStarted, "yyyy/MM/dd HH:mm:ss.FFF", System.Threading.Thread.CurrentThread.CurrentCulture);
                } // end if

                if( string.IsNullOrWhiteSpace(this.DateCompleted) == false){
                    this.DateCompletedForSave = DateTime.ParseExact(this.DateCompleted, "yyyy/MM/dd HH:mm:ss.FFF", System.Threading.Thread.CurrentThread.CurrentCulture);
                } // end if


                switch (this.Outcome)
                {
                    case "Unspecified":
                    case "0":
                    case "None":
                    case "1":
                    case "Passed":
                    case "2":
                    case "Failed":
                    case "3":
                    case "Inconclusive":
                    case "4":
                    case "Timeout":
                    case "5":
                    case "Aborted":
                    case "6":
                    case "Blocked":
                    case "7":
                    case "NotExecuted":
                    case "8":
                    case "Warning":
                    case "9":
                    case "Error":
                    case "10":
                    case "NotApplicable":
                    case "11":
                    case "Paused":
                    case "12":
                    case "InProgress":
                    case "13":
                    case "MaxValue":
                        break;

                    default:
                        this.HasFormatError = true;
                        break;
                }

            }
            catch 
            {
                this.HasFormatError = true;
            }

        } // end sub

    }
}
