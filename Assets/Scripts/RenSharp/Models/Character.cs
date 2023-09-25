using RenSharp.Models.CommandAttributes;
using System.Collections.Generic;

namespace RenSharp.Models
{
    public class Character
    {
        public Dictionary<string, CommandAttribute> Attributes { get; set; }
            = new Dictionary<string, CommandAttribute>();

        public string Name { get; set; }

        public Character(string name, Dictionary<string, CommandAttribute> attributes)
        {
            Name = name;
            Attributes = attributes;
        }
    }
}