using System;

namespace DALPuntoVentaWeb.Entidades
{
    /// <summary>
    /// Clase de control de la entidad WebService Json Responses (para las llamadas a Sr Pago)
    /// </summary>
    public class WSJsonResponses
    {
        public class PostProfile
        {
            public bool     Success     { get; set; }
            public object   Result      { get; set; }
            public object   Error       { get; set; }
        }

        public class PostProfileDocuments
        {
            public bool     Success     { get; set; }
            public object   Result      { get; set; }
            public object   Error       { get; set; }
        }

        public class Error
        {
            public string   Code                { get; set; }
            public string   Message             { get; set; }
            public string   Description         { get; set; }
            public int      Http_Status_Code    { get; set; }
        }  
    }
}
