using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Text;


namespace RSML.Tokenization
{

	/// <summary>
	/// The officially maintained lexer/tokenizer for RSML v2.0.0.
	/// </summary>
	public sealed class RsLexer : ILexer
	{

		/// <inheritdoc/>
		public string? StandardizedVersion => "2.0.0";

		/// <inheritdoc/>
		public ImmutableHashSet<string> ValidComparators => [ "==", "!=", "<", ">", "<=", ">=" ];

		/// <inheritdoc/>
		public string CreateDocumentFromTokens(IEnumerable<RsToken> tokens)
		{

			StringBuilder builder = new();

			foreach (var t in tokens)
			{

				if (t.Type == RsTokenType.Eof)
					break;

				if (t.Type == RsTokenType.Eol)
				{

					_ = builder.AppendLine();
					continue;

				}

				if (t.Type == RsTokenType.SpecialActionSymbol)
				{

					_ = builder.Append('@');
					continue;

				}

				if (t.Type == RsTokenType.LogicPathValue)
				{

					_ = builder.Append(t.Value);
					continue;

				}

				_ = builder.Append(t.Value);
				_ = builder.Append(' ');

			}

			return builder.ToString();

		}

		/// <inheritdoc/>
		public IEnumerable<RsToken> TokenizeLine(string line)
		{

			int pos = 0;
			SkipWhitespace(line, ref pos);

			if (pos >= line.Length)
			{

				yield return new(RsTokenType.Eol, Environment.NewLine);
				yield break;

			}

			if (line[ pos ] == '#')
			{

				yield return new(RsTokenType.CommentSymbol, '#');
				yield return new(RsTokenType.CommentText, line[ ++pos.. ]);
				yield return new(RsTokenType.Eol, Environment.NewLine);
				yield break;

			}

			if (line[ pos ] == '@')
			{

				yield return new(RsTokenType.SpecialActionSymbol, '@');

				++pos; // advance to ignore the #
				var actionName = ReadUntilWhitespaceOrEol(line, ref pos);
				yield return new(RsTokenType.SpecialActionName, actionName);

				var argument = ReadUntilWhitespaceOrEol(line, ref pos);
				yield return new(RsTokenType.SpecialActionArgument, argument);

				yield return new(RsTokenType.Eol, Environment.NewLine);
				yield break;

			}

			var op = ReadUntilWhitespaceOrEol(line, ref pos);

			if (op.IsEquals("->"))
				yield return new(RsTokenType.ReturnOperator, op);

			else if (op.IsEquals("!>"))
				yield return new(RsTokenType.ThrowErrorOperator, op);

			while (pos < line.Length)
			{

				if (line[ pos ] == '"')
				{

					pos++; // ignore the double quote
					var retVal = ReadQuotedString(line, ref pos);

					yield return new(RsTokenType.LogicPathValue, retVal);
					yield return new(RsTokenType.Eol, Environment.NewLine);
					yield break;

				}

				var token = TokenizeLogicPathComponent(line, ref pos);

				if (token is not null)
					yield return (RsToken)token;

			}

		}

		/// <inheritdoc/>
		public RsToken? TokenizeLogicPathComponent(ReadOnlySpan<char> line, ref int pos)
		{

			SkipWhitespace(line, ref pos);
			int currentPosVal = pos;
			var chars = ReadUntilWhitespaceOrEol(line, ref pos);

			if (chars.IsEquals("any"))
				return new(RsTokenType.WildcardKeyword, "any");

			if (chars.IsEquals("defined"))
				return new(RsTokenType.DefinedKeyword, "defined");

			if (chars.IsEquals(StringComparison.OrdinalIgnoreCase, "windows", "osx", "linux", "freebsd", "debian", "archlinux", "fedora", "alpine", "gentoo"))
				return new(RsTokenType.SystemName, chars);

			if (chars.IsEquals(StringComparison.OrdinalIgnoreCase, "x64", "x86", "arm32", "arm64", "loongarch64"))
				return new(RsTokenType.ArchitectureIdentifier, chars);

			if (Int32.TryParse(chars, out int result))
				return new(RsTokenType.MajorVersionId, result.ToString());

			var str = chars.ToString();

			if (ValidComparators.Contains(str))
			{

				#region Pragmas
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
				#endregion

				return str switch
				{

					"==" => new(RsTokenType.Equals, str),
					"!=" => new(RsTokenType.Different, str),
					">" => new(RsTokenType.GreaterThan, str),
					"<" => new(RsTokenType.LessThan, str),
					">=" => new(RsTokenType.GreaterOrEqualsThan, str),
					"<=" => new(RsTokenType.LessOrEqualsThan, str)

				};

				#region Pragmas
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
				#endregion

			}

			pos = currentPosVal; // in case we read what we shouldn't
			return null;

		}

		#region Helpers

		private static string ReadQuotedString(ReadOnlySpan<char> line, ref int pos)
		{

			int start = pos;
			var finalQuoteIndex = line[ pos.. ].LastIndexOf('"');

			if (finalQuoteIndex == -1)
				return "";

			finalQuoteIndex += pos; // absolute count

			while (pos < line.Length)
			{

				if (pos == finalQuoteIndex)
					break;

				pos++;

			}

			return line[ start..pos ].ToString(); // ignores last double quote

		}

		private static ReadOnlySpan<char> ReadUntilWhitespaceOrEol(ReadOnlySpan<char> line, ref int pos)
		{

			int start = pos;

			while (pos < line.Length && !Char.IsWhiteSpace(line[ pos ]))
				pos++;

			return line[ start..pos ];

		}

		private static void SkipWhitespace(ReadOnlySpan<char> chars, ref int pos)
		{

			while (pos < chars.Length && Char.IsWhiteSpace(chars[ pos ]))
				pos++;

		}

		#endregion

	}

}
