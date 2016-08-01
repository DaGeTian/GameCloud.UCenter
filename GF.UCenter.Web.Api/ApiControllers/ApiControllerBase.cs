namespace GF.UCenter.Web.Api.ApiControllers
{
    using System.ComponentModel.Composition;
    using System.Web.Http;
    using Common;
    using Database;
    using UCenter.Common.Portable.Contracts;


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
        protected IHttpActionResult CreateSuccessResult<TResult>(TResult result)
        {
            return this.Ok(new UCenterResponse<TResult> { Status = UCenterResponseStatus.Success, Result = result });
        }
    }
}