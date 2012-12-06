using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ProPresenterTools
{
    public class ArrangementViewModel : INotifyPropertyChanged
    {
        private bool imported;

        public event PropertyChangedEventHandler PropertyChanged;

        public PcoSongArrangement Arrangement { get; set; }

        public string ViewSequence
        {
            get { return string.Join(", ", this.Arrangement.Sequence); }
            set { this.Arrangement.Sequence = value.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray(); }
        }

        public bool Imported
        {
            get { return this.imported; }

            set
            {
                if (value != this.imported)
                {
                    this.imported = value;
                    this.TriggerChanged("Imported");
                }
            }
        }

        private void TriggerChanged(string name)
        {
            if (this.PropertyChanged == null)
                return;

            this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
