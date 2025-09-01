using System;
using System.Collections.Generic;
using System.Text;

using OceanApocalypseStudios.RSML.Toolchain.Compliance;


namespace OceanApocalypseStudios.RSML.Analyzer.Syntax
{

	/// <summary>
	/// The officially maintained lexer/tokenizer for RSML v2.0.0.
	/// </summary>
	public sealed class Lexer : ILexer
	{

		private const string ApiVersion = "2.0.0";

		/// <inheritdoc />
		public static SpecificationCompliance SpecificationCompliance => SpecificationCompliance.CreateFull(ApiVersion);

		/// <inheritdoc />
		public static string CreateDocumentFromTokens(IEnumerable<SyntaxToken> tokens)
		{

			StringBuilder builder = new();

			foreach (var t in tokens)
			{

				if (t.Kind == TokenKind.Eof)
					break;

				// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
				switch (t.Kind)
				{

					case TokenKind.Eol:
						_ = builder.AppendLine();

						continue;

					case TokenKind.SpecialActionSymbol:
						_ = builder.Append('@');

						continue;

					case TokenKind.LogicPathValue:
						_ = builder.Append(t.Value);

						continue;

					default:
						_ = builder.Append(t.Value);
						_ = builder.Append(' ');

						break;

				}

			}

			return builder.ToString();

		}

		/// <inheritdoc />
		public static IEnumerable<SyntaxToken> TokenizeLine(string line)
		{

			int pos = 0;
			SkipWhitespace(line, ref pos);

			if (pos >= line.Length)
			{

				yield return new(TokenKind.Eol, Environment.NewLine);

				yield break;

			}

			switch (line[pos])
			{

				case '#':
					yield return new(TokenKind.CommentSymbol, '#');
					yield return new(TokenKind.CommentText, line.AsSpan()[++pos..]);
					yield return new(TokenKind.Eol, Environment.NewLine);

					yield break;

				case '@':
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
					var retVal = ReadQuotedString(line, ref pos);

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
		public static SyntaxToken? TokenizeLogicPathComponent(ReadOnlySpan<char> line, ref int pos)
		{

			SkipWhitespace(line, ref pos);
			int currentPosVal = pos;
			var chars = ReadUntilWhitespaceOrEol(line, ref pos);

			if (chars.IsEquals("any"))
				return new(TokenKind.WildcardKeyword, "any");

			if (chars.IsEquals("defined"))
				return new(TokenKind.DefinedKeyword, "defined");

			if (chars.IsAsciiEqualsIgnoreCase_10(
					"windows", "osx", "linux", "freebsd", "debian",
					"ubuntu", "archlinux", "fedora"
				))
				return new(TokenKind.SystemName, chars);

			if (chars.IsAsciiEqualsIgnoreCase_5("x64", "x86", "arm32", "arm64", "loongarch64"))
				return new(TokenKind.ArchitectureIdentifier, chars);

			if (Int32.TryParse(chars, out int result))
				return new(TokenKind.MajorVersionId, result.ToString());

			if (chars.IsEquals_8(
					"==", "!=", "<", ">", "<=",
					">="
				))
			{

				if (chars.IsEquals("=="))
					return new(TokenKind.Equals, chars);

				if (chars.IsEquals("!="))
					return new(TokenKind.Different, chars);

				if (chars.IsEquals(">"))
					return new(TokenKind.GreaterThan, chars);

				if (chars.IsEquals("<"))
					return new(TokenKind.LessThan, chars);

				if (chars.IsEquals(">="))
					return new(TokenKind.GreaterOrEqualsThan, chars);

				if (chars.IsEquals("<="))
					return new(TokenKind.LessOrEqualsThan, chars);

			}

			if (chars[0] != '"')
				return new(TokenKind.UndefinedToken, chars);

			pos = currentPosVal;

			return null;

		}

		#region Helpers

		private static ReadOnlySpan<char> ReadQuotedString(ReadOnlySpan<char> line, ref int pos)
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

			return line[start..pos]; // ignores last double quote

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
