using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Modules.PhotoAlbum.BusinessLayer
{
    public class PhotoAlbumConstants
    {

        public static readonly string rootPhotoAlbumID = "{F4B89D05-8899-4434-A13A-24F365EDF8FB}";

        public static readonly string albumTemplateID = "{B2974916-C25F-46B3-A091-1FC6AA939C93}";

        public static readonly string photoTemplateID = "{DE2CA30E-6550-4037-B57F-4A696A877168}";

        public static readonly string dbName = "master";

        public static readonly string albumsOverviewPageID = "{}";

        public static readonly string folderTemplateID = "{FE5DD826-48C6-436D-B87A-7C4210C7413B}";

        public static readonly string albumFolderName = "PhotoAlbums";

        public static readonly string[] allowedMimeTypes = { "image/jpeg", "application/x-zip-compressed", "application/zip", "image/pjpeg", "image/jpg" };

        // Media gallery settings
        public static readonly string photoAlbumMediaRootID = "{C877273B-B26C-468C-899C-058DD7B991C3}";

        public static readonly string photoAlbumMediaFolderTemplatePath = "/sitecore/templates/System/Media/Media folder";

        // Comments template Id
        public static readonly string commentTemplateID = "{920DDEEC-D5FE-4F21-8C78-A209F63367FE}";


        public static string FeedTemplateId = "{B960CBE4-381F-4A2B-9F44-A43C7A991A0B}";

    }
}