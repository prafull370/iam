using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Models.Common
{
    public class RefCursorResultModel
    {
        public Type ResultType { get; set; }
        public bool IsList { get; set; }

        public RefCursorResultModel(Type resultType, bool isList)
        {
            ResultType = resultType;
            IsList = isList;
        }
    }

}
