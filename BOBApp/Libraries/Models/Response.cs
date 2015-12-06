using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libraries.Models
{
    public class Response
    {
        public Boolean Success { get; set; }
        public string Error { get; set; }

        public static implicit operator Task<object>(Response v)
        {
            throw new NotImplementedException();
        }
    }
}
