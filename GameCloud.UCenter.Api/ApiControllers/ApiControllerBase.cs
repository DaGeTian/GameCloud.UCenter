using System.ComponentModel.Composition;
using GameCloud.UCenter.Common.Portable.Contracts;
using GameCloud.UCenter.Database;
using GameCloud.UCenter.Web.Common.Filters;
using Microsoft.AspNetCore.Mvc;

namespace GameCloud.UCenter.Web.Api.ApiControllers
{
    /// <summary>
    /// API controller base class
    /// </summary>
    [Export]
    //[ActionExecutionFilter]
    public class ApiControllerBase : Controller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiControllerBase" /> class.
        /// </summary>
        /// <param name="database">The database context</param>
        [ImportingConstructor]
        public ApiControllerBase(UCenterDatabaseContext database)
        {
            this.Database = database;
        }

        /// <summary>
        /// Gets the database context.
        /// </summary>
        public UCenterDatabaseContext Database { get; private set; }

        /// <summary>
        /// Create success result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="result">The content of the result</param>
        /// <returns>Http Action result</returns>
        protected IActionResult CreateSuccessResult<TResult>(TResult result)
        {
            return this.Ok(new UCenterResponse<TResult> { Status = UCenterResponseStatus.Success, Result = result });
        }
    }
}