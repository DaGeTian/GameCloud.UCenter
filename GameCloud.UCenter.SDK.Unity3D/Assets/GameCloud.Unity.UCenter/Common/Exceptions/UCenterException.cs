// Copyright(c) Cragon.All rights reserved.

namespace GameCloud.UCenter
{
    using System;

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