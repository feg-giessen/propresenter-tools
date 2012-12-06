using System;

namespace ProPresenterTools
{
    public class PcoSong
    {
        private readonly Lazy<PcoSongArrangement[]> arrangements;

        public PcoSong(Func<PcoSongArrangement[]> arrangements)
        {
            this.arrangements = new Lazy<PcoSongArrangement[]>(arrangements);
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public int? CcliId { get; set; }

        public string Copyright { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedById { get; set; }

        public bool Hidden { get; set; }

        public string Notes { get; set; }

        public string[] Themes { get; set; }

        public PcoSongArrangement[] Arrangements 
        {
            get { return this.arrangements.Value; }
        }

        public bool Imported { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", this.Id, this.Title);
        }
    }
}