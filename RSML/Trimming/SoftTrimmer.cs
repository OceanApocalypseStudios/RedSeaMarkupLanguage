using System;
using System.Text;

using RSML.Reader;
using RSML.Tokenization;


namespace RSML.Trimming
{

	/// <summary>
	/// A trimmer that removes all comments, but does not touch valid lines, even
	/// if they're redundant.
	/// </summary>
	public class SoftTrimmer : ITrimmer
	{

		/// <summary>
		/// Creates a new instance of a soft trimmer.
		/// </summary>
		public SoftTrimmer() { }

		/// <inheritdoc />
		public string Trim(ReadOnlySpan<char> document) => Trim(new RsReader(document));

		/// <summary>
		/// Trims a reader's content.
		/// </summary>
		/// <param name="reader">The reader</param>
		/// <returns>A trimmed RSML document</returns>
		public static string Trim(RsReader reader)
		{

			StringBuilder builder = new();
			RsLexer lexer = new();

			while (reader.TryReadAndTokenizeLine(lexer, out var tokens))
			{

				switch (tokens.Length)
				{

					case 3:
						// comment, ignore it
						continue;

					case 4:
					case 6:
					case 7:
						_ = builder.AppendLine(lexer.CreateDocumentFromTokens(tokens));
						continue;

					default:
						// ignore it
						continue;

				}

			}

			return builder.ToString();

		}

	}

}
