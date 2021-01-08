﻿using Communication.Business;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Processor.Sender
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next,
            ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            //First, get the incoming request
            var request = await FormatRequest(context.Request);

            //Copy a pointer to the original response body stream
            var originalBodyStream = context.Response.Body;

            //Create a new memory stream...
            using (var responseBody = new MemoryStream())
            {
                //...and use that for the temporary response body
                context.Response.Body = responseBody;

                //Continue down the Middleware pipeline, eventually returning to this class
                await _next(context);

                //Format the response from the server
                var response = await FormatResponse(context.Response);

                //Save request log
                using (_logger.BeginScope(new Dictionary<string, object> { { "IsRequestLog", true } }))
                {
                    if (_logger.IsEnabled(LogLevel.Warning))
                    {
                        _logger.LogWarning($"{request} Response:({response})");
                    }
                    else if (_logger.IsEnabled(LogLevel.Information) ||
                        _logger.IsEnabled(LogLevel.Debug) ||
                        _logger.IsEnabled(LogLevel.Trace))
                    {
                        _logger.LogInformation($"{request} Response:({response})");
                    }
                }

                //Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            var bodyAsText = string.Empty;
            //Only log request payload if enable Information/Debug/Trace
            if (_logger.IsEnabled(LogLevel.Information) ||
                _logger.IsEnabled(LogLevel.Debug) ||
                _logger.IsEnabled(LogLevel.Trace))
            {
                var body = request.Body;

                //This line allows us to set the reader for the request back at the beginning of its stream.
                request.EnableBuffering();

                //We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
                var buffer = new byte[Convert.ToInt32(request.ContentLength)];

                //...Then we copy the entire request stream into the new buffer.
                await request.Body.ReadAsync(buffer, 0, buffer.Length);

                //We convert the byte[] into a string using UTF8 encoding...
                bodyAsText = Encoding.UTF8.GetString(buffer);

                //..and finally, assign the read body back to the request body, which is allowed because of EnableRewind()
                request.Body = body;
            }

            return $"{request.Method} {request.Scheme}://{request.Host}{request.Path} {request.QueryString} {bodyAsText}";
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            var text = string.Empty;
            //Only log response body if statuscode is not in range 200-299 OR enable Information/Debug/Trace
            if (_logger.IsEnabled(LogLevel.Information) ||
                _logger.IsEnabled(LogLevel.Debug) ||
                _logger.IsEnabled(LogLevel.Trace) ||
                !response.IsSuccessStatusCode())
            {
                //We need to read the response stream from the beginning...
                response.Body.Seek(0, SeekOrigin.Begin);

                //...and copy it into a string
                text = await new StreamReader(response.Body).ReadToEndAsync();

                //We need to reset the reader for the response so that the client can read it.
                response.Body.Seek(0, SeekOrigin.Begin);

            }

            //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
            return $"{response.StatusCode} {text}";
        }
    }
}
