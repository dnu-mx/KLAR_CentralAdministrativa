using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace ComercioElectronico.Controllers.Extensions
{
    public  static class ObjectExtensions
    {
        public static string ToJsonString<T>(this T myObject)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(myObject);
        }

        public static T FromJsonString<T>(this string myJsonObject)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(myJsonObject);
        }

        public static T FromJsonStringNull<T>(this string myJsonObject)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(myJsonObject, new JsonSerializerSettings(){NullValueHandling = NullValueHandling.Include});
        }

    }
}