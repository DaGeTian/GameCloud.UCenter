﻿namespace GF.UCenter.Web.Attributes
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using Common;
    using Common.IP;
    using Common.Portable;
    using CouchBase;
    using NLog;

    public sealed class ActionExecutionFilterAttribute : ActionFilterAttribute
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override async Task OnActionExecutingAsync(HttpActionContext context, CancellationToken token)
        {
            this.LogInboundRequest(context);

            if (!context.ModelState.IsValid)
            {
                string errorMessage = context.ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .JoinToString("\n", e => e);

                context.Response = this.CreateErrorResponseMessage(UCenterErrorCode.HttpClientError, errorMessage);
            }

            await base.OnActionExecutingAsync(context, token);
        }

        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                logger.Error(context.Exception, $"Execute request exception: url: {context.Request.RequestUri}",
                    context.ActionContext.ActionArguments);

                var errorCode = UCenterErrorCode.Failed;
                string errorMessage = null;

                if (context.Exception is UCenterException)
                {
                    errorCode = (context.Exception as UCenterException).ErrorCode;
                }
                else if (context.Exception is CouchBaseException)
                {
                    errorCode = UCenterErrorCode.CouchBaseError;
                }
                else
                {
                    errorCode = UCenterErrorCode.Failed;
                }

                errorMessage = context.Exception.Message;
                context.Response = this.CreateErrorResponseMessage(errorCode, errorMessage);
            }

            base.OnActionExecuted(context);
        }

        private void LogInboundRequest(HttpActionContext context)
        {
            try
            {
                string clientIpAddress = IPHelper.GetClientIpAddress(context.Request);
                string request = context.Request.ToString();
                string arguments = context.ActionArguments.Select(a => $"{a.Value.DumpToString(a.Key)}")
                    .JoinToString(",");

                string message = $"Inbound Request IP:{clientIpAddress}\n\tRequest: {request}\n\n\t Arguments:{arguments}";
                logger.Info(message);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Log Inbound Request Error: \n\t{context.Request}");
            }
        }

        private HttpResponseMessage CreateErrorResponseMessage(UCenterErrorCode errorCode, string errorMessage)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var content = new UCenterResponse<UCenterError>
            {
                Status = UCenterResponseStatus.Error,
                Error = new UCenterError { ErrorCode = errorCode, Message = errorMessage }
            };

            response.Content = new ObjectContent<UCenterResponse<UCenterError>>(content, new JsonMediaTypeFormatter());

            return response;
        }
    }
}