namespace GardeningAPI.Common
{
    public static class LanguageMapper
    {
        private static readonly Dictionary<string, string> NameToCode = new()
        {
            { "English", "en" },
            { "Arabic", "ar" },
            { "Kurdish", "ku" }
        };

        private static readonly Dictionary<string, string> CodeToName =
            NameToCode.ToDictionary(x => x.Value, x => x.Key);

        public static string ToCode(string name)
        {
            return NameToCode.TryGetValue(name, out var code) ? code : "en";
        }

        public static string ToName(string code)
        {
            return CodeToName.TryGetValue(code, out var name) ? name : "English";
        }
    }

}
