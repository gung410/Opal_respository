using System;
using System.Collections.Generic;
using System.Linq;

namespace Microservice.Learner.Application.Common.Utils
{
    public static class EnumUtil
    {
        public static List<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }
    }
}
