﻿using System.Collections.Generic;

 namespace Glue.Delivery.Core.Models
{
    public class ServiceObjectResult<TResultType> : ServiceResult
    {
        protected ServiceObjectResult(TResultType result)
        {
            Result = result;
        }

        protected ServiceObjectResult(TResultType result, ICollection<string> errors) : base(errors)
        {
            Result = result;
        }
        
        public TResultType Result { get; }
        public override bool Success => Errors == null && Result != null;

        public static ServiceObjectResult<TResultType> Succeeded(TResultType result)
        {
            return new ServiceObjectResult<TResultType>(result);
        }
        
        public static ServiceObjectResult<TResultType> Failed(TResultType result, ICollection<string> errors)
        {
            return new ServiceObjectResult<TResultType>(result, errors ?? new List<string>());
        }
        
        public static ServiceObjectResult<TResultType> Failed(TResultType result, string error)
        {
            return new ServiceObjectResult<TResultType>(result, new List<string>{ error });
        }
    }
}