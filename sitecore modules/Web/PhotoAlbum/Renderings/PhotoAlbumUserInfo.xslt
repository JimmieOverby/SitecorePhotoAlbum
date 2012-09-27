<?xml version="1.0" encoding="UTF-8"?>

<!--=============================================================
    File: PhotoAlbumUserInfo.xslt                                                   
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

  <!-- variables -->
  <xsl:variable name="albumsOverviewPage" select="album:GetAlbumsOverviewPageID()" />

  <!--==============================================================-->
  <!-- main                                                         -->
  <!--==============================================================-->
  <xsl:template match="*" mode="main" name="ShowUserInfo">
    <xsl:param name="username" select="''"/>
    <xsl:param name="userId" select="''"/>
    <xsl:param name="albumID" select="''"/>
    
    <div class="user-image">
      <a href="#">
        <img>
          <xsl:attribute name ="src">
            <xsl:value-of select="album:GetUserImage($username)"/>
          </xsl:attribute>
        </img>
      </a>
    </div>
    <div class="user-info">
      <p>
          <xsl:value-of select="album:GetFullUserName($username)"/>
      </p>
      <p class="username">
          <xsl:value-of select="$username"/>
      </p>
    </div>
    <xsl:variable name="feedUrl" select="album:GetFeedUrl($albumID,$userId)" />
    <xsl:if test="$feedUrl != ''">
      <a>
        <xsl:attribute name ="href">
          <xsl:value-of select="$feedUrl"/>
        </xsl:attribute>
        <img src="/sitecore modules/Web/PhotoAlbum/Styles/images/social-rss.png" />
      </a>
    </xsl:if>

    <!--<div class="user-social">
      <a href="http://twitter.com/share" class="twitter-share-button" data-count="none">Tweet</a>
      <script type="text/javascript" src="http://platform.twitter.com/widgets.js"></script>
      --><!--<iframe src="http://www.facebook.com/plugins/like.php?href=http%3A%2F%2Fnicam150rev110729%2F&amp;send=false&amp;layout=button_count&amp;width=450&amp;show_faces=false&amp;action=like&amp;colorscheme=light&amp;font&amp;height=21" scrolling="no" frameborder="0" style="border:none; overflow:hidden; width:60px; height:21px;" allowTransparency="true"></iframe>--><!--
    </div>-->
    <div class="clear"></div>
  </xsl:template>
</xsl:stylesheet>