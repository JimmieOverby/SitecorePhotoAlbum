<?xml version="1.0" encoding="UTF-8"?>

<!--=============================================================
    File: PhotoAlbum.xslt                                                   
    Created by: sitecore\admin                                       
    Created: 10.08.2011 11:26:09                                               
    Copyright notice at bottom of file
==============================================================-->

<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:sc="http://www.sitecore.net/sc"
  xmlns:dot="http://www.sitecore.net/dot"
  xmlns:album="http://www.sitecore.net/photoalbum"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  xmlns:js="urn:custom-javascript"
  exclude-result-prefixes="dot sc album msxsl js">


  
  <!-- output directives -->
  <xsl:output method="html" indent="no" encoding="UTF-8" />

  <!-- parameters -->
  <xsl:param name="lang" select="'en'"/>
  <xsl:param name="id" select="''"/>
  <xsl:param name="sc_item"/>
  <xsl:param name="sc_currentitem"/>
  <xsl:param name="album_id"/>
  <xsl:variable name="photos" select="sc:item($album_id,.)/item"/>

  
  <!-- variables -->
  <!-- Uncomment one of the following lines if you need a "home" variable in you code -->
  <!--<xsl:variable name="home" select="sc:item('/sitecore/content/home',.)" />-->
  <!--<xsl:variable name="home" select="/*/item[@key='content']/item[@key='home']" />-->
  <!--<xsl:variable name="home" select="$sc_currentitem/ancestor-or-self::item[@template='site root']" />-->


  <!-- entry point -->
  <xsl:template match="*">
    <xsl:apply-templates select="$sc_item" mode="main"/>
  </xsl:template>

 
  
  <!--==============================================================-->
  <!-- main                                                         -->
  <!--==============================================================-->
  <xsl:template match="*" mode="main">


    
    <script type="text/javascript">
      function imgClick(value){
        alert(value);
      }

    </script>

    <xsl:apply-templates mode="firstImage" select="sc:item($album_id,.)/item[1]">
    </xsl:apply-templates>
    <xsl:call-template name="imageList">
    </xsl:call-template>
  </xsl:template>

  <xsl:template mode="firstImage" match="*">
    <xsl:param name="album_id"/>
    <div id="photo" style=" text-align:center; float:left; ">
      <div style="height:320px; text-align:center;">
        <sc:image field="image" select="." width="200" height="300"  >
        </sc:image>
      </div>
      <div class="title">
      </div>
      <div class="slideshow">
        <a href="#">View Slideshow</a>
      </div>
      <div class="sub">142 photos | 21 views</div>
    </div>
  </xsl:template>
  <xsl:template name="imageList">
    <div id="thumbnails" style="float:right;">
      <ul>
        <xsl:for-each select="sc:item($album_id,.)/*">
          <li>
            <a href="#" class="aImg" onclick="imgClick('v');">
              <xsl:variable name="altText" select="sc:fld('Title',.)"/>
              <sc:image  field="image" select="." width="61" height="61" backgroundcolor="white" class="img-shadow">
              </sc:image>
            </a>
          
            <img src="{sc:fld('image',.,'src')}"  />
            
            <img>
              <xsl:attribute name ="src">
                <xsl:value-of  select="concat('/',sc:fld('image',.,'src'))"/>
              </xsl:attribute>
            </img>

            <xsl:value-of  select="concat('/',sc:fld('image',.,'src'))"/>
            
          </li>
        </xsl:for-each>
      </ul>
    </div>
  </xsl:template>


 
  
</xsl:stylesheet>



