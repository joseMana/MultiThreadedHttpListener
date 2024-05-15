using System.Net;
using System.Text;

namespace MultiThreadedHttpListener
{
    class Program
    {
        static void Main(string[] args)
        {
            string uri = "http://localhost:8080/";

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(uri);
            listener.Start();

            Console.WriteLine("Listening for connections...");

            while (true)
            {
                HttpListenerContext context = listener.GetContext();

                Task.Factory.StartNew(() => HandleRequest(context));
            }
        }

        static void HandleRequest(HttpListenerContext context)
        {
            string responseString = "<html><body>Hello from the HTTP listener!</body></html>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

            HttpListenerResponse response = context.Response;

            response.ContentType = "text/html";
            response.ContentLength64 = buffer.Length;

            response.OutputStream.Write(buffer, 0, buffer.Length);

            response.Close();
        }

        static void HandleRequest2(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;

            string[] segments = request.Url.AbsolutePath.Trim('/').Split('/');

            if (segments.Length >= 2)
            {
                string methodName = segments[0];
                string parameter = segments[1];

                if (methodName.Equals("mymethod", StringComparison.OrdinalIgnoreCase))
                {
                    string responseString = $"<html><body>Parameter received: {parameter}</body></html>";
                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);

                    HttpListenerResponse response = context.Response;

                    response.ContentType = "text/html";
                    response.ContentLength64 = buffer.Length;

                    response.OutputStream.Write(buffer, 0, buffer.Length);

                    response.Close();
                    return;
                }
            }

            string errorResponse = "<html><body>Invalid request</body></html>";
            byte[] errorBuffer = Encoding.UTF8.GetBytes(errorResponse);

            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.ContentType = "text/html";
            context.Response.ContentLength64 = errorBuffer.Length;
            context.Response.OutputStream.Write(errorBuffer, 0, errorBuffer.Length);
            context.Response.Close();
        }
    }
}
