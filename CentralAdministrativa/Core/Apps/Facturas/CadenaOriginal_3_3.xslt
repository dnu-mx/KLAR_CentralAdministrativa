<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:cfdi="http://www.sat.gob.mx/cfd/3" xmlns:iedu="http://www.sat.gob.mx/iedu" xmlns:pago10="http://www.sat.gob.mx/Pagos" xmlns:terceros="http://www.sat.gob.mx/terceros" xmlns:ventavehiculos="http://www.sat.gob.mx/ventavehiculos" xmlns:ine="http://www.sat.gob.mx/ine" xmlns:obrasarte="http://www.sat.gob.mx/arteantiguedades" xmlns:destruccion="http://www.sat.gob.mx/certificadodestruccion" xmlns:decreto="http://www.sat.gob.mx/renovacionysustitucionvehiculos" xmlns:servicioparcial="http://www.sat.gob.mx/servicioparcialconstruccion" xmlns:vehiculousado="http://www.sat.gob.mx/vehiculousado" xmlns:notariospublicos="http://www.sat.gob.mx/notariospublicos" xmlns:consumodecombustibles="http://www.sat.gob.mx/consumodecombustibles" xmlns:valesdedespensa="http://www.sat.gob.mx/valesdedespensa" xmlns:aerolineas="http://www.sat.gob.mx/aerolineas" xmlns:pagoenespecie="http://www.sat.gob.mx/pagoenespecie" xmlns:registrofiscal="http://www.sat.gob.mx/registrofiscal" xmlns:nomina12="http://www.sat.gob.mx/nomina12" xmlns:tpe="http://www.sat.gob.mx/TuristaPasajeroExtranjero" xmlns:pfic="http://www.sat.gob.mx/pfic" xmlns:leyendasFisc="http://www.sat.gob.mx/leyendasFiscales" xmlns:implocal="http://www.sat.gob.mx/implocal" xmlns:divisas="http://www.sat.gob.mx/divisas" xmlns:donat="http://www.sat.gob.mx/donat" xmlns:cce11="http://www.sat.gob.mx/ComercioExterior11" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="2.0">
  <!-- Con el siguiente método se establece que la salida deberá ser en texto -->
  <xsl:output version="1.0" indent="no" encoding="UTF-8" method="text"/>
  <!-- En esta sección se define la inclusión de las plantillas de utilerías para colapsar espacios -->
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/2/cadenaoriginal_2_0/utilerias.xslt"/>
  <!-- En esta sección se define la inclusión de las demás plantillas de transformación para la generación de las cadenas originales de los complementos fiscales -->
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/EstadoDeCuentaCombustible/ecc11.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/donat/donat11.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/divisas/divisas.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/implocal/implocal.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/leyendasFiscales/leyendasFisc.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/pfic/pfic.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/TuristaPasajeroExtranjero/TuristaPasajeroExtranjero.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/nomina/nomina12.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/cfdiregistrofiscal/cfdiregistrofiscal.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/pagoenespecie/pagoenespecie.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/aerolineas/aerolineas.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/valesdedespensa/valesdedespensa.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/consumodecombustibles/consumodecombustibles.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/notariospublicos/notariospublicos.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/vehiculousado/vehiculousado.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/servicioparcialconstruccion/servicioparcialconstruccion.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/renovacionysustitucionvehiculos/renovacionysustitucionvehiculos.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/certificadodestruccion/certificadodedestruccion.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/arteantiguedades/obrasarteantiguedades.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/ComercioExterior11/ComercioExterior11.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/ine/ine11.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/iedu/iedu.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/ventavehiculos/ventavehiculos11.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/terceros/terceros11.xslt"/>
  <xsl:include href="http://www.sat.gob.mx/sitio_internet/cfd/Pagos/Pagos10.xslt"/>
  <!-- Aquí iniciamos el procesamiento de la cadena original con su | inicial y el terminador || -->
  <xsl:template match="/">
    |
    <xsl:apply-templates select="/cfdi:Comprobante"/>
    ||
  </xsl:template>
  <!-- Aquí iniciamos el procesamiento de los datos incluidos en el comprobante -->
  <xsl:template match="cfdi:Comprobante">
    <!-- Iniciamos el tratamiento de los atributos de comprobante -->
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@Version" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Opcional">
      <xsl:with-param select="./@Serie" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Opcional">
      <xsl:with-param select="./@Folio" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@Fecha" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Opcional">
      <xsl:with-param select="./@FormaPago" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@NoCertificado" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Opcional">
      <xsl:with-param select="./@CondicionesDePago" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@SubTotal" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Opcional">
      <xsl:with-param select="./@Descuento" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@Moneda" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Opcional">
      <xsl:with-param select="./@TipoCambio" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@Total" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@TipoDeComprobante" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Opcional">
      <xsl:with-param select="./@MetodoPago" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@LugarExpedicion" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Opcional">
      <xsl:with-param select="./@Confirmacion" name="valor"/>
    </xsl:call-template>
    <!-- Llamadas para procesar al los sub nodos del comprobante -->
    <xsl:apply-templates select="./cfdi:CfdiRelacionados"/>
    <xsl:apply-templates select="./cfdi:Emisor"/>
    <xsl:apply-templates select="./cfdi:Receptor"/>
    <xsl:apply-templates select="./cfdi:Conceptos"/>
    <xsl:apply-templates select="./cfdi:Impuestos"/>
    <xsl:for-each select="./cfdi:Complemento">
      <xsl:apply-templates select="."/>
    </xsl:for-each>
  </xsl:template>
  <!-- Manejador de nodos tipo CFDIRelacionados -->
  <xsl:template match="cfdi:CfdiRelacionados">
    <!-- Iniciamos el tratamiento de los atributos del CFDIRelacionados -->
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@TipoRelacion" name="valor"/>
    </xsl:call-template>
    <xsl:for-each select="./cfdi:CfdiRelacionado">
      <xsl:call-template name="Requerido">
        <xsl:with-param select="./@UUID" name="valor"/>
      </xsl:call-template>
    </xsl:for-each>
  </xsl:template>
  <!-- Manejador de nodos tipo Emisor -->
  <xsl:template match="cfdi:Emisor">
    <!-- Iniciamos el tratamiento de los atributos del Emisor -->
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@Rfc" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Opcional">
      <xsl:with-param select="./@Nombre" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@RegimenFiscal" name="valor"/>
    </xsl:call-template>
  </xsl:template>
  <!-- Manejador de nodos tipo Receptor -->
  <xsl:template match="cfdi:Receptor">
    <!-- Iniciamos el tratamiento de los atributos del Receptor -->
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@Rfc" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Opcional">
      <xsl:with-param select="./@Nombre" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Opcional">
      <xsl:with-param select="./@ResidenciaFiscal" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Opcional">
      <xsl:with-param select="./@NumRegIdTrib" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@UsoCFDI" name="valor"/>
    </xsl:call-template>
  </xsl:template>
  <!-- Manejador de nodos tipo Conceptos -->
  <xsl:template match="cfdi:Conceptos">
    <!-- Llamada para procesar los distintos nodos tipo Concepto -->
    <xsl:for-each select="./cfdi:Concepto">
      <xsl:apply-templates select="."/>
    </xsl:for-each>
  </xsl:template>
  <!--Manejador de nodos tipo Concepto-->
  <xsl:template match="cfdi:Concepto">
    <!-- Iniciamos el tratamiento de los atributos del Concepto -->
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@ClaveProdServ" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Opcional">
      <xsl:with-param select="./@NoIdentificacion" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@Cantidad" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@ClaveUnidad" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Opcional">
      <xsl:with-param select="./@Unidad" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@Descripcion" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@ValorUnitario" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@Importe" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Opcional">
      <xsl:with-param select="./@Descuento" name="valor"/>
    </xsl:call-template>
    <!-- Manejo de sub nodos de información Traslado de Conceptos:Concepto:Impuestos:Traslados-->
    <xsl:for-each select="./cfdi:Impuestos/cfdi:Traslados/cfdi:Traslado">
      <xsl:call-template name="Requerido">
        <xsl:with-param select="./@Base" name="valor"/>
      </xsl:call-template>
      <xsl:call-template name="Requerido">
        <xsl:with-param select="./@Impuesto" name="valor"/>
      </xsl:call-template>
      <xsl:call-template name="Requerido">
        <xsl:with-param select="./@TipoFactor" name="valor"/>
      </xsl:call-template>
      <xsl:call-template name="Opcional">
        <xsl:with-param select="./@TasaOCuota" name="valor"/>
      </xsl:call-template>
      <xsl:call-template name="Opcional">
        <xsl:with-param select="./@Importe" name="valor"/>
      </xsl:call-template>
    </xsl:for-each>
    <!-- Manejo de sub nodos de Retencion por cada una de los Conceptos:Concepto:Impuestos:Retenciones-->
    <xsl:for-each select="./cfdi:Impuestos/cfdi:Retenciones/cfdi:Retencion">
      <xsl:call-template name="Requerido">
        <xsl:with-param select="./@Base" name="valor"/>
      </xsl:call-template>
      <xsl:call-template name="Requerido">
        <xsl:with-param select="./@Impuesto" name="valor"/>
      </xsl:call-template>
      <xsl:call-template name="Requerido">
        <xsl:with-param select="./@TipoFactor" name="valor"/>
      </xsl:call-template>
      <xsl:call-template name="Requerido">
        <xsl:with-param select="./@TasaOCuota" name="valor"/>
      </xsl:call-template>
      <xsl:call-template name="Requerido">
        <xsl:with-param select="./@Importe" name="valor"/>
      </xsl:call-template>
    </xsl:for-each>
    <!-- Manejo de los distintos sub nodos de información aduanera de forma indistinta a su grado de dependencia -->
    <xsl:for-each select="./cfdi:InformacionAduanera">
      <xsl:apply-templates select="."/>
    </xsl:for-each>
    <!-- Llamada al manejador de nodos de CuentaPredial en caso de existir -->
    <xsl:if test="./cfdi:CuentaPredial">
      <xsl:apply-templates select="./cfdi:CuentaPredial"/>
    </xsl:if>
    <!-- Llamada al manejador de nodos de ComplementoConcepto en caso de existir -->
    <xsl:if test="./cfdi:ComplementoConcepto">
      <xsl:apply-templates select="./cfdi:ComplementoConcepto"/>
    </xsl:if>
    <!-- Llamada al manejador de nodos de Parte en caso de existir -->
    <xsl:for-each select=".//cfdi:Parte">
      <xsl:apply-templates select="."/>
    </xsl:for-each>
  </xsl:template>
  <!-- Manejador de nodos tipo Información Aduanera -->
  <xsl:template match="cfdi:InformacionAduanera">
    <!-- Manejo de los atributos de la información aduanera -->
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@NumeroPedimento" name="valor"/>
    </xsl:call-template>
  </xsl:template>
  <!-- Manejador de nodos tipo Información CuentaPredial -->
  <xsl:template match="cfdi:CuentaPredial">
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@Numero" name="valor"/>
    </xsl:call-template>
  </xsl:template>
  <!-- Manejador de nodos tipo ComplementoConcepto -->
  <xsl:template match="cfdi:ComplementoConcepto">
    <xsl:for-each select="./*">
      <xsl:apply-templates select="."/>
    </xsl:for-each>
  </xsl:template>
  <!-- Manejador de nodos tipo Parte -->
  <xsl:template match="cfdi:Parte">
    <!-- Iniciamos el tratamiento de los atributos de Parte-->
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@ClaveProdServ" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Opcional">
      <xsl:with-param select="./@NoIdentificacion" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@Cantidad" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Opcional">
      <xsl:with-param select="./@Unidad" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Requerido">
      <xsl:with-param select="./@Descripcion" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Opcional">
      <xsl:with-param select="./@ValorUnitario" name="valor"/>
    </xsl:call-template>
    <xsl:call-template name="Opcional">
      <xsl:with-param select="./@Importe" name="valor"/>
    </xsl:call-template>
    <!-- Manejador de nodos tipo InformacionAduanera-->
    <xsl:for-each select=".//cfdi:InformacionAduanera">
      <xsl:apply-templates select="."/>
    </xsl:for-each>
  </xsl:template>
  <!-- Manejador de nodos tipo Complemento -->
  <xsl:template match="cfdi:Complemento">
    <xsl:for-each select="./*">
      <xsl:apply-templates select="."/>
    </xsl:for-each>
  </xsl:template>
  <!-- Manejador de nodos tipo Domicilio fiscal -->
  <xsl:template match="cfdi:Impuestos">
    <!-- Manejo de sub nodos de Retencion por cada una de los Impuestos:Retenciones-->
    <xsl:for-each select="./cfdi:Retenciones/cfdi:Retencion">
      <xsl:call-template name="Requerido">
        <xsl:with-param select="./@Impuesto" name="valor"/>
      </xsl:call-template>
      <xsl:call-template name="Requerido">
        <xsl:with-param select="./@Importe" name="valor"/>
      </xsl:call-template>
    </xsl:for-each>
    <!-- Iniciamos el tratamiento de los atributos de TotalImpuestosRetenidos-->
    <xsl:call-template name="Opcional">
      <xsl:with-param select="./@TotalImpuestosRetenidos" name="valor"/>
    </xsl:call-template>
    <!-- Manejo de sub nodos de información Traslado de Impuestos:Traslados-->
    <xsl:for-each select="./cfdi:Traslados/cfdi:Traslado">
      <xsl:call-template name="Requerido">
        <xsl:with-param select="./@Impuesto" name="valor"/>
      </xsl:call-template>
      <xsl:call-template name="Requerido">
        <xsl:with-param select="./@TipoFactor" name="valor"/>
      </xsl:call-template>
      <xsl:call-template name="Requerido">
        <xsl:with-param select="./@TasaOCuota" name="valor"/>
      </xsl:call-template>
      <xsl:call-template name="Requerido">
        <xsl:with-param select="./@Importe" name="valor"/>
      </xsl:call-template>
    </xsl:for-each>
    <!-- Iniciamos el tratamiento de los atributos de TotalImpuestosTrasladados-->
    <xsl:call-template name="Opcional">
      <xsl:with-param select="./@TotalImpuestosTrasladados" name="valor"/>
    </xsl:call-template>
  </xsl:template>
</xsl:stylesheet>