using System.ComponentModel.Composition;
using System.Web.Http;
using GF.UCenter.Common.Portable;
using GF.UCenter.MongoDB;
using GF.UCenter.Web.Common;

namespace GF.UCenter.Web.Api.ApiControllers
{
    /// <summary>
    /// API controller base class
    /// </summary>
    [Export]
    [ActionExecutionFilter]
    public class ApiControllerBase : ApiController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiControllerBase" /> class.
        /// </summary>
        /// <param name="database">The database context</param>
        [ImportingConstructor]
        public ApiControllerBase(DatabaseContext database)
        {
            this.Database = database;
        }

        public DatabaseContext Database { get; private set; }

        /// <summary>
        /// Create success result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="result">The content of the result</param>
        /// <returns>Http Action result</returns>
        protected IHttpActionResult CreateSuccessResult<TResult>(TResult result)
        {
            return this.Ok(new UCenterResponse<TResult> { Status = UCenterResponseStatus.Success, Result = result });
        }
    }
}