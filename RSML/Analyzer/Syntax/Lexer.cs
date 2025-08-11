using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;


namespace RSML.Analyzer.Syntax
{

	/// <summary>
	/// The officially maintained lexer/tokenizer for RSML v2.0.0.
	/// </summary>
	public sealed class Lexer : ILexer
	{

		/// <inheritdoc />
		public string StandardizedVersion => "2.0.0";

		/// <inheritdoc />
		public ImmutableHashSet<string> ValidComparators => [ "==", "!=", "<", ">", "<=", ">=" ];

		/// <inheritdoc />
		public string CreateDocumentFromTokens(IEnumerable<SyntaxToken> tokens)
		{

			StringBuilder builder = new();

			foreach (var t in tokens)
			{

				if (t.Kind == TokenKind.Eof)
					break;

				if (t.Kind == TokenKind.Eol)
				{

					_ = builder.AppendLine();

					continue;

				}

				if (t.Kind == TokenKind.SpecialActionSymbol)
				{

					_ = builder.Append('@');

					continue;

				}

				if (t.Kind == TokenKind.LogicPathValue)
				{

					_ = builder.Append(t.Value);

					continue;

				}

				_ = builder.Append(t.Value);
				_ = builder.Append(' ');

			}

			return builder.ToString();

		}

		/// <inheritdoc />
		public IEnumerable<SyntaxToken> TokenizeLine(string line)
		{

			int pos = 0;
			SkipWhitespace(line, ref pos);

			if (pos >= line.Length)
			{

				yield return new(TokenKind.Eol, Environment.NewLine);

				yield break;

			}

			if (line[pos] == '#')
			{

				yield return new(TokenKind.CommentSymbol, '#');
				yield return new(TokenKind.CommentText, line[++pos..]);
				yield return new(TokenKind.Eol, Environment.NewLine);

				yield break;

			}

			if (line[pos] == '@')
			{

				yield return new(TokenKind.SpecialActionSymbol, '@');

				++pos; // advance to ignore the #
				var actionName = ReadUntilWhitespaceOrEol(line, ref pos);

				yield return new(TokenKind.SpecialActionName, actionName);

				var argument = ReadUntilWhitespaceOrEol(line, ref pos);

				yield return new(TokenKind.SpecialActionArgument, argument);

				yield return new(TokenKind.Eol, Environment.NewLine);

				yield break;

			}

			var op = ReadUntilWhitespaceOrEol(line, ref pos);

			if (op.IsEquals("->"))
				yield return new(TokenKind.ReturnOperator, op);

			else if (op.IsEquals("!>"))
				yield return new(TokenKind.ThrowErrorOperator, op);

			while (pos < line.Length)
			{

				if (line[pos] == '"')
				{

					pos++; // ignore the double quote
					string retVal = ReadQuotedString(line, ref pos);

					yield return new(TokenKind.LogicPathValue, retVal);
					yield return new(TokenKind.Eol, Environment.NewLine);

					yield break;

				}

				var token = TokenizeLogicPathComponent(line, ref pos);

				if (token is not null)
					yield return (SyntaxToken)token;

			}

		}

		/// <inheritdoc />
		public SyntaxToken? TokenizeLogicPathComponent(ReadOnlySpan<char> line, ref int pos)
		{

			SkipWhitespace(line, ref pos);
			int currentPosVal = pos;
			var chars = ReadUntilWhitespaceOrEol(line, ref pos);

			if (chars.IsEquals("any"))
				return new(TokenKind.WildcardKeyword, "any");

			if (chars.IsEquals("defined"))
				return new(TokenKind.DefinedKeyword, "defined");

			if (chars.IsEquals(
					StringComparison.OrdinalIgnoreCase, "windows", "osx", "linux", "freebsd",
					"debian", "ubuntu", "archlinux", "fedora"
				))
				return new(TokenKind.SystemName, chars);

			if (chars.IsEquals(
					StringComparison.OrdinalIgnoreCase, "x64", "x86", "arm32", "arm64",
					"loongarch64"
				))
				return new(TokenKind.ArchitectureIdentifier, chars);

			if (Int32.TryParse(chars, out int result))
				return new(TokenKind.MajorVersionId, result.ToString());

			string str = chars.ToString();

			if (ValidComparators.Contains(str))
			{

				#region Pragmas

				#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

				#endregion

				return str switch
				{

					"==" => new(TokenKind.Equals, str),
					"!=" => new(TokenKind.Different, str),
					">"  => new(TokenKind.GreaterThan, str),
					"<"  => new(TokenKind.LessThan, str),
					">=" => new(TokenKind.GreaterOrEqualsThan, str),
					"<=" => new(TokenKind.LessOrEqualsThan, str)

				};

				#region Pragmas

				#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

				#endregion

			}

			if (str[0] == '"')
			{

				pos = currentPosVal;
				return null;

			}

			return new(TokenKind.UndefinedToken, str);

		}

		#region Helpers

		private static string ReadQuotedString(ReadOnlySpan<char> line, ref int pos)
		{

			int start = pos;
			int finalQuoteIndex = line[pos..].LastIndexOf('"');

			if (finalQuoteIndex == -1)
				return "";

			finalQuoteIndex += pos; // absolute count

			while (pos < line.Length)
			{

				if (pos == finalQuoteIndex)
					break;

				pos++;

			}

			return line[start..pos].ToString(); // ignores last double quote

		}

		private static ReadOnlySpan<char> ReadUntilWhitespaceOrEol(ReadOnlySpan<char> line, ref int pos)
		{

			int start = pos;

			while (pos < line.Length && !Char.IsWhiteSpace(line[pos]))
				pos++;

			return line[start..pos];

		}

		private static void SkipWhitespace(ReadOnlySpan<char> chars, ref int pos)
		{

			while (pos < chars.Length && Char.IsWhiteSpace(chars[pos]))
				pos++;

		}

		#endregion

	}

}
