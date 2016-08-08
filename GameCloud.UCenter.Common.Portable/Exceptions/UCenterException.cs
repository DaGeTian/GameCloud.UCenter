using System;
using GameCloud.UCenter.Common.Portable.Contracts;
using GameCloud.UCenter.Common.Portable.Resource;

namespace GameCloud.UCenter.Common.Portable.Exceptions
{
    public class UCenterException : ApplicationException
    {
        public UCenterException(UCenterErrorCode errorCode, string message, Exception innerException = null)
            : base(message, innerException)
        {
            this.ErrorCode = errorCode;
        }

        public UCenterException(UCenterErrorCode errorCode, Exception innerException = null)
            : this(errorCode, UCenterResourceManager.GetErrorMessage(errorCode), innerException)
        {
        }

        public UCenterErrorCode ErrorCode { get; private set; }
    }
}