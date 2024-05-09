namespace E2E.Tests.Common;

public static class Requests
{
    public static RestRequest Get(String path, IEnumerable<Parameter>? parameters = null!)
    {
        return Request(Method.Get, path, null, null, parameters);
    }

    public static RestRequest Post(String path, String? body, ICollection<KeyValuePair<string, string>>? headers, IEnumerable<Parameter>? parameters = null!)
    {
        return Request(Method.Post, path, body, headers, parameters);
    }

    private static RestRequest Request(Method method, String path, String? body, ICollection<KeyValuePair<string, string>>? headers, IEnumerable<Parameter>? parameters = null!)
    {
        var request = new RestRequest(path, method);
        if (headers != null)
        {
            request.AddOrUpdateHeaders(headers);
        }
        if (body != null)
        {
            request.AddJsonBody(body);
        }
        if (parameters != null)
        {
            request.AddOrUpdateParameters(parameters);
        }

        return request;
    }
}