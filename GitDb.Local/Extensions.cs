using Newtonsoft.Json;

namespace GitDb.Local
{
    internal static class Extensions
    {
        public static T As<T>(this string source) =>
            JsonConvert.DeserializeObject<T>(source);
    }
}
