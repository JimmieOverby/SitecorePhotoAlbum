using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitecore.Modules.PhotoAlbum.BusinessLayer.UrlShortenerClass
{
    public interface IUrlShortener
    {
        Uri ShortenUrl(Uri url, string key);
        Uri ShortenUrl(Uri url);
    }
}
