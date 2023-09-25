using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Gpt
{
    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
	}
}
