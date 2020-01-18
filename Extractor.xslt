<?xml version="1.0" encoding="UTF-8"?>
<!--
  XSLT for accessing data in OFX file and invoking extension function
 -->
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:ext="uri:extension">

  <xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>

  <xsl:template match="/">
    <xsl:apply-templates select="/OFX" />
  </xsl:template>

  <xsl:template match="OFX">
    <xsl:apply-templates select="BANKMSGSRSV1" />
    <xsl:apply-templates select="CREDITCARDMSGSRSV1" />
  </xsl:template>

  <xsl:template match="BANKMSGSRSV1">
    <xsl:apply-templates select="STMTTRNRS/STMTRS/BANKTRANLIST">
      <xsl:with-param name="account-number" select="STMTTRNRS/STMTRS/BANKACCTFROM/ACCTID" />
    </xsl:apply-templates>
  </xsl:template>

  <xsl:template match="CREDITCARDMSGSRSV1">
    <xsl:apply-templates select="CCSTMTTRNRS/CCSTMTRS/BANKTRANLIST">
      <xsl:with-param name="account-number" select="CCSTMTTRNRS/CCSTMTRS/CCACCTFROM/ACCTID" />
    </xsl:apply-templates>
  </xsl:template>

  <xsl:template match="BANKTRANLIST">
    <xsl:param name="account-number" />
    <xsl:apply-templates select="STMTTRN[number(TRNAMT)!=0]">
      <xsl:with-param name="account-number" select="$account-number" />
    </xsl:apply-templates>
  </xsl:template>

  <xsl:template match="STMTTRN">
    <xsl:param name="account-number" />
    <xsl:value-of select="ext:SaveTransaction($account-number, TRNTYPE, DTPOSTED, NAME, TRNAMT, FITID, CHECKNUM)"/>
  </xsl:template>

</xsl:stylesheet>
