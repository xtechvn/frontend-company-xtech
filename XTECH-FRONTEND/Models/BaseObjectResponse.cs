using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XTECH_FRONTEND.Models
{
    public class BaseObjectResponse<T>
    {
        public int status { get; set; }
        public string msg { get; set; }
        public List<T> data { get; set; }
    }
    public class BaseObjectResponse
    {
        public int status { get; set; }
        public string msg { get; set; }
      
    }
    public class BaseObjectResponse2<T>
    {
        public int status { get; set; }
        public string msg { get; set; }
        public T data { get; set; }
    }
}
