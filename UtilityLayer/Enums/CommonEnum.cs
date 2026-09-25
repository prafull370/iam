namespace IAM_API.Common.Enums
{
    public class CommonEnum
    {
        public enum ResponseStatus
        {
            TRUE = 1,
            FALSE = 0
        }
        public enum ResponseType
        {
            INTERNALSERVERERROR = 1,
            VALIDATIONFAILLED = 2,
            SUCCESS = 3,
            FAILLED = 4,
            NOTFOUND = 5,
            AuthenticationError = 6,
            AuthorizationError = 7,
            RequestLimitExceeded = 8

        }
        public enum ResponseCode
        {
            SUCCESS = 200,
            CREATED = 201,
            NOTFOUND = 404,
            VALIDATIONFAILLED = 400,
            INTERNALSERVERERROR = 500
        }
        public enum RequestType
        {
            GET = 1,
            INSERT = 2,
            UPDATE = 3,
            DELETE = 4
        }
        public enum Status
        {
            PENDING = 1,
            INPROGRESS = 2,
            COMPLETED = 3,
            CLOSED = 4
        }
        public enum ErrorType
        {
            INTERNALSERVER = 1,
            VALIDATIONFAILLED = 2,
            ERROR = 3,
            UNAUTHORIZED = 4,
            NOTFOUND = 5
        }
        public enum NotificationType
        {
            INFO = 1,
            SUCCESS = 2,
            WARNING = 3,
            ERROR = 4
        }
        public enum RecordStatus
        {
            ACTIVE = 1,
            INACTIVE = 2,
            ARCHIVED = 3,
            DELETED = 4
        }
        public enum PaymentStatus
        {
            PENDING = 1,
            COMPLETED = 2,
            FAILED = 3,
            REFUNDED = 4,
            CANCELED = 5
        }
        public enum LogLevel
        {
            INFO = 1,
            WARNING = 2,
            ERROR = 3,
            CRITICAL = 4,
            DEBUG = 5
        }
        public enum FileType
        {
            DOCUMENT = 1,
            IMAGE = 2,
            VIDEO = 3,
            AUDIO = 4,
            OTHER = 5
        }
        public enum AccessPermission
        {
            READ = 1,
            WRITE = 2,
            UPDATE = 3,
            DELETE = 4,
            FULLACCESS = 5
        }
        public enum EmailTemplate
        {
            WELCOMEEMAIL = 1,
            PASSWORDRESET = 2,
            ACCOUNTACTIVATION = 3,
            NOTIFICATION = 4
        }
    }
}
