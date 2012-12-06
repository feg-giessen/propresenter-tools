using System.Linq;

namespace ProPresenterTools
{
    public class PcoSongArrangement
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int SongId { get; set; }

        public string Notes { get; set; }

        public string ChordChart { get; set; }

        public string[] Sequence { get; set; }
    }
}