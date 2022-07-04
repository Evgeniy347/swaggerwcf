using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading;
using System.Web;
using SwaggerWcf.Models;
using SwaggerWcf.Support;

namespace SwaggerWcf
{
    /// <summary>
    /// Построитель документации swagger
    /// </summary>
    public static class SwaggerServiceBuilder
    {
        private static ConcurrentDictionary<Type, ServiceInvoker> Services { get; } = new ConcurrentDictionary<Type, ServiceInvoker>();

        public static ServiceInvoker EnshureSwaggerService(Type type, HttpContext context, SecurityDefinitions securityDefinitions = null, Info info = null)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!Services.TryGetValue(type, out ServiceInvoker serviceInvoker))
            {
                Service service = ServiceBuilder.Build(type);
                if (info != null)
                    service.Info = info;
                if (securityDefinitions != null)
                    service.SecurityDefinitions = securityDefinitions;

                service.BasePath = GetServiceURL(context);

                string json = service.SerializeJson();
                serviceInvoker = new ServiceInvoker(type, json);

                Services[type] = serviceInvoker;
            }

            return serviceInvoker;
        }

        public static void OpenSwaggerService<TService>(HttpContext context, TService service)
               where TService : class
        {
            try
            {
                if (IsDocs(context))
                {
                    OpenSwaggerDocumentation<TService>(context);
                }
                else
                {
                    string serviceName = System.IO.Path.GetFileNameWithoutExtension(context.Request.Url.Segments.Last().Trim('/'));

                    if ("get".Equals(context.Request.HttpMethod, StringComparison.InvariantCultureIgnoreCase) &&
                      typeof(TService).Name.Equals(serviceName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        string url = GetDocsURL(context);
                        context.Response.Redirect(url, true);
                        return;
                    }
                    else
                    {
                        InvokeWebMethod(context, service); 
                    }
                }
            }
            catch (ThreadAbortException)
            { }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/plain";
                context.Response.HeaderEncoding = Encoding.UTF8;
                context.Response.ContentEncoding = Encoding.UTF8;
                context.Response.Write(ex.ToString());
            }
        }

        private static void InvokeWebMethod<TService>(HttpContext context, TService service)
               where TService : class
        {
            ServiceInvoker serviceInvoker = EnshureSwaggerService(typeof(TService), context);
            serviceInvoker.InvokeWebMethod(context, service);
        }

        private static void OpenSwaggerDocumentation<TService>(HttpContext context)
        {
            string fileName = context.Request.Url.Segments.LastOrDefault();
            string extension = System.IO.Path.GetExtension(fileName).ToLower().Trim('.');

            if (fileName == "docs.swagger")
            {
                ServiceInvoker serviceInvoker = EnshureSwaggerService(typeof(TService), context);
                context.Response.Write(serviceInvoker.SwaggerDocs);
            }
            else
            {
                if ("swagger/".Equals(fileName, StringComparison.OrdinalIgnoreCase))
                    fileName = "index";

                fileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
                fileName = fileName.Replace("-", "_").Replace(".", "_");

                byte[] content = (byte[])Properties.Resource.ResourceManager.GetObject(fileName);

                if (content == null || content.Length == 0)
                {
                    context.Response.StatusCode = 404;
                    context.Response.StatusDescription = $"End point '{fileName}' not found";
                }
                else
                    context.Response.BinaryWrite(content);
            }

            context.Response.HeaderEncoding = Encoding.UTF8;
            context.Response.ContentEncoding = Encoding.UTF8;

            if (extension.Equals("js"))
                context.Response.ContentType = "application/javascript";
            else if (extension.Equals("json") || extension.Equals("djson") || extension.Equals("swagger"))
                context.Response.ContentType = "application/json";
            else if (extension.Equals("css") || extension.Equals("dcss"))
                context.Response.ContentType = "text/css";
            else if (extension.Equals("gif"))
                context.Response.ContentType = "image/gif";
            else if (extension.Equals("png"))
                context.Response.ContentType = "image/png";
            else if (extension.Equals("ico"))
                context.Response.ContentType = "image/ico";
            else if (extension.Equals("ttf"))
                context.Response.ContentType = "application/x-font-ttf";
            else if (extension.Equals("html"))
                context.Response.ContentType = "text/html";
        }

        private static string GetDocsURL(HttpContext context)
        {
            List<string> segments = context.Request.Url.Segments
                .Select(x => x.Replace("/", ""))
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            segments.Add("swagger");

            string url = $"/{string.Join("/", segments.ToArray())}/";
            return url;
        }

        private static string GetServiceURL(HttpContext context)
        {
            List<string> segments = context.Request.Url.Segments
                .Select(x => x.Replace("/", ""))
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            while (segments.Count > 0)
            {
                string value = segments.Last();
                segments.RemoveAt(segments.Count - 1);
                if ("swagger".Equals(value, StringComparison.OrdinalIgnoreCase))
                    break;
            }

            string url = $"/{string.Join("/", segments.ToArray())}/";
            return url;
        }

        private static bool IsDocs(HttpContext context)
        {
            return context.Request.Url.Segments.Any(x => "swagger/".Equals(x, StringComparison.OrdinalIgnoreCase));
        }
    }
}