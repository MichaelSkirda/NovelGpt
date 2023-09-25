using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenSharp.Models.CommandAttributes
{
    public class DelayAttribute : CommandAttribute
    {
        public int Delay { get; set; }
        public DelayAttribute(int delay)
        {
            Delay = delay;
        }
    }
}
