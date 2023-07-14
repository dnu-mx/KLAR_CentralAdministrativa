namespace DALValidacionesBatchPPF.LogicaNegocio.Dtos
{
    public class DtoList
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }

    public class DtoListFull : DtoList
    {
        public bool Active { get; set; }
        public bool Selected { get; set; }
    }
}