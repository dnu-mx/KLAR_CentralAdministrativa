using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALComercioElectronico.Entidades
{
    public static class CustomDaysOfWeek
    {
        public static List<DtoListFull> GetDaysOfWeek()
        {
            var dtoList = new List<DtoListFull>();

            dtoList.Add(new DtoListFull() { Id = 1, Name = "Domingo", Selected = true, Active = true });
            dtoList.Add(new DtoListFull() { Id = 2, Name = "Lunes", Selected = true, Active = true });
            dtoList.Add(new DtoListFull() { Id = 3, Name = "Martes", Selected = true, Active = true });
            dtoList.Add(new DtoListFull() { Id = 4, Name = "Miercoles", Selected = true, Active = true });
            dtoList.Add(new DtoListFull() { Id = 5, Name = "Jueves", Selected = true, Active = true });
            dtoList.Add(new DtoListFull() { Id = 6, Name = "Viernes", Selected = true, Active = true });
            dtoList.Add(new DtoListFull() { Id = 7, Name = "Sabado", Selected = true, Active = true });
            
            
            

            return dtoList;
        }
      
        
    }
}
