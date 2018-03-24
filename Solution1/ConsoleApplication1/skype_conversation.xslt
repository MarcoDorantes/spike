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
    <div>
      <p class="x_MsoNormal" style="margin-bottom: 0pt;">
        <span class="x_imsender86">
          <span style="line-height: 106%; font-size: 8.5pt;">
            <xsl:value-of select='_3'/>
          </span>
        </span>
        <xsl:text xml:space ='preserve'>  </xsl:text>
        <span class="x_messagetimestamp86">
          <span style="line-height: 106%; font-size: 8.5pt;">
            <xsl:value-of select='when'/>:
          </span>
        </span>
      </p>
      <div style="margin: 8.25pt;">
        <p class="x_MsoNormal" style="line-height: normal; margin-bottom: 0pt; -ms-text-autospace:;">
          <span style="color: black; font-family: &quot;Segoe UI&quot;,sans-serif,serif,'EmojiFont'; font-size: 11pt;">
            <xsl:value-of select='_6'/>
          </span>
        </p>
      </div>
    </div>
  </xsl:template>

</xsl:stylesheet>