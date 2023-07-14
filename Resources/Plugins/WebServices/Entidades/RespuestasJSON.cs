using System;

namespace WebServices.Entidades
{
    public class RespuestasJSON
    {
        #region Parabilium

        public class Login
        {
            public string   UserID          { get; set; }
            public string   NombreUsuario   { get; set; }
            public string   PrimerApellido  { get; set; }
            public string   SegundoApellido { get; set; }
            public string   Token           { get; set; }
            public int      CodRespuesta    { get; set; }
            public string   DescRespuesta   { get; set; }
        }

        public class AltaPersonaMoral
        {
            public string   Folio               { get; set; }
            public string   CuentaCacao         { get; set; }
            public string   Tarjeta             { get; set; }
            public string   MotivoRechazo       { get; set; }
            public string   FechaVencimiento    { get; set; }
            public string   Clabe               { get; set; }
            public string   TipoManufactura     { get; set; }
            public string   CodRespuesta        { get; set; }
            public string   DescRespuesta       { get; set; }
        }

        public class ActivarTarjeta
        {
            public string   IDSolicitud         { get; set; }
            public string   Tarjeta             { get; set; }
            public string   MedioAcceso         { get; set; }
            public int      CodRespuesta        { get; set; }
            public string   DescRespuesta       { get; set; }
        }

        public class BloquearTarjeta
        {
            public string   IDSolicitud           { get; set; }
            public string   Tarjeta               { get; set; }
            public string   MedioAcceso           { get; set; }
            public int      CodRespuesta            { get; set; }
            public string   DescRespuesta         { get; set; }
        }

        public class ConsultarTarjetaCredito
        {
            public string IDSolicitud { get; set; }
            public string Tarjeta { get; set; }
            public string MedioAcceso { get; set; }
            public string TipoMedioAcceso { get; set; }
            public string Cuenta { get; set; }
            public string Clabe { get; set; }
            public string FechaApertura { get; set; }
            public string NombreCliente { get; set; }
            public string Estatus { get; set; }
            public string TipoManufactura { get; set; }
            public string NumeroAdicionales { get; set; }
            public string NombreAdicionales { get; set; }
            public string FechaCorte { get; set; }
            public string FechaLimitePago { get; set; }
            public float SaldoAlCorteAnterior { get; set; }
            public float SaldoActual { get; set; }
            public float CreditoDisponible { get; set; }
            public float SaldoPromedio { get; set; }
            public float InteresesPeriodo { get; set; }
            public float PagoMinimoAlCorte { get; set; }
            public float MontoUltimoPago { get; set; }
            public string FechaUltimoPago { get; set; }
            public string FechaUltimaCompra { get; set; }
            public float MontoUltimaCompra { get; set; }
            public float TotalComprasPeriodo { get; set; }
            public string FechaUltimoRetiroATM { get; set; }
            public float MontoUltimoRetiroATM { get; set; }
            public float TotalRetirosATMEnPeriodo { get; set; }
            public float ComisionesCobradas { get; set; }
            public string FechaUltimaComisionCobrada { get; set; }
            public string AtrasoTotal { get; set; }
            public int PagosVencidos { get; set; }
            public string DiaFechaCorte { get; set; }
            public float PagoParaNoGenerarInteresesAlCorte { get; set; }
            public string FechaUltimoCorte { get; set; }
            public float MovtosEnTransito { get; set; }
            public float PagoMinimoActual { get; set; }
            public float PagoParaNoGenerarInteresesActual { get; set; }
            public float SaldoDisponible { get; set; }
            public float LimiteDeCredito { get; set; }
            public int CodRespuesta { get; set; }
            public string DescRespuesta { get; set; }
        }

        public class BlackList
        {
            public object   matches             { get; set; }
        }

        public class BlackListMatches
        {
            public string   identifier                { get; set; }
            public string   full_name                 { get; set; }
            public string   observations              { get; set; }
            public string   organism                  { get; set; }
            public string   country                   { get; set; }
            public string   fiscal_identity           { get; set; }
            public string   porcentaje_name           { get; set; }
            public string   porcentaje_observations   { get; set; }
            public string   status                    { get; set; }
        }

        public class EliminarTarjeta
        {
            public string IDSolicitud { get; set; }
            public string Tarjeta { get; set; }
            public string MedioAcceso { get; set; }
            public int CodRespuesta { get; set; }
            public string DescRespuesta { get; set; }
        }

        public class CancelarCuentas
        {
            public int CodRespuesta { get; set; }
            public string DescRespuesta { get; set; }
        }

        public class VerOperacionDif
        {
            public string   IdOperacion     { get; set; }
            public object   Parcialidades   { get; set; }
            public int      CodRespuesta    { get; set; }
            public string   DescRespuesta   { get; set; }
        }

        public class ParcialidadesOperacionDif
        {
            public string   No              { get; set; }
            public string   Fecha           { get; set; }
            public string   Intereses       { get; set; }
            public string   IvaIntereses    { get; set; }
            public string   Capital         { get; set; }
            public string   Total           { get; set; }
        }

        public class DifiereOperacion
        {
            public string   IdOperacion     { get; set; }
            public int      CodRespuesta    { get; set; }
            public string   DescRespuesta   { get; set; }
        }

        #endregion

        #region Boveda Digital

        public class Login_BD
        {
            public string user_id           { get; set; }
            public string user_name         { get; set; }
            public string first_last_name   { get; set; }
            public string second_last_name  { get; set; }
            public string token             { get; set; }
            public string guid              { get; set; }
            public string code_response     { get; set; }
            public string desc_response     { get; set; }
        }

        public class Issuers
        {
            public string issuer_key            { get; set; }
            public string issuer_name           { get; set; }
            public object sub_bins_groups       { get; set; }
            public string code_response         { get; set; }
            public string desc_response         { get; set; }
            public string guid                  { get; set; }
        }

        public class SubBinsGroup
        {
            public string sub_bins_group_key    { get; set; }
            public string sub_bins_group_name   { get; set; }
            public string issuer_key            { get; set; }
            public object manufacturing_types   { get; set; }
            public object stock                 { get; set; }
            public string code_response         { get; set; }
            public string desc_response         { get; set; }
            public string guid                  { get; set; }
        }

        public class ManufacturingTypes
        {
            public string manufacturing_Type_Key    { get; set; }
            public string manufacturing_Type_Name   { get; set; }
            public string code_response             { get; set; }
            public string desc_response             { get; set; }
            public string guid                      { get; set; }
        }

        public class Stock
        {
            public string available             { get; set; }
            public string reserved_physical     { get; set; }
            public string reserved_virtual      { get; set; }
            public string issued_physical       { get; set; }
            public string issued_virtual        { get; set; }
        }

        public class Lots
        {
            public string lot_key       { get; set; }
            public object values        { get; set; }
            public string code_response { get; set; }
            public string desc_response { get; set; }
            public string guid          { get; set; }
        }

        public class LotValues
        {
            public string masked_value      { get; set; }
            public string encrypted_value   { get; set; }
        }

        #endregion

        #region Lealtad

        public class ValidaMembresiaSams
        {
            public String CodigoRespuesta { get; set; }
            public String Descripcion { get; set; }
            public MembresiaSams MembresiaSams { get; set; }

        }

        public class MembresiaSams
        {
            public string MembershipNbr { get; set; }
            public int CategoryCd { get; set; }
            public String MemberCd { get; set; }
            public int TierType { get; set; }
            public int IssuingClubNbr { get; set; }
            public String StartDate { get; set; }
            public String NextRenewDate { get; set; }
            public String EndDate { get; set; }
            public String ShowRenewDate { get; set; }
            public String CurrentStatusCd { get; set; }
            public String PlusStatusCd { get; set; }
            public String AutoRenewInd { get; set; }
            public String AutoBillInd { get; set; }
            public String ConfidentialInd { get; set; }
            public int BusClubTypeCd { get; set; }
            public int QualifyOrg { get; set; }
            public String InfoCompleteInd { get; set; }
            public String PayrollDeductInd { get; set; }
            public CardholderResults[] CardholderResults { get; set; }
        }

        public class CardholderResults
        {
            public int CardholderNbr { get; set; }
            public String CardholderType { get; set; }
            public int ParentCardholderNbr { get; set; }
            public String FirstName { get; set; }
            public String LastName { get; set; }
            public String CardStatCd { get; set; }
        }

        public class SolictiudPedidosAmazon
        {
            public int CodigoRespuesta { get; set; }
            public string Descripcion { get; set; }
        }

        public class SolictiudPedidosUber
        {
            public int CodigoRespuesta { get; set; }
            public string Descripcion { get; set; }
        }

        #endregion

        #region DNU Usuarios

        public class LoginDNU
        {
            public int      CodigoRespuesta { get; set; }
            public string   Mensaje         { get; set; }
            public string   UserID          { get; set; }
            public string   NombreUsuario   { get; set; }
            public string   PrimerApellido  { get; set; }
            public string   SegundoApellido { get; set; }
            public string   Token           { get; set; }
            public object   Roles           { get; set; }
        }

        public class Sms
        {
            public int      CodigoRespuesta { get; set; }
            public string   Mensaje         { get; set; }
        }

        #endregion

        #region AppConnect

        public class Login_AC
        {
            public string   UserID          { get; set; }
            public string   UserTemp        { get; set; }
            public string   NombreUsuario   { get; set; }
            public string   PrimerApellido  { get; set; }
            public string   SegundoApellido { get; set; }
            public string   Token           { get; set; }
            public string   CodRespuesta    { get; set; }
            public string   DescRespuesta   { get; set; }
        }

        public class ConsultaUsuarioEmpresa
        {
            public string   CodRespuesta    { get; set; }
            public string   DescRespuesta   { get; set; }
            public string   idEmpresa       { get; set; }
            public string   idUsuario       { get; set; }
            public string   ClaveEmpresa    { get; set; }
        }

        public class UsuariosEmpresa
        {
            public string   CodRespuesta    { get; set; }
            public string   DescRespuesta   { get; set; }
            public string   idEmpresa       { get; set; }
            public string   idUsuario       { get; set; }
        }

        #endregion
    }
}
