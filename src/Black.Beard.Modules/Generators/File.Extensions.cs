﻿using System.Text;

namespace Bb.Generators
{
    public static class File
    {

        static File()
        {
		
			_invalidChars = new HashSet<char>(Path.GetInvalidFileNameChars());

		}

		/// <summary>Replaces characters in <c>text</c> that are not allowed in 
		/// file names with the specified replacement character.</summary>
		/// <param name="text">Text to make into a valid filename. The same string is returned if it is valid already.</param>
		/// <param name="replacement">Replacement character, or null to simply remove bad characters.</param>
		/// <param name="fancy">Whether to replace quotes and slashes with the non-ASCII characters ” and ⁄.</param>
		/// <returns>A string that can be used as a filename. If the output string would otherwise be empty, returns "_".</returns>
		public static string MakeValidFileName(this string text, char? replacement = '_', bool fancy = true)
		{

			StringBuilder sb = new StringBuilder(text.Length);
			
			bool changed = false;
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				if (_invalidChars.Contains(c))
				{
					changed = true;
					var repl = replacement ?? '\0';
					if (fancy)
					{
						if (c == '"') repl = '”'; // U+201D right double quotation mark
						else if (c == '\'') repl = '’'; // U+2019 right single quotation mark
						else if (c == '/') repl = '⁄'; // U+2044 fraction slash
					}
					if (repl != '\0')
						sb.Append(repl);
				}
				else
					sb.Append(c);
			}
			if (sb.Length == 0)
				return "_";
			return changed ? sb.ToString() : text;
		}

		private static readonly HashSet<char> _invalidChars;

	}
}
