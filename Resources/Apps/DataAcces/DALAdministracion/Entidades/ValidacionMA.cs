using System;
using System.Data;
using System.Text;

namespace DALAdministracion.Entidades
{
    [Serializable]
    public class ValidacionMA
    {
        public Int64 ID_ValidadorMultiasignacion {get; set;}
        public Int64 ID_Vigencia { get; set; }
        public string Vigencia {get; set;}
        public bool esValidacionBase {get; set;}
        public string Nombre {get; set;}
        public string Campo {get; set;}
        public string ClaveTipoElemento { get; set; }
        public string Formula {get; set;}
        public string CodigoError {get; set;}
        public Int64 ID_ValidacionTrue {get; set;}
        public Int64 ID_ValidacionFalse {get; set;}
        public Int32 OrdenValidacion {get; set;}
        public bool EsActiva {get; set;}
        public bool Declinar {get; set;}
        public bool Estatus { get; set; }
        public bool PreRegla {get; set;}
        public bool PostRegla {get; set;}
        public Int32 Prioridad {get; set;}
        public Int64 ID_CadenaComercial {get; set;}
        public Int64 ID_Entidad { get; set; }
        public Int64 ID_Producto { get; set; }

        public override string ToString()
        {
            StringBuilder resp = new StringBuilder();
            resp.Append(ID_ValidadorMultiasignacion + "\n");
            resp.Append(esValidacionBase + "\n");
            resp.Append(Nombre + "\n");
            resp.Append(Campo + "\n");
            resp.Append(ClaveTipoElemento + "\n");
            resp.Append(Formula + "\n");
            resp.Append(CodigoError + "\n");
            resp.Append(ID_ValidacionTrue + "\n");
            resp.Append(ID_ValidacionFalse + "\n");
            resp.Append(OrdenValidacion + "\n");
            resp.Append(Declinar + "\n");
            resp.Append(Estatus + "\n");

            return resp.ToString();
        }
    }
}
