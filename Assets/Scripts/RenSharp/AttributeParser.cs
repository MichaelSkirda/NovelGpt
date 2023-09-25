using Assets.Scripts.RenSharp.Models.CommandAttributes;
using RenSharp.Models.CommandAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenSharp
{
    internal static class AttributeParser
    {

        public static string ParseKey(Type type)
        {
            if (type == typeof(DelayAttribute))
                return "delay";
            else if (type == typeof(CommandNoClear))
                return "no-clear";
            else if (type == typeof(ColorAttribute))
                return "color";
            else if (type == typeof(CharacterAttribute))
                return "character";
            else if (type == typeof(DisplayNameAttribute))
                return "name";
            else if (type == typeof(NoPauseAttribute))
                return "no-pause";
            else if (type == typeof(PosXAttribute))
                return "posx";
            else
                throw new Exception("Unknown attribute");
        }

        internal static Dictionary<string, CommandAttribute> ParseMessageAttributes(string line)
        {
            int endOfMessageIndex = line.IndexOf('\"', 1) + 1;
            if (endOfMessageIndex >= line.Length)
                return new Dictionary<string, CommandAttribute>();

            string attrStr = line.Substring(endOfMessageIndex).Trim();
            string[] attributes = attrStr.Split(' ');

            return ParseAttributes(attributes);
        }


		internal static string ParseArgumentOrEmptyString(string line)
        {
            try
            {
                string result = line.Split(' ')[1];
                return result;
            }
            catch(IndexOutOfRangeException)
            {
                return "";
            }
        }

		internal static Dictionary<string, CommandAttribute> ParseConfigAttributes(string line)
		{
			string[] attributes = line.Split(' ').Skip(1).ToArray();

			return ParseAttributes(attributes);
		}

		internal static Dictionary<string, CommandAttribute> ParseValueAttributes(string line)
		{
			string[] attributes = line.Split(' ').Skip(2).ToArray();

			return ParseAttributes(attributes);
		}

		public static Dictionary<string, CommandAttribute> ParseAttributes(string[] attributes)
        {
            var result = new Dictionary<string, CommandAttribute>();
            if (attributes == null)
                return result;

            foreach (string attribute in attributes)
            {
                if (attribute.Trim() == "")
                    continue;
                KeyValuePair<string, CommandAttribute> pair = ParseKeyValue(attribute);
                result.Add(pair.Key, pair.Value);
            }

            return result;
        }

        private static KeyValuePair<string, CommandAttribute> ParseKeyValue(string attribute)
        {
            // delay=1 -> [key: delay, value: 1]
            // no-clear -> [key: no-clear, value: true]
            // callback=somefunc -> [key: callback, value: somefunc]
            string[] keyValue = attribute.Split('=');

            var key = keyValue[0];
            var value = ParseValue(keyValue);
           
            return new KeyValuePair<string, CommandAttribute>(key, value);
        }

        private static CommandAttribute ParseValue(string[] keyValue)
        {
            var key = keyValue[0];

            if (key == "no-clear")
                return new CommandNoClear(value: true);

            if (key == "no-pause")
                return new NoPauseAttribute(value: true);

            if (key == "delay")
            {
                string str = keyValue[1];
                int delay = int.Parse(str);
                return new DelayAttribute(delay);
            }

            if(key == "color")
            {
                string color = keyValue[1];
                return new ColorAttribute(color);
            }

            if (key == "character")
            {
                string name = keyValue[1];
                return new CharacterAttribute(name);
            }

            if(key == "posx")
            {
                int posx = Int32.Parse(keyValue[1]);
                return new PosXAttribute(posx);
            }

            if(key == "name")
            {
                string name = keyValue[1];
                return new DisplayNameAttribute(name);
            }

            throw new Exception($"Cannot parse key {key}");
        }

    }
}
