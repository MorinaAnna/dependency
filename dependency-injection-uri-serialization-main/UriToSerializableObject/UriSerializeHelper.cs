namespace UriSerializationHelper;

public static class UriSerializeHelper
{
    public static UriAddress ToSerializableObject(this Uri address)
    {
        UriAddress obj = new UriAddress
        {
            Scheme = address.Scheme,
            Host = address.Host,
            Path =
            [
                ..from line in address.Segments
                where line.Length > 1
                select line.TrimEnd('/')
            ],
            Query =
            [
                ..from query in address.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries)
                let elements = query.Split('=')
                select new QueryElement { Key = elements[0], Value = elements[1], }
            ],
        };

        return obj;
    }
}
