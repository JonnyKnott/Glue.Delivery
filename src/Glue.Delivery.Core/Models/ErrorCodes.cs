namespace Glue.Delivery.Core.Models
{
    public static class ErrorCodes
    {
        public static class Messages
        {
            public const string InvalidStateProgression =
                "The delivery state cannot be set. The desired value is invalid";
        }
        public static class Status
        {
            public const string BadRequest = "BadRequest";
            public const string NotFound = "Resource Not Found";
            public const string UnexpectedError = "UnexpectedError";
        }
    }
}