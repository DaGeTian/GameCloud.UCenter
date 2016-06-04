using System;
using GF.UCenter.Common.Portable.Contracts;
using GF.UCenter.Common.Portable.Resource;

namespace GF.UCenter.Common.Portable.Exceptions
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