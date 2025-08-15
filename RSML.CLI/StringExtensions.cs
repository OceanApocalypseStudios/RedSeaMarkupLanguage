using System;


namespace RSML.CLI
{

	internal static class StringExtensions
	{

		internal static string Capitalize(this string str) =>
			String.Create(
				str.Length, str, (span, src) =>
				{

					for (int i = 1; i < src.Length; i++)
						span[i] = Char.ToLowerInvariant(src[i]);

					span[0] = Char.ToUpperInvariant(src[0]);

				}
			);

	}

}
