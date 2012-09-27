using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Security.Accounts;
using Sitecore.Data;

namespace Sitecore.Modules.PhotoAlbum.BusinessLayer
{
    public class PhotoAlbumCore
    {
        public PhotoAlbumCore() { }

        public  static string GetAlbumItemId()
        {
          var id = string.Empty;
          var currentAlbumItem = (Item)HttpContext.Current.Session["CurrentAlbum"];
          if (currentAlbumItem != null) id = currentAlbumItem.ID.ToString();
          return id;
        }


      public static string GetFeedUrl(string albumId, string currentUserId)
        {
            var feedUrl = string.Empty;
            if (!string.IsNullOrEmpty(currentUserId))
            {
                var item = Sitecore.Context.Database.GetItem(currentUserId);
                if (item != null)
                {
                    var sourceAlbums =from x in item.Children
                                      where x.TemplateID.ToString().Equals(PhotoAlbumConstants.FeedTemplateId)
                                      select x;
                    if (sourceAlbums.Count<Item>() > 0)
                            feedUrl = LinkManager.GetItemUrl(sourceAlbums.First<Item>());
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(albumId))
                {
                    Item item2 = Sitecore.Context.Database.GetItem(albumId);
                    if (item2 != null)
                    {
                        var sourceAlbum =from x in item2.Children
                                         where x.TemplateID.ToString().Equals(PhotoAlbumConstants.FeedTemplateId)
                                         select x;
                        if (sourceAlbum.Count<Item>() > 0)
                        {
                            feedUrl = LinkManager.GetItemUrl(sourceAlbum.First<Item>());
                        }
                    }
                }
            }
            return feedUrl.ToString();
        }


        public string GetCurrentUserName()
        {
            return Sitecore.Security.Accounts.User.Current.LocalName;
        }

        public static string GetRotatorId()
        {
            return Guid.NewGuid().ToString().Replace("{", "").Replace("}", "");
        }

        public static string MetaDataQs(string id)
        {
            Item item = Context.Database.GetItem(id);
            if (item == null)
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder();
            //TrackingField trackingField = new TrackingField(item.Fields["__Tracking"]);
            //foreach (TrackingFieldProfile current2 in trackingField.Profiles)
            //{
            //    if (current2.InnerProfile != null)
            //    {
            //        string text = current2.Keys.Aggregate(string.Empty, (string current, TrackingFieldProfileKey pkey) => current + ((pkey.InnerKey == null) ? string.Empty : (new ID(pkey.InnerKey.ProfileKeyDefinitionId).ToString() + "|")));
            //        if (text.Length > 0)
            //        {
            //            text = text.Substring(0, text.Length - 1);
            //        }
            //        stringBuilder.Append(current2.InnerProfile.Name + "=");
            //        stringBuilder.Append(text);
            //        stringBuilder.Append("&");
            //    }
            //}
            if (stringBuilder.Length <= 0)
            {
                return string.Empty;
            }
            return stringBuilder.ToString(0, stringBuilder.Length - 1);
        }

        public static string GetUserImage(string login)
        {
            string userName = @"extranet\" + login;
            var user = User.FromName(userName, true);
            if (user != null)
            {
                var picture = user.Profile.GetCustomProperty("fb_ProfileImage");
                if (string.IsNullOrEmpty(picture))
                {
                    picture = @"/~/icon/" + user.Profile.Portrait.Replace("16x16", "32x32");
                }
                return picture;
            }

            return string.Empty;
        }

        public static string GetFullUserName(string login)
        {
            string userName = @"extranet\" + login;
            User usr = User.FromName(userName, true);
            string fullName = usr.Profile.FullName;
            if (string.IsNullOrEmpty(fullName))
            {
                fullName = login;
            }

            return fullName;
        }

        public static string GetUserId(string login)
        {
            string userName = @"extranet\" + login;
            var usr = User.FromName(userName, true);
            if (usr == null)
                return string.Empty;
            else
                if (usr.Profile.ProfileItemId == null)
                    return string.Empty;
                else
                    return usr.Profile.ProfileItemId;
        }
        public static string GetAlbumUser(string id)
        {
            ID itemId = new ID(id);
            Sitecore.Data.Items.Item item = Sitecore.Configuration.Factory.GetDatabase(PhotoAlbumConstants.dbName).GetItem(itemId);
            if (item != null)
            {
                string[] pathComponents = item.Paths.ContentPath.Split('/');
                return pathComponents[pathComponents.Length - 2];
            }
            return string.Empty;
        }

        public string GetRootPhotoAlbumId()
        {
            return PhotoAlbumConstants.rootPhotoAlbumID;
        }

        public string GetAlbumTemplateID()
        {
            return PhotoAlbumConstants.albumTemplateID;
        }

        public string GetPhotoTemplateID()
        {
            return PhotoAlbumConstants.photoTemplateID;
        }

        public string GetViewAlbumPageID()
        {
            return Sitecore.Configuration.Settings.GetSetting("detailAlbumPageID");
        }

        public string GetAlbumsOverviewPageID()
        {
            return PhotoAlbumConstants.albumsOverviewPageID;
        }

        public string ReturnToAlbum()
        {
            return PhotoAlbumObject.GetItemPath(Sitecore.Configuration.Settings.GetSetting("detailAlbumPageID"))+"?CurrentAlbum=" + GetAlbumItemId() + "&CurrentUser=" + HttpContext.Current.Session["CurrentUser"];
        }

        public string GetAlbumsInPage()
        {
          return Sitecore.Configuration.Settings.GetSetting("albumsInPage");
        }

        public string GetAlbumMaxPages()
        {
          return Sitecore.Configuration.Settings.GetSetting("albumMaxPages");
        }
    }
}