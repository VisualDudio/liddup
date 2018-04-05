using Liddup.Services;
using Xamarin.Forms.Xaml;

namespace Liddup.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SongsPage : ISongProvider
    {
        public SongsPage()
        {
            InitializeComponent();
        }
    }
}