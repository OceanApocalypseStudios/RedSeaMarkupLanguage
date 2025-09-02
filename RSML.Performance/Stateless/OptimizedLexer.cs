using System;

using OceanApocalypseStudios.RSML.Analyzer.Syntax;
using OceanApocalypseStudios.RSML.Performance.Value;
using OceanApocalypseStudios.RSML.Toolchain.Compliance;


namespace OceanApocalypseStudios.RSML.Performance.Stateless
{

	/// <summary>
	/// An optimized, stateless lexer.
	/// </summary>
	public static class OptimizedLexer
	{

		/// <summary>
		/// The level of compliance, per feature, with the official language standard for the current API version.
		/// </summary>
		public static SpecificationCompliance SpecificationCompliance => SpecificationCompliance.CreateFull("2.0.0");

		/// <summary>
		/// Tokenizes a line of RSML.
		/// </summary>
		/// <param name="line">The line</param>
		/// <returns>A container for up to 8 tokens</returns>
		public static SyntaxLine TokenizeLine(ReadOnlySpan<char> line)
		{

			int pos = 0;
			SkipWhitespace(line, ref pos);

			if (pos >= line.Length)
				return new(new(TokenKind.Eol, Environment.NewLine));

			switch (line[pos])
			{

				case '#':
					return new(new(TokenKind.CommentSymbol, '#'), new(TokenKind.CommentText, line[++pos..]), new(TokenKind.Eol, Environment.NewLine));

				case '@':
					++pos; // advance to ignore the #
					var actionName = ReadUntilWhitespaceOrEol(line, ref pos);
					var argument = ReadUntilWhitespaceOrEol(line, ref pos);

					return new(
						new(TokenKind.SpecialActionSymbol, '@'), new(TokenKind.SpecialActionName, actionName.ToString()),
						new(TokenKind.SpecialActionArgument, argument.ToString()), new(TokenKind.Eol, Environment.NewLine),
						ValueToken.Empty
					);

			}

			var op = ReadUntilWhitespaceOrEol(line, ref pos);
			SyntaxLine logicLine = new();

			if (op.IsEquals("->"))
				logicLine.Add(new(TokenKind.ReturnOperator, "->"));

			else if (op.IsEquals("!>"))
				logicLine.Add(new(TokenKind.ThrowErrorOperator, "!>"));

			while (pos < line.Length)
			{

				if (line[pos] == '"')
				{

					pos++; // ignore the double quote

					logicLine.Add(new(TokenKind.LogicPathValue, ReadQuotedString(line, pos, out pos)));
					logicLine.Add(new(TokenKind.Eol, Environment.NewLine));

					break;

				}

				logicLine.Add(TokenizeLogicPathComponent(line, pos, out pos));

				if (logicLine[logicLine.Last()].IsEmpty())
					logicLine.Remove(logicLine.Last());

			}

			return logicLine;

		}

		/// <summary>
		/// Tokenizes a logic path's component.
		/// </summary>
		/// <param name="line">The line</param>
		/// <param name="pos">The index position</param>
		/// <param name="position">The position of the index, after it's assigned</param>
		/// <returns>A token</returns>
		public static ValueToken TokenizeLogicPathComponent(ReadOnlySpan<char> line, int pos, out int position)
		{

			SkipWhitespace(line, ref pos);
			position = pos;
			var chars = ReadUntilWhitespaceOrEol(line, ref pos);

			if (chars.IsEquals("any"))
				return new(TokenKind.WildcardKeyword, "any");

			if (chars.IsEquals("defined"))
				return new(TokenKind.DefinedKeyword, "defined");

			var str = chars.ToString();

			if (Int32.TryParse(chars, out int result))
				return new(TokenKind.MajorVersionId, result.ToString());

			if (chars.IsAsciiEqualsIgnoreCase_10(
					"windows", "osx", "linux", "freebsd", "debian",
					"ubuntu", "archlinux", "fedora"
				))
				return new(TokenKind.SystemName, str);

			if (chars.IsAsciiEqualsIgnoreCase_5("x64", "x86", "arm32", "arm64", "loongarch64"))
				return new(TokenKind.ArchitectureIdentifier, str);

			if (chars.IsEquals_8(
					"==", "!=", "<", ">", "<=",
					">="
				))
			{

				if (chars.IsEquals("=="))
					return new(TokenKind.EqualTo, str);

				if (chars.IsEquals("!="))
					return new(TokenKind.NotEqualTo, str);

				if (chars.IsEquals(">"))
					return new(TokenKind.GreaterThan, str);

				if (chars.IsEquals("<"))
					return new(TokenKind.LessThan, str);

				if (chars.IsEquals(">="))
					return new(TokenKind.GreaterThanOrEqualTo, str);

				if (chars.IsEquals("<="))
					return new(TokenKind.LessThanOrEqualTo, str);

			}

			return chars[0] != '"' ? new(TokenKind.UndefinedToken, str) : ValueToken.Empty;

		}

		#region Helpers

		private static ReadOnlySpan<char> ReadQuotedString(ReadOnlySpan<char> line, int pos, out int position)
		{

			int start = pos;
			int finalQuoteIndex = line[pos..].LastIndexOf('"');

			position = start;

			if (finalQuoteIndex == -1)
				return "";

			finalQuoteIndex += pos; // absolute count

			while (pos < line.Length)
			{

				if (pos == finalQuoteIndex)
					break;

				pos++;

			}

			position = pos;
			return line[start..position]; // ignores last double quote

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
