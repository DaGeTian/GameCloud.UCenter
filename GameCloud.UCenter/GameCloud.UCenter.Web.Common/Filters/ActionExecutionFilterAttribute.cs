using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using GameCloud.UCenter.Common.Dumper;
using GameCloud.UCenter.Common.Extensions;
using GameCloud.UCenter.Common.IP;
using GameCloud.UCenter.Common.Portable.Contracts;
using GameCloud.UCenter.Common.Portable.Exceptions;
using GameCloud.UCenter.Web.Common.Logger;
using MongoDB.Driver;

namespace GameCloud.UCenter.Web.Common.Filters
{
    /// <summary>
    /// Provide a class for action execution filter.
    /// </summary>
    public sealed class ActionExecutionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="context">Indicating the action context.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        public override async Task OnActionExecutingAsync(HttpActionContext context, CancellationToken token)
        {
            //this.LogInboundRequest(context);

            if (!context.ModelState.IsValid)
            {
                string errorMessage = context.ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .JoinToString("\n", e => e);

                context.Response = this.CreateErrorResponseMessage(UCenterErrorCode.InternalHttpServerError, errorMessage);
            }

            await base.OnActionExecutingAsync(context, token);
        }

        /// <summary>
        /// Occurs after the action method is invoked.
        /// </summary>
        /// <param name="context">Indicating the executed context.</param>
        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                //CustomTrace.TraceError(
                //    context.Exception,
                //    "Execute request exception: url:{0}, arguments: {1}",
                //    context.Request.RequestUri,
                //    context.ActionContext.ActionArguments);

                var errorCode = UCenterErrorCode.InternalHttpServerError;

                if (context.Exception is UCenterException)
                {
                    errorCode = (context.Exception as UCenterException).ErrorCode;
                }
                else if (context.Exception is MongoException)
                {
                    errorCode = UCenterErrorCode.InternalDatabaseError;
                }

                string errorMessage = context.Exception.Message;
                context.Response = this.CreateErrorResponseMessage(errorCode, errorMessage);
            }

            base.OnActionExecuted(context);
        }

        //private void LogInboundRequest(HttpActionContext context)
        //{
        //    try
        //    {
        //        string clientIpAddress = IPHelper.GetClientIpAddress(context.Request);
        //        string request = context.Request.ToString();
        //        string arguments = context
        //            .ActionArguments.Select(a => $"{a.Value.DumpToString(a.Key)}")
        //            .JoinToString(",");

        //        string message = $"Inbound Request IP: {clientIpAddress}\n\tRequest: {request}\n\n\t Arguments: {arguments}";
        //        CustomTrace.TraceInformation(message);
        //    }
        //    catch (Exception ex)
        //    {
        //        CustomTrace.TraceError(ex, $"Log Inbound Request Error: \n\t{context.Request}");
        //    }
        //}

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