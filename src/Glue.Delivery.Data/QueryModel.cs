using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.DocumentModel;

namespace Glue.Delivery.Data
{
    public class QueryModel
    {
        public QueryModel(string fieldName, ScanOperator operatorType, object value)
        {
            if(value == null)
                throw new ArgumentException("Field is mandatory", nameof(value));
            
            FieldName = fieldName ?? throw new ArgumentException("Field is mandatory", nameof(fieldName));
            Value = value;
            Operator = operatorType;
        }
        
        public string FieldName { get; }
        public ScanOperator Operator { get; }
        public object Value { get; }
    }
}