using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.DocumentModel;

namespace Glue.Delivery.Data
{
    public class QueryModel
    {
        public QueryModel(string fieldName, ScanOperator operatorType, params object[] values)
        {
            if(!values.Any())
                throw new ArgumentException("Field is mandatory", nameof(values));
            
            FieldName = fieldName ?? throw new ArgumentException("Field is mandatory", nameof(fieldName));
            Values = values;
            Operator = operatorType;
        }
        
        public string FieldName { get; }
        public ScanOperator Operator { get; }
        public object[] Values { get; }
    }
}