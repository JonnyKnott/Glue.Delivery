using System.Collections.Generic;

namespace Glue.Delivery.Core.Models
{
    public class ServiceResult
    {
        protected ServiceResult()
        {
            
        }
        protected ServiceResult(ICollection<string> errors)
        {
            Errors = errors;
        }

        public virtual bool Success => Errors == null;
        public ICollection<string> Errors { get; } 
        
        public static ServiceResult Succeeded()
        {
            return new ServiceResult();
        }

        public static ServiceResult Failed(ICollection<string> errors)
        {
            return new ServiceResult(errors);
        }

        public static ServiceResult Failed(string error)
        {
            return new ServiceResult(new List<string>{ error });
        }
    }
}