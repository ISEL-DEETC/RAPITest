using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsLibrary.Models.AppSpecific
{
	public class Retained
	{
		public Retained(string Path)
		{
			this.Path = Path;
		}
		public string Path { get; set; }
		public string Value { get; set; }
	}
}
