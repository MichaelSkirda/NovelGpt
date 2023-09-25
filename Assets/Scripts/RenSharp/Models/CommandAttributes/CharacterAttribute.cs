using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenSharp.Models.CommandAttributes
{
    public class CharacterAttribute : CommandAttribute
    {
        public string Name { get; set; }
        public CharacterAttribute(string name)
        {
            Name = name;
        }
    }
}
