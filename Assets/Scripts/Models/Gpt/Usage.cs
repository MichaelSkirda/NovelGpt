using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
	public class Usage
    {
		public int prompt_tokens { get; set; }
		public int completion_tokens { get; set; }
		public int total_tokens { get; set; }
    }
}
