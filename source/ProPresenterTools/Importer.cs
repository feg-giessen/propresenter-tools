using System;
using System.Linq;
using ProPresenter.BO.RenderingEngine.Entities;
using ProPresenter.BO.RenderingEngine.Entities.Interface;

namespace ProPresenterTools
{
    internal class Importer
    {
        private readonly PcoFileImporter connector;

        public Importer()
        {
            this.connector = new PcoFileImporter(new ProPresenter.BO.Common.RVPresentationParameters
            {
                Height = 768,
                Width = 1024,
                SelectedCategory = "Song",
            });

            RVPresentationDocument.Load();
        }

        public bool ImportSong(PcoSong song, bool overwrite)
        {
            var arr = song.Arrangements.FirstOrDefault();

            if (arr == null)
                return false;

            var presentation = this.connector.GetImportedPresentation(
                    arr.ChordChart,
                    song.Title);

            if (presentation == null)
                return false;

            presentation.CCLINo = string.Format("{0}", song.CcliId);
            presentation.Author = song.Author;
            presentation.Publisher = song.Copyright;
            presentation.Title = song.Title;
            presentation.IsCopyrightDisplayed = true;

            foreach (var arrangement in song.Arrangements)
            {
                this.SetArrangement(presentation, arrangement);
            }

            RVPresentationDocument.AddPresentation(presentation, overwrite);

            return true;
        }
  
        public void SetArrangement(RVPresentation presentation, PcoSongArrangement arrangement)
        {
            RVGroupArrangement group;

            group = presentation.GroupArrangements.FirstOrDefault(x => x.Label.Label == arrangement.Name);

            if (group == null)
            {
                group = presentation.AddNewGroupArrangement(arrangement.Name);
            }

            if (!arrangement.Sequence.Any())
                return;

            group.SlideGroups.Clear();

            foreach (string seq in arrangement.Sequence)
            {
                RVSlideGroup selected = null;

                if (seq.First() == 'v')
                {
                    if (seq.Length > 1)
                        selected = presentation.SlideGroups.FirstOrDefault(g => g.Label.Label.Equals("verse " + seq.Substring(1), StringComparison.OrdinalIgnoreCase));

                    if (selected == null)
                        selected = presentation.SlideGroups.FirstOrDefault(g => g.Label.Label.StartsWith("verse", StringComparison.OrdinalIgnoreCase));
                }
                else if (seq.First() == 'c')
                {
                    if (seq.Length > 1)
                        selected = presentation.SlideGroups.FirstOrDefault(g => g.Label.Label.Equals("chorus " + seq.Substring(1), StringComparison.OrdinalIgnoreCase));

                    if (selected == null)
                        selected = presentation.SlideGroups.FirstOrDefault(g => g.Label.Label.StartsWith("chorus", StringComparison.OrdinalIgnoreCase));
                }
                else if (seq.First() == 'b')
                {
                    if (seq.Length > 1)
                        selected = presentation.SlideGroups.FirstOrDefault(g => g.Label.Label.Equals("bridge " + seq.Substring(1), StringComparison.OrdinalIgnoreCase));

                    if (selected == null)
                        selected = presentation.SlideGroups.FirstOrDefault(g => g.Label.Label.StartsWith("bridge", StringComparison.OrdinalIgnoreCase));
                }
                else if (seq.StartsWith("pc"))
                {
                    if (seq.Length > 2)
                        selected = presentation.SlideGroups.FirstOrDefault(g => g.Label.Label.Equals("pre-chorus " + seq.Substring(2), StringComparison.OrdinalIgnoreCase));

                    if (selected == null)
                        selected = presentation.SlideGroups.FirstOrDefault(g => g.Label.Label.StartsWith("pre-chorus", StringComparison.OrdinalIgnoreCase));
                }
                else if (seq.StartsWith("misc"))
                {
                    if (seq.Length > 4)
                        selected = presentation.SlideGroups.FirstOrDefault(g => g.Label.Label.Equals("misc " + seq.Substring(4), StringComparison.OrdinalIgnoreCase));

                    if (selected == null)
                        selected = presentation.SlideGroups.FirstOrDefault(g => g.Label.Label.StartsWith("misc", StringComparison.OrdinalIgnoreCase));
                }

                if (selected != null)
                    group.SlideGroups.Add(selected);
            }
        }

        public bool IsImported(string file)
        {
            return RVPresentationDocument.ContainsPresentation(file);
        }

        public RVPresentation GetPresentation(string file)
        {
            if (!this.IsImported(file))
                return null;

            return (RVPresentation)RVPresentationDocument.GetPresentation(file);
        }
    }
}
