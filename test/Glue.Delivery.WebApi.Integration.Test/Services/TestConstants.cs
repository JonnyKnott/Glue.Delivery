using Amazon.DynamoDBv2.Model;

namespace Glue.Delivery.WebApi.Integration.Test.Services
{
    public static class TestConstants
    {
        public const string InternalErrorValue = "Error";
        public const string BadRequestValue = "BadRequest";
        public const string NotFound = "NotFound";
        public const string SuccessValue = "Success";

        public const string TestJWTSecret = "0D49215F-76DF-471F-9F07-5E1B18BE1150";
    }
    
}