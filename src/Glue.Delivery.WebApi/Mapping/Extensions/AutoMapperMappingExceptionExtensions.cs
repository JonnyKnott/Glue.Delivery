using System.Linq;
using AutoMapper;

namespace Glue.Delivery.WebApi.Mapping.Extensions
{
    public static class AutoMapperMappingExceptionExtensions
    {
        public static object FailedFields(this AutoMapperMappingException mappingException)
        {
            return mappingException.MemberMap.SourceMembers.Select(x => x.Name).ToList();
            
        } 
    }
}