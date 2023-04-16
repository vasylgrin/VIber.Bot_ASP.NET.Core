namespace VIber.Bot_ASP.NET.Core.Extentions
{
    public static class DictionaryExtentions
    {
        public static void SetValue<TEntity,GEntity>(this Dictionary<TEntity, GEntity> dictionary, TEntity key, GEntity value)
        {
            if(!dictionary.TryGetValue(key, out GEntity outputValue))
            {
                dictionary.Add(key, value);
            }

            dictionary[key] = value;
        }
    }
}
