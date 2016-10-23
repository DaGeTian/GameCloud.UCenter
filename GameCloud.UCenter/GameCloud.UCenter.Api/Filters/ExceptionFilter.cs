using GameCloud.Database;
using GameCloud.UCenter.Common.MEF;
using GameCloud.UCenter.Common.Portable.Contracts;
using GameCloud.UCenter.Common.Portable.Exceptions;
using GameCloud.UCenter.Common.Settings;
using GameCloud.UCenter.Database;
using GameCloud.UCenter.Database.Entities;
using GameCloud.UCenter.Web.Common.Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MongoDB.Driver;
using System;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameCloud.UCenter.Api.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        private EventTrace eventTrace;

        public ExceptionFilter()
        {
            var ExportProvider = CompositionContainerFactory.Create();

            SettingsInitializer.Initialize<UCenterEventDatabaseContextSettings>(
                 ExportProvider,
                 SettingsDefaultValueProvider<UCenterEventDatabaseContextSettings>.Default,
                 AppConfigurationValueProvider.Default);
            this.eventTrace = ExportProvider.GetExportedValue<EventTrace>();
        }

        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            var exception = context.Exception;
            if (exception != null)
            {
                await TraceException(exception);

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

                var content = new UCenterResponse<UCenterError>
                {
                    Status = UCenterResponseStatus.Error,
                    Error = new UCenterError { ErrorCode = errorCode, Message = errorMessage }
                };

                context.Result = new JsonResult(content);
            }
        }

        private async Task TraceException(Exception exception)
        {
            if (exception != null && !(exception is UCenterException))
            {
                var exceptionEvent = new ExceptionEventEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    Message = exception.Message,
                    StackTrace = exception.StackTrace
                };
                if (exception.InnerException != null)
                {
                    exceptionEvent.Message = exception.InnerException.Message;
                    exceptionEvent.StackTrace = exception.InnerException.StackTrace;
                }

                await this.eventTrace.TraceEvent<ExceptionEventEntity>(exceptionEvent, CancellationToken.None);
            }
        }
    }
}
