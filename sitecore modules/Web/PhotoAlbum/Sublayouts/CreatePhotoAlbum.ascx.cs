using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Data.Items;
using Sitecore.Modules.PhotoAlbum.BusinessLayer;
using Sitecore.Modules.PhotoAlbum.BusinessLayer.FeedHelperClass;



namespace Sitecore.Modules.PhotoAlbum.Sublayouts
{
    public partial class CreatePhotoAlbum : System.Web.UI.UserControl
    {
        public Item currPageItem;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Text = Sitecore.Globalization.Translate.Text("Album name");
            CreateNUpload.Text = Sitecore.Globalization.Translate.Text("Create and Upload");
            
            currPageItem = Sitecore.Context.Item;
        }

        /// <summary>
        /// Handles the Create N upload_ click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void CreateNUpload_Click(object sender, EventArgs e)
        {
            var album = new PhotoAlbumObject(AlbumName.Text.TrimEnd(' '));
            ErrMsg.Visible = false;
            if (AlbumName.Text.TrimEnd(' ').Length >= 20)
            {
                ErrMsg.Text = "The length of album name has to be less then 20 characters.";
                ErrMsg.Visible = true;
            }
            else
            {
                if (!album.CreatePhotoAlbumItem(AlbumName.Text.TrimEnd(' '), string.Empty))
                {
                    if (!string.IsNullOrEmpty(album.PageErrorMsg))
                    {
                        ErrMsg.Text = album.PageErrorMsg;
                        ErrMsg.Visible = true;
                    }
                    // check message property in class and show this message on registration page
                }
                else
                {
                    FeedHelper.CreateUserAlbumsFeed(album.CurrentAlbumItem);
                    FeedHelper.CreateAlbumFeed(album.CurrentAlbumItem);
                    // redirect to upload page
                    PhotoAlbumObject.RedirectToAlbumPage(Sitecore.Configuration.Settings.GetSetting("uploadPhotosPageItemID"), album.CurrentAlbumItem.ID.ToString());
                }
            }
        }
    }
}