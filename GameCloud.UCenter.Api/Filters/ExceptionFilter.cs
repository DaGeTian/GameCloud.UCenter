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
using System.ComponentModel.Composition.Hosting;
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
            context.Result = new JsonResult(exception.Message);

            if (context.Exception != null)
            {
                var exceptionEvent = new ExceptionEventEntity()
                {
                    ExceptionMessage = context.Exception.Message,
                    ExceptionStackTrace = context.Exception.StackTrace
                };

                await this.eventTrace.TraceEvent<ExceptionEventEntity>(exceptionEvent, CancellationToken.None);

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
    }
}
