using System;
using System.Collections.Generic;

namespace DALAdministracion.Entidades
{
    /// <summary>
    /// Clase de control de la entidad Datos Personales Cuenta
    /// </summary>
    public class DatosPersonalesCuenta
    {
        #region Cuentas y Tarjetas

        public int      ID_Cuenta           { get; set; }
        public int      ID_MA               { get; set; }
        public int      ID_Colectiva        { get; set; }
        public int      ID_Cadena           { get; set; }
        public string   CuentaInterna       { get; set; }
        public string   TarjetaTitular      { get; set; }
        public string   TarjetaAdicional    { get; set; }
        public bool     SoloAdicionales     { get; set; }

        #endregion

        #region Personales

        public string   Nombre                  { get; set; }
        public string   ApellidoPaterno         { get; set; }
        public string   ApellidoMaterno         { get; set; }
        public DateTime FechaNacimiento         { get; set; }
        public string   ClaveEdoNacimiento      { get; set; }
        public string   EstadoNacimiento        { get; set; }
        public string   Genero                  { get; set; }
        public string   NumeroIdentificacion    { get; set; }
        public string   RFC                     { get; set; }
        public string   CURP                    { get; set; }
        public string   Ocupacion               { get; set; }
        public string   Profesion               { get; set; }
        public string   Nacionalidad            { get; set; }

        #endregion

        #region Domicilio

        public int      IdDireccion     { get; set; }
        public string   Calle           { get; set; }
        public string   NumExterior     { get; set; }
        public string   NumInterior     { get; set; }
        public string   EntreCalles     { get; set; }
        public string   Referencias     { get; set; }
        public string   CodigoPostal    { get; set; }
        public int      IdColonia       { get; set; }
        public string   Colonia         { get; set; }
        public string   ClaveMunicipio  { get; set; }
        public string   Municipio       { get; set; }
        public string   ClaveCiudad     { get; set; }
        public string   Ciudad          { get; set; }
        public string   ClaveEstado     { get; set; }
        public string   Estado          { get; set; }
        public int?     IdPais          { get; set; }
        public string   Pais            { get; set; }
        public string   Latitud         { get; set; }
        public string   Longitud        { get; set; }

        #endregion

        #region Contacto

        public string   NumTelParticular    { get; set; }
        public string   NumTelCelular       { get; set; }
        public string   NumTelTrabajo       { get; set; }
        public string   Email               { get; set; }

        #endregion

        #region Generales

        public string GiroNegocio   { get; set; }
        public string Enmascara     { get; set; }

        #endregion

        #region Fiscales

        public int      IdDireccionFiscal       { get; set; }
        public string   CalleFiscal             { get; set; }
        public string   NumExteriorFiscal       { get; set; }
        public string   NumInteriorFiscal       { get; set; }
        public string   CodigoPostalFiscal      { get; set; }
        public int      IdColoniaFiscal         { get; set; }
        public string   ColoniaFiscal           { get; set; }
        public string   ClaveMunicipioFiscal    { get; set; }
        public string   MunicipioFiscal         { get; set; }
        public string   ClaveEstadoFiscal       { get; set; }
        public string   EstadoFiscal            { get; set; }
        public int      IdRegimenFiscal         { get; set; }
        public string   ClaveRegimenFiscal      { get; set; }
        public string   RegimenFiscal           { get; set; }
        public int      IdUsoCFDI               { get; set; }
        public string   ClaveUsoCFDI            { get; set; }
        public string   UsoCFDI                 { get; set; }

        #endregion

        public IEnumerator<object> GetEnumerator()
        {
            yield return ID_Cuenta;
            yield return ID_MA;
            yield return ID_Colectiva;
            yield return ID_Cadena;
            yield return CuentaInterna;

            yield return TarjetaTitular;
            yield return TarjetaAdicional;
            yield return SoloAdicionales;

            yield return Nombre;
            yield return ApellidoPaterno;
            yield return ApellidoMaterno;
            yield return FechaNacimiento;
            yield return ClaveEdoNacimiento;
            yield return EstadoNacimiento;
            yield return Genero;
            yield return NumeroIdentificacion;
            yield return RFC;
            yield return CURP;
            yield return Ocupacion;
            yield return Profesion;
            yield return Nacionalidad;

            yield return IdDireccion;
            yield return Calle;
            yield return NumExterior;
            yield return NumInterior;
            yield return EntreCalles;
            yield return Referencias;
            yield return CodigoPostal;
            yield return IdColonia;
            yield return Colonia;
            yield return ClaveMunicipio;
            yield return Municipio;
            yield return ClaveCiudad;
            yield return Ciudad;
            yield return ClaveEstado;
            yield return Estado;
            yield return IdPais;
            yield return Pais;
            yield return GiroNegocio;
            yield return Latitud;
            yield return Longitud;

            yield return NumTelParticular;
            yield return NumTelCelular;
            yield return NumTelTrabajo;
            yield return Email;

            yield return IdDireccionFiscal;
            yield return CalleFiscal;
            yield return NumExteriorFiscal;
            yield return NumInteriorFiscal;
            yield return CodigoPostalFiscal;
            yield return IdColoniaFiscal;
            yield return ColoniaFiscal;
            yield return ClaveMunicipioFiscal;
            yield return MunicipioFiscal;
            yield return ClaveEstadoFiscal;
            yield return EstadoFiscal;
            yield return IdRegimenFiscal;
            yield return ClaveRegimenFiscal;
            yield return RegimenFiscal;
            yield return IdUsoCFDI;
            yield return ClaveUsoCFDI;
            yield return UsoCFDI;
        }
    }
}
