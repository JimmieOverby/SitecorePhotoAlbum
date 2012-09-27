<?xml version="1.0" encoding="UTF-8"?>

<!--=============================================================
    File: PhotoAlbumUserInfoWrapper.xslt                                                   
    Created by: sitecore\admin                                       
    Created: 10.08.2011 11:44:40                                               
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
  <xsl:param name="username" select="''"/>
  <xsl:param name="userId" select="''"/>
  <xsl:param name="albumID" select="''"/>
  <xsl:include  href="PhotoAlbumUserInfo.xslt"/>
  <!-- entry point -->
  <xsl:template match="*">
    <xsl:call-template name="ShowUserInfo">
      <xsl:with-param name="username" select="$username" />
      <xsl:with-param name="albumID" select="$albumID"/>
      <xsl:with-param name="userId" select="$userId"/>
    </xsl:call-template>
  </xsl:template>
</xsl:stylesheet>