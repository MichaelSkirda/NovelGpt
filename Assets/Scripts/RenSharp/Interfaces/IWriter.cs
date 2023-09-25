using RenSharp.Models.CommandAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenSharp.Interfaces
{
    public interface IWriter
    {
        public void Write(string message, Dictionary<string, CommandAttribute> attributes);
    }
}
