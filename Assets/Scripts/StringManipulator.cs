using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Assets.Scripts
{
	internal static class StringManipulator
	{
		internal static string FormatString(this string str)
		{
			Regex regex = new Regex("[^a-zA-ZА-яЁё -]");
			str = regex.Replace(str, "");
			str = str.ToLower();
			return str;
		}

		internal static string ToWord(this string str)
		{
			string[] arr = str
				.FormatString()
				.Split(' ')
				.OrderBy(x => x)
				.ToArray();

			return string.Join("", arr);
		}
	}
}
