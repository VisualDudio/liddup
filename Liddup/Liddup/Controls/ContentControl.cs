using Xamarin.Forms;
using PropertyChanged;

namespace Liddup.Controls
{   
    [AddINotifyPropertyChangedInterface]
    public class ContentControl : ContentView
    {
        public static readonly BindableProperty ContentTemplateProperty = BindableProperty.Create("ContentTemplate", typeof(DataTemplate), typeof(ContentControl), propertyChanged: OnContentTemplateChanged);
        public DataTemplate ContentTemplate
        {
            get => (DataTemplate)GetValue(ContentTemplateProperty);
            set => SetValue(ContentTemplateProperty, value);
        }

        private static void OnContentTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var contentControl = (ContentControl)bindable;

            var template = (DataTemplate)newValue;

            if (template != null)
                contentControl.Content = (View)template.CreateContent();
            else
                contentControl.Content = null;
        }
    }
}
