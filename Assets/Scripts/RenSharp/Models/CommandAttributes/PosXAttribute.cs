using RenSharp.Models.CommandAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.RenSharp.Models.CommandAttributes
{
	public class PosXAttribute : CommandAttribute
	{
		public int PosX { get; set; }
		public PosXAttribute(int posX)
		{
			PosX = posX;
		}
	}
}
