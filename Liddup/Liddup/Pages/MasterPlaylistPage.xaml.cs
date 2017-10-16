using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Liddup.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Liddup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPlaylistPage : ContentPage
    {
        private ObservableCollection<Song> _songs = new ObservableCollection<Song>();

        public MasterPlaylistPage()
        {
            InitializeComponent();
            
        }
    }
}