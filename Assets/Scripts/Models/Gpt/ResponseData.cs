using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Gpt
{
	public class ResponseData
    {
        public string id { get; set; }
        public string @object { get; set; }
        public ulong created { get; set; }
        public List<Choice> choices { get; set; }
        public Usage usage { get; set; }
	}
}
