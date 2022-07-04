using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Web;
using SwaggerWcf.Attributes;
using SwaggerWcf.Test.Service.Data;

namespace SwaggerWcf.Test.Service
{
    /// <summary>
    /// Обработчик Ajax запросов TestService.
    /// </summary>
    [SwaggerWcf("")]
    [SwaggerWcfTag("BookStore")]
    [SwaggerWcfServiceInfo(
        title: "SampleService",
        version: "0.0.1",
        Description = "Sample Service to test SwaggerWCF",
        TermsOfService = "Terms of Service"
    )]
    [SwaggerWcfContactInfo(
        Name = "Abel Silva",
        Url = "http://github.com/abelsilva",
        Email = "no@e.mail"
    )]
    [SwaggerWcfLicenseInfo(
        name: "Apache License 2.0",
        Url = "https://github.com/abelsilva/SwaggerWCF/blob/master/LICENSE"
    )]
    public class TestService : IHttpHandler
    { 
        public void ProcessRequest(HttpContext context)
        {
            SwaggerServiceBuilder.OpenSwaggerService(context, this);
        }

        [SwaggerWcfTag("Books")]
        [SwaggerWcfHeader("clientId", false, "Client ID", "000")]
        [SwaggerWcfResponse(HttpStatusCode.Created,
                            "Book created, value in the response body with id updated",
                            ExampleMimeType = "application/json",
                            ExampleContent = "{\"Author\": {\"Id\": \"1dacefd76d3f443d802d326dba990ab0\",\"Name\": \"Miguel de Cervantes\"},\"FirstPublished\": 1605,\"Id\": \"017b1d907db64a868cb5d2c4d47d6077\",\"Language\": 2,\"Title\":\"Don Quixote\"}")]
        [SwaggerWcfResponse(HttpStatusCode.BadRequest,
                            "Bad request",
                            true,
                            ExampleMimeType = "application/json",
                            ExampleContent = "{\"error:\": \"error description\"}")]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError,
                            "Internal error (can be forced using ERROR_500 as book title)",
                            true,
                            ExampleMimeType = "application/json",
                            ExampleContent = "{\"error:\": \"error description\"}")]
        public Book CreateBook(Book value)
        { 
            value.Id = Guid.NewGuid().ToString("N");
            Store.Books.Add(value); 
            return value;
        }
         
        public bool IsReusable => false;
    }
}