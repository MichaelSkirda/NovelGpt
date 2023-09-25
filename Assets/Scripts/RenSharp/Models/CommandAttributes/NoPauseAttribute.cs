using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenSharp.Models.CommandAttributes
{
    public class NoPauseAttribute : CommandAttribute
    {
        public bool Value { get; set; }
        public NoPauseAttribute(bool value)
        {
            Value = value;
        }
    }
}
