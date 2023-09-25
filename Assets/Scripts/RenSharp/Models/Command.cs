using RenSharp.Models.CommandAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenSharp.Models
{
    public class Command
    {
        public CommandType Type { get; set; }
        public string Value { get; set; } = "";
        public int Line { get; set; }
        public Dictionary<string, CommandAttribute> Attributes { get; set; } = new Dictionary<string, CommandAttribute>();
    }
}
