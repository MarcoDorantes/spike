<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="html" standalone="yes" omit-xml-declaration="yes" indent="no" encoding="iso-8859-1"/>

  <xsl:template match="/">
    <xsl:apply-templates select="chat"/>
  </xsl:template>

  <xsl:template match="chat">
    <html>
      <head>
        <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
        <title>Conversation</title>
      </head>
      <body>
        <xsl:apply-templates select="say"/>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="say">
    <div style="background-color: transparent; color: rgb(33, 33, 33); font-family: &amp;quot; wf_segoe-ui_normal&amp;quot;,&amp; quot; Segoe UI&amp;quot;,&amp; quot; Segoe WP&amp;quot;, Tahoma,Arial,sans-serif,serif,&amp;quot; EmojiFont&amp;quot;; font-size: 15px; font-style: normal; font-variant: normal; font-weight: 400; letter-spacing: normal; orphans: 2; text-align: left; text-decoration: none; text-indent: 0px; text-transform: none; -webkit-text-stroke-width: 0px; white-space: normal; word-spacing: 0px;">
      <font color="#666666" face="Segoe UI" size="1" style="color: rgb(102, 102, 102); font-family: Segoe UI; font-size: 10.06px;">
        <span style="font-size: 11px; font-variant: normal; text-transform: none;">
          <b>
            <xsl:value-of select='_3'/>
          </b>
        </span>
      </font>
      <xsl:text xml:space ='preserve'>      </xsl:text>
      <font color="#666666" face="Segoe UI" size="1" style="color: rgb(102, 102, 102); font-family: Segoe UI; font-size: 10.06px;">
        <span style="font-size: 11px; font-variant: normal; text-transform: none;">
          <b>
            <xsl:value-of select='when'/>:
          </b>
        </span>
      </font>
      <xsl:apply-templates select="_6"/>
    </div>
  </xsl:template>

  <xsl:template match="_6">
    <div style="margin-left: 8px;">
      <div style="color: rgb(33, 33, 33); font-family: &amp;quot; wf_segoe-ui_normal&amp;quot;,&amp; quot;Segoe UI&amp;quot;,&amp;quot;Segoe WP&amp;quot;,Tahoma,Arial,sans-serif,serif,&amp;quot;EmojiFont&amp;quot;; font-size: 15px;">
        <div style="margin-bottom: 0px; margin-left: 0px; margin-right: 0px; margin-top: 0px; padding-bottom: 0px; padding-left: 0px; padding-right: 0px; padding-top: 0px;">
          <xsl:apply-templates select="t"/>
        </div>
      </div>
    </div>
  </xsl:template>

  <xsl:template match="t">
    <font face="Calibri,sans-serif" size="2" style="color: rgb(33, 33, 33); font-family: Calibri,sans-serif; font-size: 13.33px;">
      <span style="font-size: 14.66px;">
        <font color="black" face="Segoe UI,sans-serif" size="2">
          <span style="font-size: 13.33px;">
            <xsl:value-of select='.'/>
          </span>
        </font>
      </span>
    </font>
  </xsl:template>

</xsl:stylesheet>