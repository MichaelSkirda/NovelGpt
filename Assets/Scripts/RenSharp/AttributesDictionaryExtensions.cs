using Assets.Scripts.RenSharp.Models.CommandAttributes;
using RenSharp.Models.CommandAttributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace RenSharp
{
    public static class AttributesDictionaryExtensions
    {

        private static string BasicColor = "#FFDAED";

		public static string GetName(this Dictionary<string, CommandAttribute> attributes)
        {
            if (attributes == null)
                return null;

            DisplayNameAttribute? displayName = attributes.GetAttributeOrDefault<DisplayNameAttribute>();

            if (displayName != null)
                return displayName.Name;
            else
                return "";
        }

        public static int GetDelay(this Dictionary<string, CommandAttribute> attributes)
        {
            if (attributes == null)
                return 0;
            DelayAttribute? commandDelay = attributes.GetAttributeOrDefault<DelayAttribute>();

            if (commandDelay != null)
                return commandDelay.Delay;
            else
                return 0;
        }

        public static bool GetClear(this Dictionary<string, CommandAttribute> attributes)
        {
            if (attributes == null)
                return false;

            CommandNoClear commandNoClear = attributes.GetAttributeOrDefault<CommandNoClear>();

            if (commandNoClear != null)
                return false; // default no clear
            else
                return true;
        }

        public static int GetPosX(this Dictionary<string, CommandAttribute> attributes)
        {
            if (attributes == null)
                return 0;

			PosXAttribute posX = attributes.GetAttributeOrDefault<PosXAttribute>();

            if (posX == null)
                return 0;
            else
                return posX.PosX;
		}
        public static string GetColor(this Dictionary<string, CommandAttribute> attributes)
        {
            ColorAttribute colorAttribute = attributes.GetAttributeOrDefault<ColorAttribute>();

            if (colorAttribute != null)
            {
                return $"<color={colorAttribute.Color}>";
            }
            return "";
        }

        public static T? GetAttributeOrDefault<T>(this Dictionary<string, CommandAttribute> dict)
            where T : CommandAttribute
        {
            Type type = typeof(T);
            string key = AttributeParser.ParseKey(type);

            return GetAttributeOrDefault<T>(dict, key);
        }

        public static CommandAttribute? GetAttributeOrDefault(this Dictionary<string, CommandAttribute> dict, Type type)
        {
            string key = AttributeParser.ParseKey(type);

            dict.TryGetValue(key, out CommandAttribute? attribute);
            return attribute;
        }

        private static T? GetAttributeOrDefault<T>(Dictionary<string, CommandAttribute> dict, string key)
            where T : CommandAttribute
        {
            dict.TryGetValue(key, out CommandAttribute? attribute);
            if (attribute == null)
                return null;

            T? result = attribute as T;
            if (result == null)
                throw new InvalidCastException("Wrong cast");
            return result;
        }
    }
}
