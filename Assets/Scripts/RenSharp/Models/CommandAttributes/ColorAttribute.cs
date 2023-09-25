using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenSharp.Models.CommandAttributes
{
    public class ColorAttribute : CommandAttribute
    {
        public string Color { get; set; } = "white";
        public ColorAttribute(string color)
        { 
            Color = color;
        }
    }
}
