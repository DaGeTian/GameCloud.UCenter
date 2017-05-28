using System.Collections.Generic;
using GameCloud.UCenter.Common.Portable.Contracts;

namespace GameCloud.UCenter.Common.Portable.Resource
{
    internal static class UCenterResourceManager
    {
        private const string GeneralErrorMessage = "Uknown server error.";

        private static readonly Dictionary<UCenterErrorCode, string> errorMessages =
            new Dictionary<UCenterErrorCode, string>();

        static UCenterResourceManager()
        {
            errorMessages.Add(UCenterErrorCode.InvalidAccountName, @"In valid account name.");
            errorMessages.Add(UCenterErrorCode.InvalidAccountPassword, @"Password length should be greater than 6 and less than 32.");
            errorMessages.Add(UCenterErrorCode.InvalidAccountEmail, @"In valid account email.");
            errorMessages.Add(UCenterErrorCode.InvalidAccountPhone, @"In valid account phone.");
            errorMessages.Add(UCenterErrorCode.DeviceInfoNull, @"Device infomation is missing.");
            errorMessages.Add(UCenterErrorCode.DeviceIdNull, @"Device id can not be null or empty.");
            errorMessages.Add(UCenterErrorCode.AccountPasswordUnauthorized, @"Account name and password not match.");
            errorMessages.Add(UCenterErrorCode.AccountTokenUnauthorized, @"Account token invalid or expired.");
            errorMessages.Add(UCenterErrorCode.AppTokenUnauthorized, @"App token invalid or expired.");
            errorMessages.Add(UCenterErrorCode.AccountDisabled, @"Account is disabled.");
            errorMessages.Add(UCenterErrorCode.AccountNotExist, @"Account does not exist.");
            errorMessages.Add(UCenterErrorCode.AppNotExists, @"App does not exist.");
            errorMessages.Add(UCenterErrorCode.AccountNameAlreadyExist, @"Account name already exists");
            errorMessages.Add(UCenterErrorCode.AppNameAlreadyExist, @"App name already exists");
            errorMessages.Add(UCenterErrorCode.InternalDatabaseError, @"Internal database error");
            errorMessages.Add(UCenterErrorCode.InternalHttpServerError, @"Internal http server error.");
            errorMessages.Add(UCenterErrorCode.ServiceUnavailable, @"Server too busy.");
        }

        public static string GetErrorMessage(UCenterErrorCode errorCode)
        {
            if (errorMessages.ContainsKey(errorCode))
            {
                return errorMessages[errorCode];
            }
            return GeneralErrorMessage;
        }
    }
}