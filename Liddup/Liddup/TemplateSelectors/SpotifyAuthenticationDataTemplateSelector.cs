using Liddup.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Liddup.TemplateSelectors
{
    class SpotifyAuthenticationDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AuthenticatedTemplate { get; set; }
        public DataTemplate NotAuthenticatedTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => (item as User).IsSpotifyAuthenticated ? AuthenticatedTemplate : NotAuthenticatedTemplate;
    }
}
