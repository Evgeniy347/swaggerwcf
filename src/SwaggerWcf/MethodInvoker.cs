using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using SwaggerWcf.ServiceModel;

namespace SwaggerWcf
{
    public class MethodInvoker
    {
        public MethodInfo Method => _method;

        private readonly MethodInfo _method;
        private readonly ParameterInfo[] _parametrs;
        private readonly ParameterInfo _postParametr;
        private readonly WebInvokeAttribute _webAttribute;

        public MethodInvoker(MethodInfo method, WebInvokeAttribute webInvokeAttribute)
        {
            _method = method;
            _webAttribute = webInvokeAttribute;
            _parametrs = method.GetParameters();
            _postParametr = _parametrs?.FirstOrDefault();
        }


        public void Check()
        {
            //Доделать получение параметров из адресной строки
            if (_parametrs.Length > 1)
                throw new Exception("Запрещено использовать более одного параметра в запросах");

            if (!"POST".Equals(_webAttribute.Method, StringComparison.OrdinalIgnoreCase))
                throw new Exception("Потдерживаются только методы 'POST'");

            if (_webAttribute.RequestFormat != WebMessageFormat.Json || _webAttribute.ResponseFormat != WebMessageFormat.Json)
                throw new Exception("Потдерживается только Json");
        }

        internal void Invoke<TService>(HttpContext context, TService service)
            where TService : class
        {
            try
            {
                object responceObj = null;
                if (_postParametr != null)
                {
                    context.Request.InputStream.Seek(0, SeekOrigin.Begin);
                    using (StreamReader stream = new StreamReader(context.Request.InputStream))
                    {
                        string requestJson = stream.ReadToEnd();
                        if (service is ITracerWebService tracer)
                            tracer.TraceRequest(this, requestJson);

                        object postParam = null;
                        try
                        {
                            postParam = JsonConvert.DeserializeObject(requestJson, _postParametr.ParameterType);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Deserialize object exeption type '{_postParametr.ParameterType}' message '{requestJson}'", ex);
                        }
                        responceObj = _method.Invoke(service, new object[] { postParam });
                    }
                }
                else
                {
                    responceObj = _method.Invoke(service, null);
                }


                if (responceObj != null)
                {
                    string responceJson = JsonConvert.SerializeObject(responceObj);
                    context.Response.Write(responceJson);
                    if (service is ITracerWebService tracer)
                        tracer.TraceResponce(this, responceJson);
                }
                context.Response.ContentType = "application/json";
                context.Response.HeaderEncoding = Encoding.UTF8;
                context.Response.ContentEncoding = Encoding.UTF8;
            }
            catch (Exception ex)
            {
                if (service is ITracerWebService tracer)
                    tracer.TraceError(ex);
                throw;
            } 
        }
    }
}