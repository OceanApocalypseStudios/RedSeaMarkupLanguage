using System;
using System.Text;

using RSML.Language;
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
		public string Trim(ReadOnlySpan<char> document, in LanguageStandard languageStandard) => Trim(new RsReader(document), languageStandard);

		/// <summary>
		/// Trims a reader's content.
		/// </summary>
		/// <param name="reader">The reader</param>
		/// <param name="languageStandard">The language standard</param>
		/// <returns>A trimmed RSML document</returns>
		public static string Trim(RsReader reader, in LanguageStandard languageStandard)
		{

			StringBuilder builder = new();
			RsTokenizer tokenizer = new();

			while (reader.TryReadAndTokenizeLine(tokenizer, languageStandard, out var tokens))
			{

				switch (tokens.Length)
				{

					case 0:
					case 1:
						// only valid for comments and eof and eol and malformed lines because:
						// special actions take 2 (handler + name) or even 3 (handler + name + arg)
						// logic paths take 3 (left + op + right)
						continue;

					case 2:
						// special action with no arg
						if (tokens[0].Type != RsTokenType.SpecialActionHandler || tokens[1].Type != RsTokenType.SpecialActionName)
							continue;

						_ = builder.AppendLine($"{String.Join("", tokens[0].Value)}{String.Join("", tokens[1].Value)}");

						continue;

					case 3:
						// special actions with argument or like logic paths
						if (tokens[0].Type == RsTokenType.SpecialActionHandler)
						{

							if (tokens[1].Type != RsTokenType.SpecialActionName)
								continue;

							_ = builder.AppendLine(
								$"{String.Join("", tokens[0].Value)}{String.Join("", tokens[1].Value)}{String.Join("", tokens[2].Value)}"
							);

							continue;

						}

						if (tokens[0].Type != RsTokenType.LogicPathLeft)
							continue;

						if (tokens[1].Type != RsTokenType.PrimaryOperator &&
							tokens[1].Type != RsTokenType.SecondaryOperator &&
							tokens[1].Type != RsTokenType.TertiaryOperator)
							continue;

						if (tokens[2].Type != RsTokenType.LogicPathRight)
							continue;

						if (tokens[2].Value.Length < 3)
							continue;

						if (tokens[2].Value[0] != '"' || tokens[2].Value[^1] != '"')
							continue;

						_ = builder.AppendLine(
							$"{String.Join("", tokens[0].Value)}{String.Join("", tokens[1].Value)}{String.Join("", tokens[2].Value)}"
						);

						continue;

					default:
						continue; // 4 tokens would be unprecedent, but still

				}

			}

			return builder.ToString();

		}

	}

}
