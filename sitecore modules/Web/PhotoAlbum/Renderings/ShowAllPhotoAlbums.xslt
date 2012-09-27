<?xml version="1.0" encoding="UTF-8"?>

<!--=============================================================
    File: ShowAllPhotoAlbums.xslt                                                   
    Created by: sitecore\admin                                       
    Created: 10.08.2011 15:54:59                                               
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

  <!-- variables -->
  <xsl:variable name="viewAlbumPage" select="album:GetViewAlbumPageID()" />
  <xsl:param name="PageSize" select="album:GetAlbumsInPage()" />
  <xsl:param name="MaxPages" select="album:GetAlbumMaxPages()" />

  <!-- entry point -->

  <!--==============================================================-->
  <!-- main                                                         -->
  <!--==============================================================-->
  <xsl:template match="*" mode="main" name="ShowAllPhotoAlbums" >

    <xsl:param name="username" select="''"/>
    <xsl:param name="pathPhotoAlbums" select="."/>
    <xsl:param name="CurrentPage" select="sc:qs('page','1')" />

    <ul>

      <xsl:for-each select="$pathPhotoAlbums" >
        <xsl:sort select="boolean(./item[@tid = album:GetPhotoTemplateID()])" order="descending"/>
        <xsl:variable name="imagesCount" select="count(./item[@tid=album:GetPhotoTemplateID()])" />
        <xsl:variable name="IsHaveChild" select="boolean(./item[@tid = album:GetPhotoTemplateID()])" />
        <xsl:variable name="CurrentUser" select="sc:qs('CurrentUser','')" />
        <xsl:choose>
          <xsl:when test="$CurrentUser=$username">
            <xsl:if test="$IsHaveChild">
              <xsl:if test="(position() >= 1 + ($CurrentPage - 1) * ($PageSize)) and (position() &lt; (1 + $CurrentPage * ($PageSize)))">
                <xsl:variable name="currID" select="./@id"/>
                <li>
                  <xsl:variable name="albumHref" select="concat(sc:path(sc:item($viewAlbumPage,.)), '?CurrentAlbum=', $currID)"/>
                  <a>
                    <xsl:attribute name ="href">
                      <xsl:value-of select="concat($albumHref,'&amp;CurrentUser=',$username)" disable-output-escaping="yes"/>
                    </xsl:attribute>
                    <xsl:if test="$IsHaveChild">
                      <sc:image field="image" select="./item[@tid=album:GetPhotoTemplateID()]" width="61" height="61" backgroundcolor="white" class="img-shadow"/>
                    </xsl:if>
                  </a>
                  <div class="title">
                    <a>
                      <xsl:attribute name ="href">
                        <xsl:value-of select="concat($albumHref,'&amp;CurrentUser=',$username)" disable-output-escaping="yes"/>

                      </xsl:attribute>
                      <xsl:value-of select="sc:fld('Name', .)"/>
                    </a>
                  </div>
                  <div class="sub">
                    <xsl:choose>
                      <xsl:when test="$imagesCount = 1">
                        <xsl:value-of select="$imagesCount"/> photo
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:value-of select="$imagesCount"/> photos
                      </xsl:otherwise>
                    </xsl:choose>
                  </div>
                </li>
              </xsl:if>
            </xsl:if>

          </xsl:when>
          <xsl:otherwise>

            <xsl:if test="position() &lt; $PageSize+1">
              <xsl:if test="$IsHaveChild">
                <xsl:variable name="currID" select="./@id"/>
                <li>
                  <xsl:variable name="albumHref" select="concat(sc:path(sc:item($viewAlbumPage,.)), '?CurrentAlbum=', $currID)"/>
                  <a>
                    <xsl:attribute name ="href">
                      <xsl:value-of select="concat($albumHref,'&amp;CurrentUser=',$username)" disable-output-escaping="yes"/>
                    </xsl:attribute>
                    <xsl:if test="$IsHaveChild">
                      <sc:image field="image" select="./item[@tid=album:GetPhotoTemplateID()]" width="61" height="61" backgroundcolor="white" class="img-shadow"/>
                    </xsl:if>
                  </a>
                  <div class="title">
                    <a>
                      <xsl:attribute name ="href">
                        <xsl:value-of select="concat($albumHref,'&amp;CurrentUser=',$username)" disable-output-escaping="yes"/>

                      </xsl:attribute>
                      <xsl:value-of select="sc:fld('Name', .)"/>
                    </a>
                  </div>
                  <div class="sub">
                    <xsl:choose>
                      <xsl:when test="$imagesCount = 1">
                        <xsl:value-of select="$imagesCount"/> photo
                      </xsl:when>
                      <xsl:otherwise>
                        <xsl:value-of select="$imagesCount"/> photos
                      </xsl:otherwise>
                    </xsl:choose>
                  </div>
                </li>
              </xsl:if>
            </xsl:if>


          </xsl:otherwise>
        </xsl:choose>

      </xsl:for-each>
    </ul>

    <div class="clear"></div>

    <xsl:call-template name="Pages">
      <xsl:with-param name="pathPhotoAlbums" select="$pathPhotoAlbums" />
      <xsl:with-param name="username" select="$username" />
    </xsl:call-template>

  </xsl:template>

  <!-- Template displays page buttons -->
  <xsl:template name="Pages">
    <!-- params -->
    <xsl:param name="CurrentPage" select="sc:qs('page','1')" />
    <xsl:param name="pathPhotoAlbums" select="."/>
    <xsl:param name="username" select="''"/>

    <!-- variables -->
    <xsl:variable name="TotalItems" select="count($pathPhotoAlbums)-2" />
    <xsl:variable name="Pages" select="ceiling($TotalItems div $PageSize)" />
    <xsl:variable name="CurrentUser" select="sc:qs('CurrentUser','')" />

    <div style=" float:right;width:100%; " class="album-paging">
      <table width="100%">
        <tr>
          <td align="center">
            <xsl:if test="$TotalItems &gt; $PageSize">
              <!-- displays Previous button -->
              <xsl:choose>
                <xsl:when test="$CurrentPage = 1">
                  <xsl:variable name="albumHref" select="'?page=1'"/>
                  <a>
                    <xsl:attribute name ="href">
                      <xsl:value-of select="concat($albumHref,'&amp;CurrentUser=',$username)" disable-output-escaping="yes"/>
                    </xsl:attribute>
                    <span style="padding-left:2px"></span>
                    <span>Previous</span>
                  </a>
                </xsl:when>
                <xsl:otherwise>

                  <xsl:choose>
                    <xsl:when test="$CurrentUser=$username">
                      <xsl:variable name="albumHref" select="concat('?page=', ceiling($CurrentPage)-1)"/>
                      <a>
                        <xsl:attribute name ="href">
                          <xsl:value-of select="concat($albumHref,'&amp;CurrentUser=',$username)" disable-output-escaping="yes"/>
                        </xsl:attribute>
                        <span style="padding-left:2px"></span>
                        <span>Previous</span>
                      </a>

                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:variable name="albumHref" select="'?page=1'"/>
                      <a>
                        <xsl:attribute name ="href">
                          <xsl:value-of select="concat($albumHref,'&amp;CurrentUser=',$username)" disable-output-escaping="yes"/>
                        </xsl:attribute>
                        <span style="padding-left:2px"></span>
                        <span> Previous</span>
                      </a>
                    </xsl:otherwise>
                  </xsl:choose>
                </xsl:otherwise>
              </xsl:choose>

              <!-- displays links -->
              <xsl:for-each select="$pathPhotoAlbums[((position()-1) mod $PageSize = 0)]">

                <xsl:if test="(position() > ($CurrentPage - ceiling($MaxPages div 2)) or position() > (last() - $MaxPages)) and ((position() &lt; $CurrentPage + $MaxPages div 2) or (position() &lt; 1 + $MaxPages)) and position()">
                  <xsl:choose>
                    <xsl:when test="position() = $CurrentPage and $CurrentUser=$username">

                      <xsl:variable name="albumHref" select="concat('?page=', $CurrentPage)"/>
                      <a>
                        <xsl:attribute name ="href">
                          <xsl:value-of select="concat($albumHref,'&amp;CurrentUser=',$username)" disable-output-escaping="yes"/>
                        </xsl:attribute>
                        <span style="padding-left:4px"></span>
                        <xsl:value-of select="$CurrentPage"/>
                      </a>
                    </xsl:when>
                    <xsl:when test="(position() = $CurrentPage and $CurrentUser='') or (position() = 1 and $CurrentUser!=$username)">
                      <xsl:variable name="albumHref" select="concat('?page=', $CurrentPage)"/>
                      <a>
                        <xsl:attribute name ="href">
                          <xsl:value-of select="concat($albumHref,'&amp;CurrentUser=',$username)" disable-output-escaping="yes"/>
                        </xsl:attribute>
                        <span style="padding-left:4px"></span>
                        <xsl:value-of select="'1'"/>
                      </a>
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:variable name="albumHref" select="concat('?page=', position())"/>
                      <a>
                        <xsl:attribute name ="href">
                          <xsl:value-of select="concat($albumHref,'&amp;CurrentUser=',$username)" disable-output-escaping="yes"/>
                        </xsl:attribute>
                        <span style="padding-left:2px"></span>
                        <span><xsl:value-of select="position()"/>
                        </span>
                      </a>
                    </xsl:otherwise>
                  </xsl:choose>
                </xsl:if>

              </xsl:for-each>

              <!-- displays Next button -->
              <xsl:choose>
                <xsl:when test="$Pages  = $CurrentPage">
                  <xsl:variable name="albumHref" select="concat('?page=', $CurrentPage)"/>
                  <a>
                    <xsl:attribute name ="href">
                      <xsl:value-of select="concat($albumHref,'&amp;CurrentUser=',$username)" disable-output-escaping="yes"/>
                    </xsl:attribute>
                    <span style="padding-left:2px"></span>
                    <span >Next</span>
                  </a>
                </xsl:when>
                <xsl:otherwise>

                  <xsl:choose>
                    <xsl:when test="$CurrentUser=$username">
                      <xsl:variable name="albumHref" select="concat('?page=', ceiling($CurrentPage)+1)"/>
                      <a>
                        <xsl:attribute name ="href">
                          <xsl:value-of select="concat($albumHref,'&amp;CurrentUser=',$username)" disable-output-escaping="yes"/>
                        </xsl:attribute>
                        <span style="padding-left:2px"></span>
                        <span >Next</span>
                      </a>

                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:variable name="albumHref" select="'?page=2'"/>
                      <a>
                        <xsl:attribute name ="href">
                          <xsl:value-of select="concat($albumHref,'&amp;CurrentUser=',$username)" disable-output-escaping="yes"/>
                        </xsl:attribute>
                        <span style="padding-left:2px"></span>
                        <span >Next</span>
                      </a>
                    </xsl:otherwise>
                  </xsl:choose>

                </xsl:otherwise>
              </xsl:choose>
            </xsl:if>

          </td>
        </tr>
      </table>

    </div>

  </xsl:template>

</xsl:stylesheet>