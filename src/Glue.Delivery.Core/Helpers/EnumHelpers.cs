using System;
using System.Collections.Generic;
using System.Linq;

namespace Glue.Delivery.Core.Helpers
{
    public static class EnumHelpers
    {
        public static ICollection<string> GetStringValues<TEnum>()
        where TEnum : Enum
        {
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(x => x.ToString());

            return values.ToList();
        }
    }
}