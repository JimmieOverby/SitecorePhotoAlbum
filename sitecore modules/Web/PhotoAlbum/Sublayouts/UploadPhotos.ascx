<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UploadPhotos.ascx.cs" Inherits="Sitecore.Modules.PhotoAlbum.Sublayouts.UploadPhotos" %>
<link rel="Stylesheet" href="/sitecore modules/Web/PhotoAlbum/Styles/common_photoalbum.css" />
<%@ Import Namespace="Sitecore.Web.UI.WebControls" %>
<div>
    <h1>
        <%= FieldRenderer.Render(currPageItem, "Title")%>
    </h1>
</div>
<div class="npaCreateAlbumContainer">
    <div class="npaCreateAlbumRow">
        <div class="npaCreateAlbumLabel">
            <asp:Label ID="Label1" runat="server"></asp:Label>:
        </div>
        <div class="npaUploadPhotoInput">
            <asp:DropDownList ID="AlbumsList" runat="server">
            </asp:DropDownList>
            <br />
            <asp:Label ID="ErrorMsg" runat="server" ForeColor="Red" Visible="False"></asp:Label>
        </div>
    </div>
    <div class="npaCreateAlbumRow">
        <div class="npaCreateAlbumLabel">
            <asp:Label ID="Label2" runat="server"></asp:Label>:
        </div>
        <div class="npaUploadPhotoInput">
            <asp:FileUpload ID="FileUpload1"  runat="server" />
            <div class="selectNUploadButton">
                <asp:Button ID="SelectNUpload" runat="server" Text="" OnClick="SelectNUpload_Click" />
            </div>
        </div>
    </div>
    <div style="clear: both">
    </div>    
</div>

