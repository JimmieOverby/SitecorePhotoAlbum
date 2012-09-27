using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Collections;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Modules.PhotoAlbum.BusinessLayer;
using Sitecore.Data;

namespace Sitecore.Modules.PhotoAlbum.Sublayouts
{
    public partial class UploadPhotos : System.Web.UI.UserControl
    {
        public PhotoAlbumObject _albumInst;
        public Item currPageItem;

        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Text = Translate.Text("Album");
            Label2.Text = Translate.Text("Upload");
            SelectNUpload.Text = Translate.Text("Upload");

            currPageItem = Sitecore.Context.Item;

            if (!Page.IsPostBack)
            {
                // If query string variable CurrentAlbum is set then copy it to session
                if (Request.QueryString["CurrentAlbum"] != null)
                {
                    System.Web.HttpContext.Current.Session["CurrentAlbum"] = PhotoAlbumObject.RestoreItemFromQueryString(Request.QueryString["CurrentAlbum"]);
                }
                _albumInst = new PhotoAlbumObject(null);
                if (_albumInst.CurrentPhotoAlbumFolder != null)
                {
                    ChildList albums = _albumInst.CurrentPhotoAlbumFolder.Children;
                    var lstUserAlbums = new List<UserAlbum>();
                    foreach (Item alb in albums.InnerChildren)
                    {
                        if (alb.Name != "AlbumsListFeed")
                            lstUserAlbums.Add(new UserAlbum { Id = alb.ID, Name = alb.Name });
                    }
                    if (albums != null)
                    {
                        AlbumsList.DataSource = lstUserAlbums;
                        AlbumsList.DataTextField = "Name";
                        AlbumsList.DataValueField = "ID";
                        AlbumsList.DataBind();
                        if (Request.QueryString["CurrentAlbum"] != null)
                        {
                            AlbumsList.SelectedValue = Request.QueryString["CurrentAlbum"];
                        }
                        if (_albumInst.CurrentAlbumItem != null)
                        {
                            AlbumsList.SelectedValue = _albumInst.CurrentAlbumItem.Name;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Select N upload_ click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SelectNUpload_Click(object sender, EventArgs e)
        {
            // base.Request.Files
            _albumInst = new PhotoAlbumObject(AlbumsList.SelectedItem.Text);
            ErrorMsg.Visible = false;

            if (!_albumInst.UploadNCreate(FileUpload1, Request.Files, AlbumsList.SelectedValue) && !string.IsNullOrEmpty(_albumInst.PageErrorMsg))
            {
                ErrorMsg.Text = _albumInst.PageErrorMsg;
                ErrorMsg.Visible = true;
            }
            else
            {
                /*  var albumUrl = @"/Photo_album/Custom/View_Album.aspx?CurrentAlbum=" + _albumInst.CurrentAlbumItem.ID;
                  var uriString = string.Format(CultureInfo.InvariantCulture, @"http://{0}/{1}", Request.Url.Host, albumUrl);
                  var shortener = new UrlShortener.BusinessLayer.GoogleUrlShortener();
                  var shortUri = shortener.ShortenUrl(new Uri(uriString));
                  var sharableEvent = new SharableEvent(Sitecore.Context.User, SharableEventType.PhotoUploaded, SharingTarget.All, shortUri, Request.Files[0].FileName);
                  EventsDispatcher.ProcessEvent(sharableEvent);*/

                // Tracking Goal Upload photos
                // AnalyticsTrackerHelper.TriggerEvent("Upload photos", "User has uploaded photos", string.Empty, string.Empty, "CurrentPage");
            }
        }
    }

    public class UserAlbum
    {
        public ID Id { set; get; }
        public string Name { set; get; }
    }

}