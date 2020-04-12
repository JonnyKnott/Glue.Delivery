using System.Collections;
using System.Collections.Generic;
using Glue.Delivery.Core.Models;

namespace Glue.Delivery.Models.ApiModels
{
    public class ErrorResponse
    {
        public ErrorResponse(ICollection<string> errors)
        {
            Errors = errors;
        }
        public ICollection<string> Errors { get; }
    }
}