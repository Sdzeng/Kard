using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kard.Core.Dtos
{
    public class ResultDto<T> 
    {
        public bool Result { get; set; }
        public string Message { get; set; }

        public T Data { get; set; }

        public ResultDto<T> Set(bool result, string message = "", T t = default(T))
        {
            Result = result;
            Message = message;
            Data = t;
            return this;
        }
    }

    public class ResultDto : ResultDto<object>
    {

    }

}
