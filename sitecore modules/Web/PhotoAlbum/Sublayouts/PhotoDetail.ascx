<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PhotoDetail.ascx.cs"
    Inherits="Sitecore.Modules.PhotoAlbum.Sublayouts.PhotoDetail" %>
<%@ Import Namespace="Sitecore.Globalization" %>
<link rel="Stylesheet" href="/sitecore modules/Web/PhotoAlbum/Styles/common_photoalbum.css" />
<script src="/sitecore modules/Web/PhotoAlbum/js/jquery-1.6.2.min.js"></script>
<script src="/sitecore modules/Web/PhotoAlbum/js/PhotoAlbum.js"></script>
<div id="photoalbum" class="photo-album" runat="server">
    <div id="image-detail">
        <%-- <h1>
            <%= FieldRenderer.Render(currPageItem, "Title")%></h1>--%>
        <h1>
            My Photo Album</h1>
        <sc:XslFile runat="server" Path="/sitecore modules/Web/PhotoAlbum/Renderings/PhotoAlbumUserInfoWrapper.xslt"
            ID="XslFile1"></sc:XslFile>
        <div class="clear">
        </div>
        <div class="return">
            <a href="" id="ReturnToAlbumLink" runat="server">
                <%= Translate.Text("Return to album  ") %></a>
        </div>
        <div id="photo">
        <sc:XslFile runat="server" Path="/xsl/ZoomFlash/ZoomFlash.xslt" DataSource="/sitecore/content/Globals/Spots/PhotoDetailZoom"
            ID="XslFile2"></sc:XslFile>
             
            <div id="ImagesContent" runat="server" class="npaImagesList">
            </div>
            <div class="download">
                <a href="#" class="btn-submit-gray" id="downLoadAsWallpaper" runat="server">DOWNLOAD
                    AS WALLPAPER</a></div>
        </div>
        <div class="clear">
        </div>
        <div id="comment-wrapper">
            <div id="reviewList" runat="server">
            </div>
            <div id="reviewForm" runat="server">
                <h2>
                    <%= Translate.Text("Rate This Image") %></h2>
                <div>
                    <label>
                        Click Stars to rate</label>
                    <div class="rate-stars">
                        <input id="photo_rate_rate" type="hidden" name="photo_rate_rate" value="0" />
                        <ul class="stars">
                            <li><a href="#1" class="rate1" id="rate1" title="I hated it">*</a></li>
                            <li><a href="#2" class="rate2" id="rate2" title="I disliked it">*</a></li>
                            <li><a href="#3" class="rate3" id="rate3" title="It was OK">*</a></li>
                            <li><a href="#4" class="rate4" id="rate4" title="I liked it">*</a></li>
                            <li><a href="#5" class="rate5" id="rate5" title="I loved it">*</a></li>
                        </ul>
                    </div>
                    <input type="hidden" name="stars" value="" />
                    <label>
                        <%= Translate.Text("Title")%>:</label><input type="text" name="photo_rate_title"
                            value="" />
                    <label>
                        <%= Translate.Text("Comment")%>:</label><textarea name="photo_rate_text" rows="4"
                            cols="20"></textarea>
                    <input id="PhotoRateButton" name="PhotoRateButton" type="hidden" />
                    <input type="button" name="button" value="<%= Translate.Text("RATE")%>" class="btn-submit"
                        onclick="javascript: document.getElementById('PhotoRateButton').value='PhotoRateButton';document.forms[0].submit();return false;" />
                </div>
            </div>
        </div>
    </div>
</div>
