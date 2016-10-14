namespace GameCloud.UCenter.Common.Portable.Contracts
{
    public enum UCenterErrorCode
    {
        NoError = 0,

        // Error sends http request on client side
        ClientError = 1,

        // BadRequest - 400
        InvalidAccountName = 400001,
        InvalidAccountPassword = 400002,
        InvalidAccountEmail = 400003,
        InvalidAccountPhone = 400004,
        DeviceInfoNull = 400010,
        DeviceIdNull = 400011,

        // Unauthorized - 401
        AccountPasswordUnauthorized = 401001,
        AccountTokenUnauthorized = 401002,
        AppTokenUnauthorized = 401003,
        AccountDisabled = 401004,

        // NotFound - 404
        AccountNotExist = 404001,
        AppNotExists = 404002,
        OrderNotExists = 404003,

        // Conflict - 409
        AccountNameAlreadyExist = 409001,
        AppNameAlreadyExist = 409002,

        // InternalServerError - 500
        InternalDatabaseError = 500001,
        InternalHttpServerError = 500002,

        // ServiceUnavailable - 503
        ServiceUnavailable = 503001
    }
}