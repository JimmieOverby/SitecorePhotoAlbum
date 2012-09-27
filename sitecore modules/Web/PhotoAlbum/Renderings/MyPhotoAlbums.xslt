<?xml version="1.0" encoding="UTF-8"?>

<!--=============================================================
    File: AllPhotoAlbums.xslt                                                   
    Created by: sitecore\admin                                       
    Created: 08.08.2011 14:31:10                                               
    Copyright notice at bottom of file
==============================================================-->

<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:sc="http://www.sitecore.net/sc"
  xmlns:dot="http://www.sitecore.net/dot"
  xmlns:album="http://www.sitecore.net/photoalbum"
  exclude-result-prefixes="dot sc album">

  <!-- output directives -->
  <xsl:output method="html" indent="no" encoding="UTF-8" />

  <!-- parameters -->
  <xsl:param name="lang" select="'en'"/>
  <xsl:param name="id" select="''"/>
  <xsl:param name="sc_item"/>
  <xsl:param name="sc_currentitem"/>

  <!-- variables -->

  <xsl:variable name="allUsers" select="sc:item(album:GetRootPhotoAlbumId(),.)/*/*" />

  <xsl:include  href="PhotoAlbumUserInfo.xslt"/>
  <xsl:include  href="ShowAllPhotoAlbums.xslt"/>
  
  <!-- entry point -->
  <xsl:template match="*">
    <xsl:apply-templates select="$sc_item" mode="main"/>
  </xsl:template>

  <!--==============================================================-->
  <!-- main                                                         -->
  <!--==============================================================-->
  <xsl:template match="*" mode="main">
    <link rel="Stylesheet" href="/sitecore modules/Web/PhotoAlbum/Styles/common_photoalbum.css"></link>

    <div class="photo-album">
      <h1>
        My Photo Albums
        <!-- <sc:text field="Title" /> -->
      </h1>
     
      <xsl:call-template name="ShowUsersAlbums">
      </xsl:call-template>
    </div>
  </xsl:template>

  <xsl:template name="ShowUsersAlbums">

    <xsl:for-each select="$allUsers">
      <xsl:if test="@name=album:GetCurrentUserName()">
        <xsl:if test="boolean(item[@tid=album:GetAlbumTemplateID()])">

          <div id="albums-overview">
            
            <xsl:call-template name="ShowUserInfo">
              <xsl:with-param name="username" select="@name" />
              <xsl:with-param name="userId" select="@id" />
            </xsl:call-template>
            
            <div class="clear"></div>
            <xsl:call-template name="ShowAllPhotoAlbums">
              <xsl:with-param name="username" select="@name" />
              <xsl:with-param name="pathPhotoAlbums" select="./*" />
            </xsl:call-template>

          </div>
        </xsl:if>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>

</xsl:stylesheet>