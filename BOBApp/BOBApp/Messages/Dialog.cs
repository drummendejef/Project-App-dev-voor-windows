using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.Messages
{
    public class Dialog
    {
      
        public string Message { get; internal set; }
        public string Ok { get; internal set; }
        public string Nok { get; internal set; }
        public Type ViewOk { get; internal set; }
        public Type ViewNok { get; internal set; }
        public object ParamView { get; internal set; }
        public string Cb { get; internal set; }
    }
}
