using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProPresenter.BO.Common;
using ProPresenter.BO.Session;

namespace ProPresenterTools
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Importer importer = new Importer();

        public MainWindow()
        {
            IRVSession sessionObject = RVSessionManager.GetSessionObject();
            sessionObject.MainWindow = this;
            sessionObject.MainDispatcher = this.Dispatcher;

            AppDomain.CurrentDomain.SetThreadPrincipal(sessionObject);
            Thread.CurrentPrincipal = sessionObject;

            this.DataContext = this;

            this.Songs = new ObservableCollection<SongViewModel>();

            InitializeComponent();

            this.ltbSongs.SelectedCellsChanged += ltbSongs_SelectedCellsChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void ltbSongs_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs("Current"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("CurrentText"));
            }

            txbText_Loaded(null, null);
        }

        public ObservableCollection<SongViewModel> Songs { get; set; }

        public SongViewModel Current
        {
            get
            {
                return this.ltbSongs.SelectedItem as SongViewModel;
            }
        }

        public string CurrentText
        {
            get
            {
                if (this.Current == null)
                    return null;

                return this.Current.Pco.Arrangements[0].ChordChart;
            }

            set
            {
                if (this.Current == null)
                    return;

                this.Current.Pco.Arrangements[0].ChordChart = value;
            }
        }

        private void OnButton1Click(object sender, RoutedEventArgs e)
        {
            var connector = new PcoConnector();

            foreach (var song in connector.GetSongs())
            {
                var model = new SongViewModel(song, () => this.importer.GetPresentation(song.Title));
                model.Imported = this.importer.IsImported(song.Title);

                this.Songs.Add(model);
            }
        }

        private void OnImportClicked(object sender, RoutedEventArgs e)
        {
            if (this.Current == null)
                return;

            this.importer.ImportSong(this.Current.Pco, false);
            this.Current.Imported = true;

            foreach (var item in this.Current.Arrangements)
            {
                item.Imported = true;
            }

            this.ltbSongs.Items.Refresh();

            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs("Current"));
            }
        }

        private void OnOverwriteClicked(object sender, RoutedEventArgs e)
        {
            if (this.Current == null)
                return;

            this.importer.ImportSong(this.Current.Pco, true);
        }

        private void txbText_Loaded(object sender, RoutedEventArgs e)
        {
            //if (string.IsNullOrWhiteSpace(this.CurrentText))
            //    return;

            //int ind = this.CurrentText.ToLower().IndexOf("verse");

            //if (ind < 0)
            //    return;

            //var start = this.txbText.Document.ContentStart.GetPositionAtOffset(ind);
            //var ende = this.txbText.Document.ContentStart.GetPositionAtOffset(this.CurrentText.IndexOf("\n", ind));
            //this.txbText.Selection.Select(start, ende);
            //this.txbText.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Color.FromRgb(255, 100, 100)));
        }

        private void OnArrangementRefreshClicked(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button == null || this.Current == null || this.Current.Presentation == null)
                return;

            ArrangementViewModel arrangement = this.Current.Arrangements.FirstOrDefault(x => x.Arrangement.Id == (int)button.Tag);

            this.importer.SetArrangement(this.Current.Presentation, arrangement.Arrangement);

            this.Current.Presentation.IsDirty = true;
            this.Current.Presentation.Save();

            arrangement.Imported = true;
        }
    }
}
