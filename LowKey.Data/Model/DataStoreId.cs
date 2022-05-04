using System.Diagnostics.CodeAnalysis;

namespace LowKey.Data.Model
{
    public record DataStoreId(string Value)
    {
        public static DataStoreId Empty = new DataStoreId(string.Empty);
        public static DataStoreId From(string value) => new DataStoreId(value);
    }
}
