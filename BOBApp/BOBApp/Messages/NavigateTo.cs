using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOBApp.Messages
{
    public class NavigateTo
    {
        public bool Reload { get; internal set; }
        public string Name { get; internal set; }
        public Type View { get; internal set; }

        public object Result { get; internal set; }
        public object ParamView { get; internal set; }
        public object Data { get; internal set; }

    }
}
