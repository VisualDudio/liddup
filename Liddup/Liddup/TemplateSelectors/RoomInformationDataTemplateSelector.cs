using Liddup.Models;
using Xamarin.Forms;
using Liddup.Pages;

namespace Liddup.TemplateSelectors
{
    class RoomInformationDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate HostTemplate = new DataTemplate(() => new RoomInformationPage());
        public DataTemplate NotHostTemplate = new DataTemplate(() => new HostRoomInformationPage());

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => HostTemplate;
    }
}
