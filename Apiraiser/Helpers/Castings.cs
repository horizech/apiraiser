using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Dapper;
using Npgsql;


namespace Apiraiser.Helpers
{
    public static class Castings
    {

        public static IEnumerable<T> GetDictionaryFromDapperDynamicIEnumerable<T>(IEnumerable<dynamic> data)
        {
            List<Dictionary<string, object>> dict = data
                    .Select(x => ((IDictionary<string, object>)x)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)).ToList();
            return CastObject<IEnumerable<T>>(dict);
        }

        public static T ConvertObject<T>(object M) where T : class
        {
            // Serialize the original object to json
            // Desarialize the json object to the new type 
            var obj = JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(M));
            return obj;
        }

        public static T CastObject<T>(object M) where T : class
        {
            // Apply soft casting
            return (T)M;
        }
    }
}
