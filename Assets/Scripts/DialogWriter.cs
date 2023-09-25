using RenSharp.Interfaces;
using RenSharp.Models.CommandAttributes;
using RenSharp;
using System.Collections.Generic;
using Assets.Scripts.Models;

namespace Assets.Scripts
{
	internal class DialogWriter : IWriter
	{

		private DialogController Dialog { get; set; }

		public DialogWriter(DialogController dialog)
		{ 
			Dialog = dialog;
		}

		public void Write(string message, Dictionary<string, CommandAttribute> attributes)
		{
			int delay = attributes.GetDelay();
			string name = attributes.GetName();
			string color = attributes.GetColor();
			bool isClear = attributes.GetClear();

			if (name == null)
				name = "";

			var dialogMessage = new DialogMessage()
			{
				Text = message,
				Delay = delay
			};

			if (isClear)
				Dialog.DrawText(dialogMessage);
			else
				Dialog.AppendText(dialogMessage);

			Dialog.SetCharacterName(color + name);
		}
	}
}
