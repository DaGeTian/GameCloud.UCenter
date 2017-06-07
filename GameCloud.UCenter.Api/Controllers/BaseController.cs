using System.ComponentModel.Composition;
using GameCloud.UCenter.Common.Portable.Contracts;
using GameCloud.UCenter.Database;
using Microsoft.AspNetCore.Mvc;

namespace GameCloud.UCenter
{
    [Export]
    [ExceptionFilter]
    public class BaseController : Controller
    {
        //---------------------------------------------------------------------
        public UCenterDatabaseContext Database { get; private set; }

        //---------------------------------------------------------------------
        [ImportingConstructor]
        public BaseController(UCenterDatabaseContext database)
        {
            this.Database = database;
        }

        //---------------------------------------------------------------------
        protected IActionResult CreateSuccessResult<TResult>(TResult result)
        {
            return this.Ok(new UCenterResponse<TResult> { Status = UCenterResponseStatus.Success, Result = result });
        }
    }
}