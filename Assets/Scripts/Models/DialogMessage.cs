using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
	public class DialogMessage
	{
		public int Id { get; set; }
		public string Text { get; set; }
		public string CharacterName { get; set; }
		public int Delay { get; set; }
	}
}
