using System;

namespace DALPuntoVentaWeb.Entidades
{
    public class DiaNoBancario
    {
        public string   Title       { get; set; }
        public int      EventId     { get; set; }
        public int      CalendarId  { get; set; }
        public DateTime StartDate   { get; set; }
        public DateTime EndDate     { get; set; }
        public bool     IsAllDay    { get; set; }
        //public string   Fecha       { get; set; }
    }
}
