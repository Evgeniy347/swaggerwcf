using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Web;
using System.Web;

namespace SwaggerWcf
{
    public class ServiceInvoker
    {
        public ServiceInvoker(Type type, string json)
        {
            if (string.IsNullOrEmpty(json))
                throw new ArgumentNullException(nameof(json));

            Type = type ??
                throw new ArgumentNullException(nameof(type));

            SwaggerDocs = json;
        }

        public Type Type { get; }

        public string SwaggerDocs { get; }


        private Dictionary<string, MethodInvoker> _Methods;
        private Dictionary<string, MethodInvoker> Methods => _Methods ?? (_Methods = InitMethod());

        public void InvokeWebMethod<TService>(HttpContext context, TService service)
               where TService : class
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            if (typeof(TService) != Type)
                throw new InvalidCastException($"type '{typeof(TService)}' not cast to type '{Type}'");

            string methodName = context.Request.Url.Segments.Last().Trim('/');

            if (Methods.TryGetValue(methodName, out MethodInvoker methodInvoker))
            {
                methodInvoker.Invoke(context, service);
            }
            else
            {
                context.Response.StatusCode = 404;
                context.Response.StatusDescription = $"Action '{methodName}' not found";
            }
        }

        private Dictionary<string, MethodInvoker> InitMethod()
        {
            MethodInfo[] methods = Type.GetMethods();

            Dictionary<string, MethodInvoker> result = new Dictionary<string, MethodInvoker>(StringComparer.OrdinalIgnoreCase);

            foreach (MethodInfo method in methods)
            {
                WebInvokeAttribute webInvoke = method.GetCustomAttribute<WebInvokeAttribute>();
                if (webInvoke != null)
                {
                    MethodInvoker methodInvoker = new MethodInvoker(method, webInvoke);
                    methodInvoker.Check();
                    result[method.Name] = methodInvoker;
                }
            }

            return result;
        }
    }
}