using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SwaggerWcf.ServiceModel
{
    /// <summary>
    /// Интерфейс для трассировки сообщений
    /// </summary>
    public interface ITracerWebService
    {
        /// <summary>
        /// Трассировака запроса
        /// </summary>
        /// <param name="methodInvoker"></param>
        /// <param name="request"></param>
        void TraceRequest(MethodInvoker methodInvoker, string request);

        /// <summary>
        /// Трассировка ответа
        /// </summary>
        /// <param name="methodInvoker"></param>
        /// <param name="responce"></param>
        void TraceResponce(MethodInvoker methodInvoker, string responce);

        /// <summary>
        /// Трассировка ошибки
        /// </summary>
        /// <param name="ex"></param> 
        void TraceError(Exception ex);
    }
}