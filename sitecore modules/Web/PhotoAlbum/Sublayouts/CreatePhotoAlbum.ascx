<%@ Control Language="C#" Debug="true" AutoEventWireup="true" CodeBehind="CreatePhotoAlbum.ascx.cs"
    Inherits="Sitecore.Modules.PhotoAlbum.Sublayouts.CreatePhotoAlbum" %>
<%@ Import Namespace="Sitecore.Web.UI.WebControls" %>
<link rel="Stylesheet" href="/sitecore modules/Web/PhotoAlbum/Styles/common_photoalbum.css" />
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
        <div class="npaCreateAlbumInput">
            <asp:TextBox ID="AlbumName" runat="server" Columns="30"></asp:TextBox><br />
            <asp:RegularExpressionValidator ID="AlbumNameValidator" runat="server" ErrorMessage="The Album name is not valid"
                ControlToValidate="AlbumName" ValidationExpression="^[\d\w\s]+$" Display="Dynamic"></asp:RegularExpressionValidator>
            <asp:Label ID="ErrMsg" runat="server" Visible="False" ForeColor="Red"></asp:Label>
        </div>
    </div>
    
    <div class="npaClear">
    </div>
    <div class="buttonCreate">
        <asp:Button ID="CreateNUpload" runat="server" Text="" OnClick="CreateNUpload_Click" />
    </div>
</div>
