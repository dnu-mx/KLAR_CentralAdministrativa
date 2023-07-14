using DALComercioElectronico.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DALComercioElectronico.Utilidades
{
    public static class CoreExtensions
    {

        #region datablesTolist
        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result;
        }

        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(System.DayOfWeek))
                {
                    DayOfWeek day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), row[property.Name].ToString());
                    property.SetValue(item, day, null);
                }
                else
                    if (property.PropertyType == typeof(string))
                    {

                        property.SetValue(item, row[property.Name].ToString(), null);
                    }
                else
                {

                    property.SetValue(item, row[property.Name], null);
                }
            }
            return item;
        }


        public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            
                                PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);

                                Type t = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                                var value = row[prop.Name];
                                object safeValue = (value == null) ? null : Convert.ChangeType(value, t);


                                //prop.SetValue(row[prop.Name], safeValue, null);
                            //Convert.ChangeType(safeValue, propertyInfo.PropertyType)


                                propertyInfo.SetValue(obj, safeValue, null);
                            


                            
                            
                            //


                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region enums

        public static List<DtoListFull> EnumToDtoList<T>() where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("Type must be an enum");

            return Enum.GetValues(typeof(T))
                .Cast<Enum>()
                .ToList().ConvertAll(m => new DtoListFull()
                {
                    Id = (int)Convert.ChangeType(m, m.GetType()),
                    Name = m.GetDescription(),
                    Active = true,

                });
        }


        public static string GetDescription<T>(this T value)
        {
            if (value == null)
                return "";

            FieldInfo field = value.GetType().GetField(value.ToString());

            if (field == null)
                return "";

            DescriptionAttribute attribute
                = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                    as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;

        }

        #endregion


        #region MyRegion

        

        #endregion
    }

}
