using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ProPresenter.BO.RenderingEngine.Entities;

namespace ProPresenterTools
{
    public class SongViewModel : INotifyPropertyChanged
    {
        private readonly Lazy<RVPresentation> presentationFac;

        private readonly Lazy<IList<ArrangementViewModel>> arrangements;

        private bool imported;

        public event PropertyChangedEventHandler PropertyChanged;

        public SongViewModel(PcoSong pco, Func<RVPresentation> presentationFac)
        {
            this.presentationFac = new Lazy<RVPresentation>(presentationFac);
            this.Pco = pco;

            this.arrangements = new Lazy<IList<ArrangementViewModel>>(() => this.Pco.Arrangements.Select(x => new ArrangementViewModel 
            { 
                Arrangement = x ,
                Imported = this.Presentation != null && this.Presentation.GroupArrangements.Any(y => y.Label.Label == x.Name)
            }).ToList());
        }

        public PcoSong Pco { get; private set; }

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

        public RVPresentation Presentation
        {
            get 
            {
                if (!this.Imported)
                    return null;

                return this.presentationFac.Value; 
            }
        }

        public IList<ArrangementViewModel> Arrangements
        {
            get { return this.arrangements.Value; }
        }

        public string CurrentLabels
        {
            get
            {
                if (this.Presentation == null)
                    return null;

                return string.Join(", ", this.Presentation.GroupArrangements.Select(x => x.SlideGroups).First().Select(x => x.Label.Label));
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
