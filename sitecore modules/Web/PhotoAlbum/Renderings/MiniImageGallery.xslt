<?xml version="1.0" encoding="UTF-8"?>

<!--=============================================================
    File: MiniImageGallery.xslt                                                   
    Created by: sitecore\admin                                       
    Created: 09.08.2011 10:07:48                                               
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
  <xsl:param name="rotation_frequency" select="'3000'"/>
  <xsl:param name="album_path" select="album:GetAlbumItemId()"/>
  <xsl:variable name="RotatorId" select="album:GetRotatorId()" />

  <!-- entry point -->
  <xsl:template match="*">
    <xsl:apply-templates select="$sc_item" mode="main"/>
  </xsl:template>

  <!--==============================================================-->
  <!-- main                                                         -->
  <!--==============================================================-->
  <xsl:template match="*" mode="main">
    <xsl:call-template name="RotateSpots">
    </xsl:call-template>
  </xsl:template>

  <!--=========================================
                 RotateSpots
  =============================================-->

  <xsl:template name="RotateSpots">
    <xsl:choose>
      <xsl:when test="sc:fld('image',sc:item($album_path,.)/item,'src')!=''">
        <xsl:if test ="count(sc:item($album_path,.)/item) > 0">

          <div class="photo-album">
            <div id="photo-slideshow">

              <h1>My Photo Album</h1>
              <div class="return">
                <a>
                  <xsl:attribute name ="href">
                    <xsl:value-of select="album:ReturnToAlbum()"/>
                  </xsl:attribute>
                  Return to Album
                </a>
              </div>

              <div id="photo">

                <xsl:attribute name="id">
                  <xsl:value-of select="$RotatorId"/>
                </xsl:attribute>
                <h2>
                  <!--<xsl:value-of select="sc:fld('Name',sc:item($album_path,.))"/>-->
                </h2>
                <xsl:call-template name="spot">
                  <xsl:with-param name="spotItem" select="sc:item($album_path,.)"/>
                </xsl:call-template>

              </div>

              <div class="overlay">
                <span>
                  <xsl:value-of select="sc:fld('Name',sc:item($album_path,.))"/>
                </span>
              </div>
              <div class="controls">
                <a href="#" class="back"></a>
                <a href="#" class="play"></a>
                <a href="#" class="pause"></a>
                <a href="#" class="forward"></a>
                <div class="interval">
                  <a href="#" class="minus"></a>
                  <a href="#" class="plus"></a>
                  seconds:<span>3</span>
                </div>
                <a href="#" class="hide-captions">hide captions</a>
              </div>

            </div>
            <div class="clear"></div>

            <xsl:call-template name="RotateScript">
              <xsl:with-param name="controlId" select="$RotatorId"/>
              <xsl:with-param name="timeout" select="$rotation_frequency"/>
            </xsl:call-template>
          </div>
        </xsl:if>

      </xsl:when>
      <xsl:otherwise>
        <p width='100%' align='center'>"This page requires specific parameters and can only be reached from a Album Detail page"</p>
      </xsl:otherwise>
    </xsl:choose>


  </xsl:template>

  <!--=========================================
                 Spot images
  =============================================-->

  <xsl:template name="spot">
    <xsl:param name="spotItem" select="''"/>

    <xsl:if test="$spotItem/@id">
      <xsl:for-each select="$spotItem/item[sc:fld('Hide', .) = '']">
        <div class="spotMiniGallery">
          <xsl:attribute name="style">
            <xsl:if test="position()!=1">
              <xsl:text>display:none;</xsl:text>
            </xsl:if>
          </xsl:attribute>
          <xsl:variable name="qsParam" select="album:MetaDataQs(./@id)"/>
          <sc:image field="image" select="." width="370" height="518" maxHeight="518" maxWidth="370" backgroundColor="white"  style="margin-left:55px;"/>
        </div>
      </xsl:for-each>
    </xsl:if>
  </xsl:template>

  <!--=========================================
                 RotateScript
  =============================================-->

  <xsl:template name="RotateScript">
    <xsl:param name="controlId" select="''"/>
    <xsl:param name="timeout" select="''"/>

    <script type="text/javascript">


      injectJQuery162();
      injectPhtoAlbumCSS();
      injectPhtoAlbumScripts();

      function injectPhtoAlbumCSS(){
      var style = document.createElement("link");
      style.href = "/sitecore modules/Web/PhotoAlbum/Styles/common_photoalbum.css";
      style.rel = "Stylesheet";
      document.getElementsByTagName('head')[0].appendChild(style);
      }

      function injectPhtoAlbumScripts(){
      var script = document.createElement("script");
      script.src = "/sitecore modules/Web/PhotoAlbum/js/PhotoAlbum.js";
      document.getElementsByTagName('head')[0].appendChild(script);
      }

      function injectJQuery162(){
      var script = document.createElement("script");
      script.src = "/sitecore modules/Web/PhotoAlbum/js/jquery-1.6.2.min.js";
      document.getElementsByTagName('head')[0].appendChild(script);
      }
      setTimeout(function () {
      new PhotoAlbumSlider("<xsl:value-of select="$controlId"/>").fadeNext();
      }, 900);

    </script>
  </xsl:template>

</xsl:stylesheet>