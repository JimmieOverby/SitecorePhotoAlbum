using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Security.Accounts;
using Sitecore.Web.UI.WebControls;

namespace Sitecore.Modules.PhotoAlbum.BusinessLayer
{
    /// <summary>
    /// Defines the photo album review helper class.
    /// </summary>
    public class PhotoAlbumReviewHelper
    {
        /// <summary>
        /// Builds the review HTML.
        /// </summary>
        /// <param name="itm">The source itm.</param>
        /// <returns>The review HTML.</returns>
        public static string BuildReviewsHtml(Item itm)
        {
            StringBuilder htmlReview = new StringBuilder();

            var reviews = from r in new List<Item>(itm.Children).Where(i => string.Equals(i.TemplateID.ToString(), PhotoAlbumReviewConstants.ReviewTemplateId))
                          select r;

            // Display reviews
            if (reviews.Count() > 0)
            {
                htmlReview.Append("<div class=\"comment-row\">");
                foreach (Item row in reviews)
                {
                    htmlReview.Append("<div class=\"rated\">");
                    htmlReview.Append(row.Fields["Rate"].Value + " ");
                    htmlReview.Append(Translate.Text("out of") + " 5");
                    htmlReview.Append("<div class=\"stars\">");
                    string ratingHtml = string.Format("<div class=\"current\" style=\"width: {0}em;\"></div>", row.Fields["Rate"].Value);
                    htmlReview.Append(ratingHtml);
                    htmlReview.Append("</div>");
                    htmlReview.Append("</div>");

                    htmlReview.Append("<div class=\"comment\">");
                    htmlReview.Append("<div class=\"title\">");
                    htmlReview.Append(FieldRenderer.Render(row, "Title"));
                    htmlReview.Append("</div>");
                    htmlReview.Append("<div class=\"author\">");
                    var login = row.Fields["User"].Value.Replace(@"extranet\", string.Empty);
                    string userName = @"extranet\" + login;
                    User user = User.FromName(userName, true);
                    string profileName = (user != null && user.Profile != null) ? user.Profile.FullName : login;

                    var profileLink = string.Concat(@"/My_Nicam/Other_user_profile.aspx?username=", row.Fields["User"].Value);
                    string authorLinkTag = string.Format("<a href=\"{0}\">{1}</a>", profileLink, profileName);
                    htmlReview.Append("By " + authorLinkTag);
                    htmlReview.Append("</div>");
                    htmlReview.Append(FieldRenderer.Render(row, "Description"));
                    htmlReview.Append("</div>");

                    htmlReview.Append("<div class=\"clear\"></div>");
                }

                htmlReview.Append("</div>");
            }

            return htmlReview.ToString();

        }

        /// <summary>
        /// Determines whether [is current user owner].
        /// </summary>
        /// <param name="albumInst">The album inst.</param>
        /// <param name="owner">The owner.</param>
        /// <returns>
        /// <c>true</c> if [is current user owner]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCurrentUserOwner(PhotoAlbumObject albumInst, object owner)
        {
            try
            {
                bool result = false;
                string currUser = PhotoAlbumObject.ParseUserName(Sitecore.Context.User.Profile.UserName);
                string albumOwner = (string)HttpContext.Current.Session["CurrentUser"];
                if (currUser == albumOwner)
                {
                    result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("Cannot compare current user and current photo album creator", ex, owner);
                return false;
            }

        }
        /// <summary>
        /// Gets the specific object from current HTTP request.
        /// </summary>
        /// <param name="name">The specific object name.</param>
        /// <returns>
        /// The spesific object from the current HTTP request.
        /// </returns>
        public static string SafeRequest(string name)
        {
            return HttpContext.Current.Request[name] ?? string.Empty;
        }
    }
}