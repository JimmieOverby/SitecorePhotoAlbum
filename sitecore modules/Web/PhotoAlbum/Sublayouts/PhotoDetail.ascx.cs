using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Modules.PhotoAlbum.BusinessLayer;
using Sitecore.Modules.PhotoAlbum.sitecore_modules.Web.PhotoAlbum.BusinessLayer;
using Sitecore.Web.UI.WebControls;
using ImageField = System.Web.UI.WebControls.ImageField;

namespace Sitecore.Modules.PhotoAlbum.Sublayouts
{
  public partial class PhotoDetail : System.Web.UI.UserControl
  {
    /// <summary>
    /// The CurrentAlbum ID
    /// </summary>   
    private static string CurrentPhotoId { get; set; }

    /// <summary>
    /// The CurrentUser name
    /// </summary>   
    private static string CurrentUser { get; set; }

    /// <summary>
    /// The curr page item
    /// </summary>
    public Item currPageItem;

    /// <summary>
    /// The album class instance
    /// </summary>
    private PhotoAlbumObject albumInst;

    /// <summary>
    /// The current photo item
    /// </summary>
    public Item currPhoto;

    protected void Page_Load(object sender, EventArgs e)
    {
      CurrentUser = Request.QueryString["CurrentUser"];
      CurrentPhotoId = Request.QueryString["photo_id"];
      if (string.IsNullOrEmpty(CurrentUser) || string.IsNullOrEmpty(CurrentPhotoId))
      {
        photoalbum.Visible = false;
        Response.Write("<p width='100%' align='center'>\"This page requires specific parameters and can only be reached from a Album Detail page\"</p>");
        return;
      }

      this.currPageItem = Sitecore.Context.Item;
      XslFile1.Parameters = String.Format(CultureInfo.CurrentCulture, "username={0}", CurrentUser);
      this.currPhoto = PhotoAlbumObject.RestoreItemFromQueryString(CurrentPhotoId);

      this.ImagesContent.InnerHtml = this.BuildImagesHtml(this.currPhoto);
      this.albumInst = new PhotoAlbumObject(null)
                         {
                           CurrentAlbumItem = (Item) HttpContext.Current.Session["CurrentAlbum"]
                         };
      // Generate album view link
      this.ReturnToAlbumLink.HRef = PhotoAlbumObject.GetItemPath(Sitecore.Configuration.Settings.GetSetting("detailAlbumPageID")) + "?CurrentAlbum=" + this.albumInst.CurrentAlbumItem.ID.ToString() + "&CurrentUser=" + CurrentUser;
      this.ShowReviews();

      this.HideReviewFormForOwner();

      if (PhotoRateClicked())
      {
        this.PostReview();
        Response.Redirect(Request.RawUrl);
      }
      EditZoomPicture();

    }

    /// <summary>
    /// Change of image to zoom
    /// </summary> 
    public void EditZoomPicture()
    {
      using (new Sitecore.SecurityModel.SecurityDisabler())
      {
        if (Sitecore.Context.Database == null)
          return;
        Database db = Sitecore.Context.Database;
        Item item = db.Items["/sitecore/content/Globals/Spots/PhotoDetailZoom"]; 
        if (this.currPhoto.Fields["image"] != null)
        {
          try
          {
            item.Editing.BeginEdit();
            item.Fields["picture_small"].Value = this.currPhoto.Fields["image"].Value;
          }
          finally
          {
            item.Editing.EndEdit();
          }
        }
      }
    }

    /// <summary>
    /// Posts the review.
    /// </summary>
    private void PostReview()
    {
      string s1 = PhotoAlbumReviewHelper.SafeRequest("photo_rate_title");
      string s2 = PhotoAlbumReviewHelper.SafeRequest("photo_rate_text");
      string s3 = PhotoAlbumReviewHelper.SafeRequest("photo_rate_rate");
      Item photoForRating = this.currPhoto;// this.paginator.GetPageItems()[0];
      if (photoForRating != null)
      {
        PhotoAlbumObject.AddPhotoRatingComment(s1, s2, s3, photoForRating);
        PhotoAlbumHelper.AddPhotoToSessionNonRateable(photoForRating.ID.ToString(), Request.QueryString["CurrentUser"]);//this.albumInst.CurrUserName);
      }
      // Triger goal Rate Photo
      //  AnalyticsTrackerHelper.TriggerEvent("Rate photos", "Photo was rated", string.Empty, string.Empty, "CurrentPage");
    }

    /// <summary>
    /// Photoes the rate clicked.
    /// </summary>
    /// <returns>The rate clicked.</returns>
    private static bool PhotoRateClicked()
    {
      /* // Analytic
       AnalyticsTracker analyticsTracker = AnalyticsTracker.Current;
       if (analyticsTracker != null && analyticsTracker.CurrentPage != null)
       {
           analyticsTracker.TriggerEvent("Rate photo", "User rated a photo");
       }*/
      return PhotoAlbumReviewHelper.SafeRequest("PhotoRateButton").Equals("PhotoRateButton");
    }

    /// <summary>
    /// Shows the reviews.
    /// </summary>
    private void ShowReviews()
    {
      this.reviewList.InnerHtml = PhotoAlbumReviewHelper.BuildReviewsHtml(this.currPhoto);//this.paginator.GetPageItems()[0]);
    }

    /// <summary>
    /// Builds the images HTML.
    /// </summary>
    /// <param name="itm">The source itm.</param>
    /// <returns>The images HTML.</returns>
    private string BuildImagesHtml(Item itm)
    {
      StringBuilder htmlList = new StringBuilder();
      htmlList.Append("<div class=\"title\">");
      htmlList.Append(itm.Fields["Title"].Value);
      htmlList.Append("</div>");
      XmlField xmlField = itm.Fields["image"];
      if (xmlField != null)
      {
        var strUrl = string.Concat("/~/", xmlField.GetAttribute("src"), "?w=1024&h=768&as=1&force=1");
        this.downLoadAsWallpaper.Attributes.Add("href", strUrl);
      }
      return htmlList.ToString();
    }

    /// <summary>
    /// Hides the review form for owner.
    /// </summary>
    private void HideReviewFormForOwner()
    {
      // If current user is owner of this album then he can't rate it                    
      this.reviewForm.Visible = !PhotoAlbumReviewHelper.IsCurrentUserOwner(this.albumInst, this);
    }

  }
}