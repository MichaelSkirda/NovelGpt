using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Gpt
{
    public class Request
    {
        public string model { get; set; }
        public List<Message> messages { get; set; }
        public float temperature { get; set; }
	}
}
