using Liddup.Models;
using Xamarin.Forms;

namespace Liddup.TemplateSelectors
{
    public class SongDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate InQueueTemplate { get; set; }
        public DataTemplate NotInQueueTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => ((Song)item).IsInQueue ? InQueueTemplate : NotInQueueTemplate;
    }
}
