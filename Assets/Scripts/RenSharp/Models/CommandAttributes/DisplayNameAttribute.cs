using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenSharp.Models.CommandAttributes
{
    public class DisplayNameAttribute : CommandAttribute
    {
        public string Name { get; set; }
        public DisplayNameAttribute(string name)
        {
            Name = name;
        }
    }
}
