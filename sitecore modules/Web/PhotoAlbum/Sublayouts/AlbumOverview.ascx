<%@ Control Language="c#" AutoEventWireup="True" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"
    CodeBehind="AlbumOverview.ascx.cs" Inherits="Layouts.Albumoverview.AlbumoverviewSublayout" %>
<%@ Import Namespace="Sitecore.Globalization" %>
<%@ Register Assembly="Sitecore.Modules.PhotoAlbum" Namespace="Sitecore.Modules.PhotoAlbum.Controls"
    TagPrefix="cc1" %>
<link rel="Stylesheet" href="/sitecore modules/Web/PhotoAlbum/Styles/common_photoalbum.css" />
<script src="/sitecore modules/Web/PhotoAlbum/js/jquery-1.6.2.min.js"></script>
<script src="/sitecore modules/Web/PhotoAlbum/js/PhotoAlbum.js"></script>
<div class="photo-album" id="photoalbum" runat="server">
    <div class="photo-album">
        <sc:XslFile runat="server" Path="/sitecore modules/Web/PhotoAlbum/Renderings/PhotoAlbumUserInfoWrapper.xslt"
            ID="XslFile1"></sc:XslFile>
        <div class="clear">
        </div>
        <div id="photo">
            <asp:Literal ID="BigPhoto" runat="server" />
        </div>
        <div id="thumbnails">
            <ul>
                <cc1:DataPagerRepeater ID="ThumbnailsRepeater" runat="server" PersistentDataSource="true">
                    <ItemTemplate>
                        <li>
                            <table width="100%">
                                <tr>
                                    <td width="100%" align="center" valign="middle" height="75px">
                                        <a href="<%# Eval("link") %>">
                                            <img class="img-shadow" alt="" src="<%# Eval("srcLink") %>" />
                                        </a>
                                    </td>
                                </tr>
                            </table>
                        </li>
                    </ItemTemplate>
                </cc1:DataPagerRepeater>
                <asp:ScriptManager ID="ScriptManager1" runat="server">
                </asp:ScriptManager>
                <asp:DataPager ID="dpPhotoAlbum" PagedControlID="ThumbnailsRepeater"  PageSize="9" runat="server" >
                    <Fields>
                        <asp:NextPreviousPagerField ButtonType="Link" ShowPreviousPageButton="true" ShowNextPageButton="false"  />
                        <asp:NumericPagerField ButtonType="Link" />
                        <asp:NextPreviousPagerField ButtonType="Link" ShowPreviousPageButton="false" ShowNextPageButton="true" />
                    </Fields>
                </asp:DataPager>
            </ul>
            <div class="clear">
            </div>
        </div>
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
