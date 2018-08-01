﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using Rin.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rin.Middlewares
{
    public class DownloadRequestBodyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RequestRecordStorage _requestEventStorage;

        public DownloadRequestBodyMiddleware(RequestDelegate next, RequestRecordStorage requestEventStorage)
        {
            _next = next;
            _requestEventStorage = requestEventStorage;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var entry = _requestEventStorage.Records.FirstOrDefault(x => x.Id == context.Request.Query["id"]);
            if (entry == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            var contentType = entry.RequestHeaders["Content-Type"].FirstOrDefault();
            context.Response.ContentType = "application/octet-stream";
            context.Response.StatusCode = 200;
            var contentDisposition = new ContentDispositionHeaderValue("attachment");
            contentDisposition.SetHttpFileName(entry.Id + FileNameHelper.GetExtension(entry.Path, contentType));
            context.Response.GetTypedHeaders().ContentDisposition = contentDisposition;
            await context.Response.Body.WriteAsync(entry.RequestBody, 0, entry.RequestBody.Length);
        }
    }

    public class DownloadResponseBodyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RequestRecordStorage _requestEventStorage;

        public DownloadResponseBodyMiddleware(RequestDelegate next, RequestRecordStorage requestEventStorage)
        {
            _next = next;
            _requestEventStorage = requestEventStorage;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var entry = _requestEventStorage.Records.FirstOrDefault(x => x.Id == context.Request.Query["id"]);
            if (entry == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            var contentType = entry.ResponseHeaders["Content-Type"].FirstOrDefault();
            context.Response.ContentType = "application/octet-stream";
            context.Response.StatusCode = 200;
            var contentDisposition = new ContentDispositionHeaderValue("attachment");
            contentDisposition.SetHttpFileName(entry.Id + FileNameHelper.GetExtension(entry.Path, contentType));
            context.Response.GetTypedHeaders().ContentDisposition = contentDisposition;
            await context.Response.Body.WriteAsync(entry.ResponseBody, 0, entry.ResponseBody.Length);
        }
    }

    internal static class FileNameHelper
    {
        public static string GetExtension(string path, string contentType)
        {
            var originalExt = Path.GetExtension(path);
            if (!String.IsNullOrWhiteSpace(originalExt)) return originalExt;

            var pos = contentType.IndexOf(';');
            if (pos > -1) contentType = contentType.Substring(0, pos);

            switch (contentType)
            {
                case "text/html": return ".html";
                case "text/plain": return ".txt";
                case "text/css": return ".css";
                case "text/javascript":
                case "application/javascript": return ".js";
                case "text/xml":
                case "applicaitn/xml": return ".xml";
                case "text/json":
                case "application/json": return ".json";
                case "image/png": return ".png";
                case "image/jpeg": return ".jpg";
                case "image/svg":
                case "image/svg+xml": return ".svg";
                default: return ".bin";
            }
        }
    }
}
