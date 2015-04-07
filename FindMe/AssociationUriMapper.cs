using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Navigation;

namespace FindMe
{
    class AssociationUriMapper : UriMapperBase
    {
        private string tempUri;
        public override Uri MapUri(Uri uri)
        {
            tempUri = System.Net.HttpUtility.UrlDecode(uri.ToString());
            if (tempUri.Contains("findme"))
            {
                string parameters = "";
                return new Uri("/FindPage.xaml?" + parameters, UriKind.Relative);
            }

            return uri;
        }
    }
}
