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

    }
}
