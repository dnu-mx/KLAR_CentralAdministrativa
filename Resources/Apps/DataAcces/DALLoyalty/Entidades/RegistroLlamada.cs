
namespace DALCentroContacto.Entidades
{
    public class RegistroLlamada
    {
        public int      ID_MotivoLlamada    { get; set; }
        public int      ID_MedioAcceso      { get; set; }
        public int      ID_Colectiva        { get; set; }
        public string   Comentarios         { get; set; }
        public string   UsuarioLlama        { get; set; }
        public string   ParametrosLlamada   { get; set; }
    }
}
