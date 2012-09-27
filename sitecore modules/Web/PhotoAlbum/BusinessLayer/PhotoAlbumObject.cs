using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Pipelines.Upload;
using Sitecore.Publishing;
using Sitecore.SecurityModel;
using Sitecore.Web;
using ImageField = Sitecore.Data.Fields.ImageField;
using Sitecore.Pipelines;

namespace Sitecore.Modules.PhotoAlbum.BusinessLayer
{
    public class PhotoAlbumObject
    {
        #region Fields

        #endregion Fields

        #region Private properties

        public string DbName { get; set; }

        private Item FolderTemplate { get; set; }

        private Item MediaRootItem { get; set; }

        private Item MeadiaFolderTemplate { get; set; }

        private Item AlbumTemplate { get; set; }

        #endregion

        #region Public properties

        public Item CurrentAlbumItem { get; set; }

        public string PageErrorMsg { get; set; }

        public string CurrUserName { get; set; }

        public Item CurrentPhotoAlbumFolder { get; set; }

        public Item CurrentMediaFolder { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PhotoAlbum"/> class.
        /// </summary>
        public PhotoAlbumObject(string currentFolderAlbum)
        {
            try
            {
                InitProperties();
                InitCurrentPhotoAlbumFolder();
                InitCurrentMediaFolders(currentFolderAlbum);

                // If session has CurrentAlbum we will set appropriate property                
                /*  if (HttpContext.Current.Session["CurrentAlbum"] != null)
                  {
                      this.CurrentAlbumItem = (Item)HttpContext.Current.Session["CurrentAlbum"];
                  }*/
            }
            catch (Exception ex)
            {
                Log.Error("Cannot get user settings root item", ex, this);
            }
        }

        private void InitProperties()
        {
            DbName = PhotoAlbumConstants.dbName;

            /*** UserName ***/
            CurrUserName = ParseUserName(Context.User.Profile.UserName);

            /*** Photo Album Item ***/
            CurrentAlbumItem = Factory.GetDatabase(DbName).Items[PhotoAlbumConstants.rootPhotoAlbumID];
            FolderTemplate = Factory.GetDatabase(DbName).Items[PhotoAlbumConstants.folderTemplateID];

            /*** Album ***/
            AlbumTemplate = Factory.GetDatabase(DbName).Items[PhotoAlbumConstants.albumTemplateID];

            //*** Media Folder ***/
            MediaRootItem = Factory.GetDatabase(DbName).Items[PhotoAlbumConstants.photoAlbumMediaRootID];
            MeadiaFolderTemplate = Factory.GetDatabase(DbName).Items[PhotoAlbumConstants.photoAlbumMediaFolderTemplatePath];

        }

        private void InitCurrentMediaFolders(string mediaAlbum)
        {
          if (mediaAlbum != null)
          {
            // Try navigate to current user's photo album media folder item
            Item navMediaTmp = this.MediaRootItem.Children[this.CurrUserName.Substring(0, 1).ToLower()];
            this.CurrentMediaFolder = null;
            if (navMediaTmp != null)
            {
              navMediaTmp = navMediaTmp.Children[this.CurrUserName];
              if (navMediaTmp == null) return;
              navMediaTmp = navMediaTmp.Children[mediaAlbum];
              if (navMediaTmp != null)
              {
                this.CurrentMediaFolder = navMediaTmp;
              }
            }
          }
        }

        private void InitCurrentPhotoAlbumFolder()
        {
            // Try navigate to current user's photo albums
            Item navTmp = this.CurrentAlbumItem.Children[this.CurrUserName.Substring(0, 1).ToLower()];
            this.CurrentPhotoAlbumFolder = null;
            if (navTmp == null) return;
            navTmp = navTmp.Children[this.CurrUserName];
            if (navTmp != null)
            {
                this.CurrentPhotoAlbumFolder = navTmp;
            }
        }

        /// <summary>
        /// Creates the photo album item.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        public bool CreatePhotoAlbumItem(string name, string description)
        {
            try
            {
                bool success = true;
                Item currentAlbumItem = CurrentAlbumItem;
                Item curentMediaFolderItem = MediaRootItem;

                using (new Sitecore.SecurityModel.SecurityDisabler())
                {
                    /*** Level 1 (letter name folder) ***/
                    var letterItemFolderName = CheckExistingItemName(currentAlbumItem, getFirstLetter(CurrUserName), true);
                    if (letterItemFolderName != string.Empty)
                        CreateItemSimple(letterItemFolderName, currentAlbumItem, FolderTemplate);

                    var letterMediaFolderName = CheckExistingItemName(curentMediaFolderItem, getFirstLetter(CurrUserName), true);
                    if (letterMediaFolderName != string.Empty)
                        CreateItemSimple(letterMediaFolderName, curentMediaFolderItem, MeadiaFolderTemplate);

                    /*** Level 2 (username name folder) ***/
                    currentAlbumItem = currentAlbumItem.Children[getFirstLetter(CurrUserName)];
                    string userNameFolderName = CheckExistingItemName(currentAlbumItem, CurrUserName, true);
                    if (userNameFolderName != string.Empty)
                    {
                        CreateItemSimple(userNameFolderName, currentAlbumItem, FolderTemplate);
                        Item mediaLetterItem = CreateItemSimple(userNameFolderName, MediaRootItem.Children[getFirstLetter(CurrUserName)], MeadiaFolderTemplate);
                        CurrentMediaFolder = mediaLetterItem;
                    }
                    currentAlbumItem = currentAlbumItem.Children[CurrUserName];

                    /*** Level 3 (Photo Album name folder) ***/
                    curentMediaFolderItem = MediaRootItem.Children[getFirstLetter(CurrUserName)].Children[CurrUserName];
                    string albumNameFolderName = CheckExistingItemName(curentMediaFolderItem, name, true);
                    if (albumNameFolderName != string.Empty)
                    {
                        Item mediaAlbumNameItem = CreateItemSimple(albumNameFolderName, curentMediaFolderItem, MeadiaFolderTemplate);
                        CurrentMediaFolder = mediaAlbumNameItem;
                    }

                    Hashtable fields = new Hashtable();
                    fields.Add("Name", name);
                    fields.Add("Description", description);

                    CurrentAlbumItem = CreateAndFillItem(name, fields, currentAlbumItem, AlbumTemplate, true);

                    if (CurrentAlbumItem == null)
                    {
                        success = false;
                    }
                }

                RegisterItemInSession("CurrentAlbum", CurrentAlbumItem);
                return success;
            }
            catch (Exception ex)
            {
                Log.Error("Cannot create album item", ex, this);
                return false;
            }
        }

        private string getFirstLetter(string value)
        {
            return value.Substring(0, 1).ToLower();
        }

        public static string ParseUserName(string userNameWithDomain)
        {
            string[] array = userNameWithDomain.Split(new char[] { '\\' });
            return array[1];
        }


        private void RegisterItemInSession(string index, Item item)
        {
            HttpContext.Current.Session[index] = item;
        }

        private Item CreateItemSimple(string itemName, Item parentItem, Item itemTemplate)
        {
            Item result;
            try
            {
                using (new SecurityDisabler())
                {
                    Item item = parentItem.Add(itemName, new TemplateID(itemTemplate.ID));
                    PhotoAlbumObject.PublishOneItem(item);
                    result = item;
                }
            }
            catch (Exception exception)
            {
                Log.Error("Cannot create item", exception, this);
                result = null;
            }
            return result;
        }

        private string CheckExistingItemName(Item itemFolder, string itemName, bool nameExact)
        {
            string text = itemName;
            int num = 1;
            bool flag;
            do
            {
                flag = true;
                foreach (Item item in itemFolder.Children)
                {
                    if (item.Name == text)
                    {
                        if (nameExact)
                        {
                            return string.Empty;
                        }
                        flag = false;
                        string str = num.ToString();
                        num++;
                        text = itemName + " " + str;
                    }
                }
            }
            while (!flag);
            return text;
        }

        private static void PublishOneItem(Item item)
        {
            Language language = Context.Language;
            Database database = Factory.GetDatabase("master");
            Database database2 = Factory.GetDatabase("web");
            Publisher publisher = new Publisher(new PublishOptions(database, database2, PublishMode.SingleItem, language, DateTime.Now)
            {
                RootItem = item,
                Deep = true,
                RepublishAll = true
            });
            publisher.Publish();
        }

        private Item CreateAndFillItem(string itemName, Hashtable itemFieldsData, Item destionationItem, Item itemTemplate, bool nameExact)
        {
            Item result;
            using (new SecurityDisabler())
            {
                try
                {
                    string text = this.CheckExistingItemName(destionationItem, itemName, nameExact);
                    if (nameExact && text == string.Empty)
                    {
                        if (itemName == string.Empty)
                        {
                            this.PageErrorMsg = "Enter album name, please";
                            result = null;
                        }
                        else
                        {
                            this.PageErrorMsg = "The album with name \"" + itemName + "\" already exists";
                            result = null;
                        }

                    }
                    else
                    {
                        Item item = destionationItem.Add(text, new TemplateID(itemTemplate.ID));
                        Language[] languages = item.Languages;
                        for (int i = 0; i < languages.Length; i++)
                        {
                            Language language = languages[i];
                            Item item2 = item.Database.GetItem(item.ID, language);
                            if (item2.Versions.Count == 0)
                            {
                                item2 = item2.Versions.AddVersion();
                            }
                            item2.Editing.BeginEdit();
                            foreach (string text2 in itemFieldsData.Keys)
                            {
                                string type;
                                if ((type = item2.Fields[text2].Type) != null && type == "Image")
                                {
                                    if (itemFieldsData[text2] is Item)
                                    {
                                        Item item3 = (Item)itemFieldsData[text2];
                                        MediaItem mediaItem = item3;
                                        ImageField imageField = item2.Fields[text2];
                                        imageField.Src = "~/media" + mediaItem.MediaPath + ".ashx";
                                        imageField.MediaPath = mediaItem.MediaPath;
                                        imageField.MediaID = mediaItem.ID;
                                        imageField.Alt = "photo image";
                                    }
                                }
                                else
                                {
                                    item2[text2] = (string)itemFieldsData[text2];
                                }
                            }
                            item2.Editing.EndEdit();
                        }
                        PhotoAlbumObject.PublishOneItem(item);
                        result = item;
                    }
                }
                catch (Exception exception)
                {
                    Log.Error("Cannot create or edit the item. Possible there are invalid fieds collection", exception, this);
                    result = null;
                }
            }
            return result;
        }

        public static Item RestoreItemFromQueryString(string itemID)
        {
            Item result;
            try
            {
                result = Factory.GetDatabase(PhotoAlbumConstants.dbName).Items[itemID];
            }
            catch (Exception exception)
            {
                Log.Error("Cannot restore item from query string by item ID", exception, "RestoreItemFromQueryString");
                result = null;
            }
            return result;
        }
        public bool UploadNCreate(FileUpload fileUpload, HttpFileCollection files, string albumID)
        {
            bool flag = true;
            bool result;
            try
            {
                if (fileUpload.PostedFile != null && fileUpload.PostedFile.ContentLength != 0)
                {
                    if (string.IsNullOrEmpty(albumID))
                    {
                        this.PageErrorMsg = "You should choose album for uploading";
                        result = false;
                        return result;
                    }
                    if (!this.IsMimeTypeAllowed(fileUpload.PostedFile.ContentType, PhotoAlbumConstants.allowedMimeTypes))
                    {
                        this.PageErrorMsg = "You should upload only jpeg, jpg images or zip archives";
                        result = false;
                        return result;
                    }
                    using (new SecurityDisabler())
                    {
                        Item item = this.CurrentMediaFolder;
                        UploadArgs uploadArgs = new UploadArgs();
                        uploadArgs.Files = files;
                        uploadArgs.Folder = item.Paths.Path;
                        uploadArgs.Overwrite = false;
                        uploadArgs.Unpack = true;
                        uploadArgs.Versioned = false;
                        uploadArgs.Language = Context.Language;
                        uploadArgs.Destination = UploadDestination.Database;
                        Context.SetActiveSite("shell");

                        try
                        {
                            if (PipelineFactory.GetPipeline("uiUpload") != null && PipelineFactory.GetPipeline("uiUpload").Processors != null)
                            {
                                foreach (Processor p in from Processor p in PipelineFactory.GetPipeline("uiUpload").Processors where p.Name.Equals("Sitecore.Pipelines.Upload.Done,Sitecore.Kernel,Process") select p)
                                {
                                    PipelineFactory.GetPipeline("uiUpload").Processors.Remove(p);
                                }
                            }
                        }
                        catch (Exception exception) { Log.Error("Delete from Pipelines ", exception, this); }

                        PipelineFactory.GetPipeline("uiUpload").Start(uploadArgs);

                        if (uploadArgs.UploadedItems.Count == 1)
                        {
                            PhotoAlbumObject.PublishOneItem(uploadArgs.UploadedItems[0]);
                        }
                        else
                        {
                            if (uploadArgs.UploadedItems.Count > 1)
                            {
                                PhotoAlbumObject.PublishOneItem(uploadArgs.UploadedItems[0].Parent);
                            }
                        }
                        foreach (Item current in uploadArgs.UploadedItems)
                        {
                            current.Editing.BeginEdit();
                            if (current.Fields["alt"] != null)
                            {
                                current.Fields["alt"].Value = current.Name;
                            }
                            current.Editing.EndEdit();
                        }
                        Context.SetActiveSite("website");
                        Item item2 = Factory.GetDatabase(this.DbName).Items[albumID];
                        Item itemTemplate = Factory.GetDatabase(this.DbName).Items[PhotoAlbumConstants.photoTemplateID];
                        foreach (Item current2 in uploadArgs.UploadedItems)
                        {
                            string itemName2 = this.CheckExistingItemName(item2, current2.Name, false);
                            this.CreateAndFillItem(itemName2, new Hashtable
					{

						{
							"Title", 
							current2.Name
						}, 

						{
							"image", 
							current2
						}
					}, item2, itemTemplate, false);
                        }
                        this.CurrentAlbumItem = item2;
                        result = flag;
                        return result;
                    }
                }
                this.PageErrorMsg = "You should choose zip archive or image";
                result = false;
            }
            catch (Exception exception)
            {
                Log.Error("Cannot upload image(s)", exception, this);
                result = false;
            }
            return result;
        }

        private bool IsMimeTypeAllowed(string type, string[] allowedTypes)
        {
            bool result = false;
            for (int i = 0; i < allowedTypes.Length; i++)
            {
                string a = allowedTypes[i];
                if (a == type)
                {
                    result = true;
                }
            }
            return result;
        }


        /// <summary>
        /// Adds the photo rating comment.
        /// </summary>
        /// <param name="review_title">The review_title.</param>
        /// <param name="review_text">The review_text.</param>
        /// <param name="review_rate">The review_rate.</param>
        /// <param name="folderItm">The folder itm.</param>
        public static void AddPhotoRatingComment(string review_title, string review_text, string review_rate, Item folderItm)
        {
            using (new SecurityDisabler())
            {
                Item currentItm = folderItm;
                Database masterDb = Factory.GetDatabase("master");

                if (currentItm != null && masterDb != null)
                {

                    //------------------------------------------
                    //            validation
                    //------------------------------------------
                    bool reviewValidate = false;
                    if (!review_title.Equals(string.Empty) &&
                        !review_text.Equals(string.Empty) &&
                        !review_rate.Equals(string.Empty))
                    {
                        reviewValidate = true;
                    }

                    if (reviewValidate)
                    {
                        Item reviewsFolderItm = masterDb.Items[currentItm.Paths.FullPath];
                        Database webDB = Factory.GetDatabase("web");

                        if (reviewsFolderItm != null)
                        {
                            //------------------------------------------
                            //     create name from date and time
                            //------------------------------------------
                            string reviewName = DateUtil.ToIsoDate(DateTime.Now, false);

                            //------------------------------------------
                            //     create item from template
                            //------------------------------------------
                            string templateID = PhotoAlbumConstants.commentTemplateID;
                            TemplateItem reviewTemplate = masterDb.Templates[templateID];
                            if (reviewTemplate != null)
                            {
                                Item itm = reviewsFolderItm.Add(reviewName, reviewTemplate);
                                foreach (Language ln in itm.Languages)
                                {
                                    Item currentLnCreated = itm.Database.GetItem(itm.ID, ln);
                                    if (currentLnCreated.Versions.Count == 0)
                                    {
                                        currentLnCreated = currentLnCreated.Versions.AddVersion();
                                    }

                                    //------------------------------------------
                                    //         fill review item fields
                                    //------------------------------------------
                                    using (new EditContext(currentLnCreated, true, false))
                                    {
                                        currentLnCreated.Fields["Title"].Value = review_title;
                                        currentLnCreated.Fields["Description"].Value = review_text;
                                        currentLnCreated.Fields["user"].Value = Context.User.Name;
                                        currentLnCreated.Fields["Rate"].Value = review_rate;
                                    }

                                    itm.Locking.Unlock();
                                }

                                //------------------------------------------
                                //            publish item
                                //------------------------------------------
                                PublishOneItem(itm.Parent);
                            }
                        }
                    }

                }
            }
        }

        public List<Item> GetAlbumPhotos()
        {
            return this.CurrentAlbumItem.Children.Where(i => string.Equals(i.TemplateID.ToString(), PhotoAlbumConstants.photoTemplateID)).ToList();
        }

        public static string GetItemPath(string id)
        {
            try
            {
                Item item = Context.Database.Items[id];
                return Sitecore.Links.LinkManager.GetItemUrl(item);
            }
            catch (Exception ex)
            {
                Log.Error("Cannot get item path", ex, "GetItemPath");
                return string.Empty;
            }
        }

        public static string GetItemName(string id)
        {
            try
            {
                Item item = Context.Database.Items[id];
                return item.Name;
            }
            catch (Exception ex)
            {
                Log.Error("Cannot get item name", ex, "GetItemName");
                return string.Empty;
            }
        }

        public static void RedirectToAlbumPage(string itemPageID, string CurrentAlbum)
        {
            try
            {
                Item item = Context.Database.Items[itemPageID];
                string itemUrl = LinkManager.GetItemUrl(item) + "?CurrentAlbum=" + CurrentAlbum;
                WebUtil.Redirect(itemUrl);
            }
            catch (Exception exception)
            {
                Log.Error("Cannot redirect to item page", exception, "RedirectToAlbumPage");
            }
        }
    }
}