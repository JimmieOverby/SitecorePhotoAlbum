using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Modules.PhotoAlbum.sitecore_modules.Web.PhotoAlbum.BusinessLayer
{
    /// <summary>
    /// Defines the photo album helper class.
    /// </summary>
    public class PhotoAlbumHelper
    {
        public static string _rateSessionKeyPrefix = "npaRated_";

        /// <summary>
        /// Adds the photo to session non rateable.
        /// </summary>
        /// <param name="photoId">The photo id.</param>
        /// <param name="userName">Name of the user.</param>
        public static void AddPhotoToSessionNonRateable(string photoId, string userName)
        {
            string sessionKey = string.Concat(_rateSessionKeyPrefix, photoId);
            List<string> userNames = new List<string>();
            if (HttpContext.Current.Session[sessionKey] != null)
            {
                userNames = HttpContext.Current.Session[sessionKey] as List<string>;
            }

            if (userNames == null)
            {
                return;
            }

            userNames.Add(userName.ToLower());
            HttpContext.Current.Session[sessionKey] = userNames;
        }

        /// <summary>
        /// Determines whether [is already rated by user] [the specified photo id].
        /// </summary>
        /// <param name="photoId">The photo id.</param>
        /// <param name="userName">The user name.</param>
        /// <returns>
        /// <c>true</c> if [is already rated by user] [the specified photo id]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAlreadyRatedByUser(string photoId, string userName)
        {
            bool result = false;
            string sessionKey = string.Concat(_rateSessionKeyPrefix, photoId);
            if (HttpContext.Current.Session[sessionKey] != null)
            {
                List<string> userNames = HttpContext.Current.Session[sessionKey] as List<string>;
                var selected = from usr in userNames
                               where usr == userName.ToLower()
                               select usr;

                if (selected.Count() > 0)
                {
                    result = true;
                }
            }

            return result;
        }
    }
}