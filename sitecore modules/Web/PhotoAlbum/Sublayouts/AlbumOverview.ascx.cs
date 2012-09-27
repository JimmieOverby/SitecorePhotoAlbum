namespace Layouts.Albumoverview
{
  using System;
  using System.Data;
  using System.Globalization;
  using System.Linq;
  using Sitecore.Data.Fields;
  using Sitecore.Data.Items;
  using Sitecore.Modules.PhotoAlbum.sitecore_modules.Web.PhotoAlbum.BusinessLayer;
  using Sitecore.Modules.PhotoAlbum.BusinessLayer;

  /// <summary>
  /// Summary description for AlbumoverviewSublayout
  /// </summary>
  public partial class AlbumoverviewSublayout : System.Web.UI.UserControl
  {
    /// <summary>
    /// The current album
    /// </summary>   
    public PhotoAlbumObject album;

    /// <summary>
    /// The album class instance
    /// </summary>
    private PhotoAlbumObject albumInst;

    /// <summary>
    /// The CurrentUser name
    /// </summary>   
    private static string CurrentUser { get; set; }

    /// <summary>
    /// The CurrentAlbum ID
    /// </summary>   
    private static string CurrentAlbum { get; set; }

    private string preSelectPhoto;

    private int photosInPage
    {
      get { return int.Parse(Sitecore.Configuration.Settings.GetSetting("photosInPage")); }
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void Page_Load(object sender, EventArgs e)
    {
      CurrentUser = Request.QueryString["CurrentUser"];
      CurrentAlbum = Request.QueryString["CurrentAlbum"];

      if (string.IsNullOrEmpty(CurrentUser) || string.IsNullOrEmpty(CurrentAlbum))
      {
        photoalbum.Visible = false;
        Response.Write("<p width='100%' align='center'>\"This page requires specific parameters and can only be reached from a All Albums or My Albums pages\"</p>");
        return;

      }

      System.Web.HttpContext.Current.Session["CurrentAlbum"] = PhotoAlbumObject.RestoreItemFromQueryString(CurrentAlbum);
      System.Web.HttpContext.Current.Session["CurrentUser"] = CurrentUser;
      this.album = new PhotoAlbumObject("new Album");
      this.album.CurrentAlbumItem = PhotoAlbumObject.RestoreItemFromQueryString(Request.QueryString["CurrentAlbum"]);
      var thumbnailsDataSource = GenerateDataSource(this.album);
      var pagePosition = 0;
      var i = 0;
      if (!string.IsNullOrEmpty(Request.QueryString["CurrentPhoto"]))
      {
        foreach (var p in this.album.GetAlbumPhotos())
        {
          if (string.Equals(p.ID.ToString(), Request.QueryString["CurrentPhoto"].ToString()))
          {
            pagePosition = i / photosInPage;
          }
          i++;
        }
        if (!string.Equals(preSelectPhoto, Request.QueryString["CurrentPhoto"].ToString()))
        {
          preSelectPhoto = Request.QueryString["CurrentPhoto"];
          dpPhotoAlbum.SetPageProperties(pagePosition * photosInPage, photosInPage, true);
        }
      }

      if (thumbnailsDataSource.Rows.Count <= photosInPage)
        dpPhotoAlbum.Visible = false;

      this.ThumbnailsRepeater.DataSource = thumbnailsDataSource;
      this.ThumbnailsRepeater.DataBind();

      var currentPhotoId = Request.QueryString["CurrentPhoto"];
      var currentPhotoItem = !string.IsNullOrEmpty(currentPhotoId) ? Sitecore.Context.Database.GetItem(currentPhotoId) : this.album.GetAlbumPhotos().FirstOrDefault();

      if (currentPhotoItem != null)
      {
        this.BigPhoto.Text = this.GenerateFirstPhotoHtml(currentPhotoItem);
      }

      this.albumInst = new PhotoAlbumObject(null);
      var albumItm = PhotoAlbumObject.RestoreItemFromQueryString(Request.QueryString["CurrentAlbum"]);
      this.ShowReviews(albumItm);
      this.HideReviewFormForOwner(albumItm);

      if (Page.IsPostBack)
      {
        if (PhotoRateClicked())
        {
          this.PostReview(albumItm);
          Response.Redirect(Request.RawUrl);
        }
      }

      XslFile1.Parameters = String.Format(CultureInfo.CurrentCulture, "username={0}", CurrentUser);
    }

    
    /// <summary>
    /// Generates the data source.
    /// </summary>
    /// <param name="album">The album.</param>
    /// <returns>The data source.</returns>
    private static DataTable GenerateDataSource(PhotoAlbumObject album)
    {
      var table = new DataTable("Thumbnails");
      table.Columns.Add("link");
      table.Columns.Add("srcLink");

      var images = album.GetAlbumPhotos();
      foreach (var item in images)
      {
        //  const string Template = @"/Photo_album/Custom/View_Album.aspx?CurrentAlbum={0}&CurrentPhoto={1}";

        const string Template = @"{0}?CurrentAlbum={1}&CurrentPhoto={2}&CurrentUser={3}";
        var link = string.Format(CultureInfo.InvariantCulture, Template, PhotoAlbumObject.GetItemPath(Sitecore.Configuration.Settings.GetSetting("detailAlbumPageID")),
            album.CurrentAlbumItem.ID, item.ID, CurrentUser);
        var srcLink = GetPhotoSrc(item, 80);

        table.Rows.Add(link, srcLink);
      }

      return table;
    }

    /// <summary>
    /// Gets the photo src.
    /// </summary>
    /// <param name="item">The source item.</param>
    /// <param name="maxWidth">Width of the max.</param>
    /// <returns>The photo src.</returns>
    private static string GetPhotoSrc(Item item, int maxWidth)
    {
      XmlField field = item.Fields["image"];
      var srcLink = field.GetAttribute("src") + "?mw=" + maxWidth + "&mh=" + maxWidth + "&bc=white";

      return srcLink;
    }

    /// <summary>
    /// Gets the photo page link.
    /// </summary>
    /// <param name="item">The photo item.</param>
    /// <returns>The photo page link.</returns>
    private static string GetPhotoPageLink(Item item)
    {
      var link = PhotoAlbumObject.GetItemPath(Sitecore.Configuration.Settings.GetSetting("detailPhotoPageItemID")) + "?photo_id=" + item.ID + "&CurrentUser=" + CurrentUser;
      return link;
    }



    /// <summary>
    /// Generates the first photo HTML.
    /// </summary>
    /// <param name="item">The source item.</param>
    /// <returns>The first photo HTML.</returns>
    private string GenerateFirstPhotoHtml(Item item)
    {
      const string Template = @"
      <a href='{0}'><img class='img-shadow' width='230px' src='{1}'></a>
      <div class='title'>{2}</div>
      <div class='slideshow'><a href='{3}'>{4}</a></div>
      <div class='sub'>{5}</div>";

      var bigPhotoLink = GetPhotoPageLink(item);
      var bigPhotoSrc = GetPhotoSrc(item, 230);
      var count = this.album.GetAlbumPhotos().Count();
      var countText = count == 1 ? string.Format(CultureInfo.InvariantCulture, "{0} photo", count) : string.Format(CultureInfo.InvariantCulture, "{0} photos", count);
      var viewSlider = PhotoAlbumObject.GetItemPath(Sitecore.Configuration.Settings.GetSetting("albumSliderPageItemID")) + "?CurrentAlbum=" + this.album.CurrentAlbumItem.ID + "&CurrentUser=" + CurrentUser;
      var result = string.Format(CultureInfo.InvariantCulture, Template, bigPhotoLink, bigPhotoSrc, item.Name, viewSlider, "View Slideshow", countText);

      return result;
    }

    /// <summary>
    /// Photoes the rate clicked.
    /// </summary>
    /// <returns>The rate clicked.</returns>
    private static bool PhotoRateClicked()
    {
      // Analytic
      /*  AnalyticsTracker analyticsTracker = AnalyticsTracker.Current;
        if (analyticsTracker != null && analyticsTracker.CurrentPage != null)
        {
            analyticsTracker.TriggerEvent("Rate photo", "User rated a photo");
        }*/

      return (PhotoAlbumReviewHelper.SafeRequest("PhotoRateButton").Equals("PhotoRateButton"));
    }

    /// <summary>
    /// Posts the review.
    /// </summary>
    /// <param name="albumForRating">The album for rating.</param>
    private void PostReview(Item albumForRating)
    {
      string s1 = PhotoAlbumReviewHelper.SafeRequest("photo_rate_title");
      string s2 = PhotoAlbumReviewHelper.SafeRequest("photo_rate_text");
      string s3 = PhotoAlbumReviewHelper.SafeRequest("photo_rate_rate");

      if (albumForRating != null)
      {
        PhotoAlbumObject.AddPhotoRatingComment(s1, s2, s3, albumForRating);

        // register session when user already rated image                            
        PhotoAlbumHelper.AddPhotoToSessionNonRateable(albumForRating.ID.ToString(), this.albumInst.CurrUserName);
      }

      // Triger goal Rate Photo
      //AnalyticsTrackerHelper.TriggerEvent("Rate photos", "Album was rated", string.Empty, string.Empty, "CurrentPage");
    }

    /// <summary>
    /// Hides the review form for owner.
    /// </summary>
    /// <param name="albumItm">The album itm.</param>
    private void HideReviewFormForOwner(Item albumItm)
    {
      // If current user is owner of this album then he can't rate it                    
      if (PhotoAlbumReviewHelper.IsCurrentUserOwner(this.albumInst, this) || PhotoAlbumHelper.IsAlreadyRatedByUser(albumItm.ID.ToString(), this.albumInst.CurrUserName))
      {
        this.reviewForm.Visible = false;
      }
      else
      {
        this.reviewForm.Visible = true;
      }
    }

    /// <summary>
    /// Shows the reviews.
    /// </summary>
    /// <param name="albumItm">The album itm.</param>
    private void ShowReviews(Item albumItm)
    {
      this.reviewList.InnerHtml = PhotoAlbumReviewHelper.BuildReviewsHtml(albumItm);
    }
  }
}