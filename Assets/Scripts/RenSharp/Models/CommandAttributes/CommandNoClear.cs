using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenSharp.Models.CommandAttributes
{
    public class CommandNoClear : CommandAttribute
    {
        public bool Value { get; set; }

        public CommandNoClear(bool value)
        { 
            Value = value;
        }
    }
}
