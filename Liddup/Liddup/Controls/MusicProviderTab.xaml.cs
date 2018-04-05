using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Liddup.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MusicProviderTab : Grid
	{
        public static readonly BindableProperty CurrentColorProperty = BindableProperty.Create("CurrentColor", typeof(Color), typeof(MusicProviderTab), Color.White);
        public static readonly BindableProperty ActiveColorProperty = BindableProperty.Create("ActiveColor", typeof(Color), typeof(MusicProviderTab), Color.FromHex("#ff2d2d"));
        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(MusicProviderTab), "");
        public static readonly BindableProperty InactiveIconProperty = BindableProperty.Create("InactiveIcon", typeof(string), typeof(MusicProviderTab), "");
        public static readonly BindableProperty CurrentIconProperty = BindableProperty.Create("CurrentIcon", typeof(string), typeof(MusicProviderTab), "");
        public static readonly BindableProperty ActiveIconProperty = BindableProperty.Create("ActiveIcon", typeof(string), typeof(MusicProviderTab), "");
        public static readonly BindableProperty IsActiveProperty = BindableProperty.Create("IsActive", typeof(bool), typeof(MusicProviderTab), false, propertyChanged: IsActiveChanged);

        public Color CurrentColor
        {
            get { return (Color)GetValue(CurrentColorProperty); }
            set { SetValue(CurrentColorProperty, value); }
        }

        public Color ActiveColor
        {
            get { return (Color)GetValue(ActiveColorProperty); }
            set { SetValue(ActiveColorProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public string InactiveIcon
        {
            get { return (string)GetValue(InactiveIconProperty); }
            set { SetValue(InactiveIconProperty, value); }
        }

        public string CurrentIcon
        {
            get { return (string)GetValue(CurrentIconProperty); }
            set { SetValue(CurrentIconProperty, value); }
        }

        public string ActiveIcon
        {
            get { return (string)GetValue(ActiveIconProperty); }
            set { SetValue(ActiveIconProperty, value); }
        }

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public MusicProviderTab()
        {
            BindingContext = this;
            InitializeComponent();

            GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(BecomeActive) });
        }

        public void BecomeActive()
        {
            IsActive = true;
        }
        
        private static void IsActiveChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var tab = bindable as MusicProviderTab;
            if ((bool)newValue)
            {
                tab.CurrentIcon = tab.ActiveIcon;
                tab.CurrentColor = tab.ActiveColor;
            }
            else
            {
                tab.CurrentIcon = tab.InactiveIcon;
                tab.CurrentColor = Color.White;
            }
        }
    }
}