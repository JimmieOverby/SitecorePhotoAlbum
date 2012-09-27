using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Publishing;
using Sitecore.SecurityModel;

namespace Sitecore.Modules.PhotoAlbum.BusinessLayer.FeedHelperClass
{
    public class FeedHelper
    {

        private const string AlbumsFeedName = "AlbumsListFeed";
        private const string AlbumFeedName = "AlbumFeed";
        private const string AlbumsFeedQuery = "query:..//*[@@templatename=\"Album\"]";
        private const string AlbumFeedQuery = "query:..//*[@@templatename=\"Photo\"]";
        public static void CreateUserAlbumsFeed(Item album)
        {
            using (new SecurityDisabler())
            {
                Item parent = album.Parent;
                IEnumerable<Item> source =
                    from x in parent.Children
                    where x.TemplateID.ToString().Equals(PhotoAlbumConstants.FeedTemplateId)
                    select x;
                if (source.Count<Item>() == 0)
                {
                    TemplateItem templateItem = Context.Database.Templates[PhotoAlbumConstants.FeedTemplateId];
                    if (templateItem != null)
                    {
                        Item item = parent.Add("AlbumsListFeed", new TemplateID(templateItem.ID));
                        item.Editing.BeginEdit();
                        item["Title"] = "Albums Feed";
                        item["Source"] = "query:..//*[@@templatename=\"Album\"]";
                        item.Editing.EndEdit();
                        Database database = Factory.GetDatabase("master");
                        Database database2 = Factory.GetDatabase("web");
                        var publisher = new Publisher(new PublishOptions(database, database2, PublishMode.SingleItem, Context.Language, DateTime.Now)
                        {
                            RootItem = item
                        });
                        publisher.Publish();
                    }
                }
            }
        }
        public static void CreateAlbumFeed(Item album)
        {
            using (new SecurityDisabler())
            {
                IEnumerable<Item> source =
                    from x in album.Children
                    where x.TemplateID.ToString().Equals(PhotoAlbumConstants.FeedTemplateId)
                    select x;
                if (source.Count<Item>() == 0)
                {
                    TemplateItem templateItem = Context.Database.Templates[PhotoAlbumConstants.FeedTemplateId];
                    if (templateItem != null)
                    {
                        Item item = album.Add("AlbumFeed", new TemplateID(templateItem.ID));
                        item.Editing.BeginEdit();
                        item["Title"] = "Album Feed";
                        item["Source"] = "query:..//*[@@templatename=\"Photo\"]";
                        item.Editing.EndEdit();
                        Database database = Factory.GetDatabase("master");
                        Database database2 = Factory.GetDatabase("web");
                        var publisher = new Publisher(new PublishOptions(database, database2, PublishMode.SingleItem, Context.Language, DateTime.Now)
                        {
                            RootItem = item
                        });
                        publisher.Publish();
                    }
                }
            }
        }
    }
}