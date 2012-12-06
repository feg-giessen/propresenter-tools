using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ProPresenter.DO.PCO;

namespace ProPresenterTools
{
    partial class PcoConnector
    {
        private RVOAuthPCO access = RVOAuthPCO.GetInstance();

        public IList<PcoSong> GetSongs()
        {
            string songs = this.GetSongsStructure();

            var doc = XDocument.Parse(songs);

            List<PcoSong> songList = new List<PcoSong>();

            foreach (XElement element in doc.Root.Elements("song"))
            {
                var song = this.ParseSong(element);

                songList.Add(song);
            }

            return songList;
        }

        private string GetSongsStructure()
        {
            return this.access.APIWebRequest(
                "GET", 
                "https://www.planningcenteronline.com/songs.xml", 
                null);
        }

        private string GetArrangementStructure(int songId)
        {
            return this.access.APIWebRequest(
                "GET",
                string.Format("https://www.planningcenteronline.com/songs/{0}/arrangements.xml", songId),
                null);
        }

        private PcoSongArrangement ParseArrangement(XElement element)
        {
            return new PcoSongArrangement
            {
                Id = int.Parse(element.Element("id").Value),
                Name = element.Element("name").Value,
                SongId = Convert.ToInt32(element.Element("song-id").Value),
                Notes = element.Element("notes").Value,
                ChordChart = element.Element("chord-chart").Value,
                Sequence = element.Element("sequence-to-s").Value.Split(',').Select(s => s.Trim().ToLower()).Where(s => s != "intro" && s != "outro" && !string.IsNullOrWhiteSpace(s)).ToArray(),
            };
        }

        private PcoSong ParseSong(XElement element)
        {
            int id = int.Parse(element.Element("id").Value);

            Func<PcoSongArrangement[]> func = () => 
            {
                var arrangementXml = XDocument.Parse(this.GetArrangementStructure(id));
                return arrangementXml.Root.Elements("arrangement").Select(this.ParseArrangement).ToArray();
            };

            return new PcoSong(func)
            {
                Id = id,
                Title = this.AdjustUmlauts(element.Element("title").Value),
                Author = element.Element("author").Value,
                CcliId = string.IsNullOrEmpty(element.Element("ccli-id").Value) ? default(int?) : Convert.ToInt32(element.Element("ccli-id").Value),
                Copyright = element.Element("copyright").Value,
                CreatedAt = Convert.ToDateTime(element.Element("created-at").Value),
                CreatedById = int.Parse(element.Element("created-by-id").Value),
                Hidden = bool.Parse(element.Element("hidden").Value),
                Notes = element.Element("notes").Value,
                Themes = element.Element("themes").Value.Split(',').Select(s => s.Trim()).ToArray(),
            };
        }

        private string AdjustUmlauts(string text)
        {
            //var chars = new[]
            //{
            //    "Ä", "ä", "Ö", "ö", "Ü", "ü", "ß"
            //};

            //foreach (string item in chars)
            //{
            //    text = text.Replace(Encoding.ASCII.GetString(Encoding.UTF8.GetBytes(item)), item);
            //}
            
            text = text.Replace("Ã¶", "ö");

            return text;
        }
    }
}