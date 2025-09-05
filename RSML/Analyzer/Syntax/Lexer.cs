using System;
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
		private const string QUOTE = "\"";

		/// <inheritdoc />
		public static SpecificationCompliance SpecificationCompliance => SpecificationCompliance.CreateFull(ApiVersion);

		/// <inheritdoc />
		public static string CreateDocumentFromTokens(in SyntaxLine line, DualTextBuffer context)
		{

			StringBuilder builder = new();

			for (int i = 0; i < 8; i++)
			{

				if (line[i].IsEmpty)
					continue;

				if (line[i].Kind == TokenKind.Eof)
					break;

				// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
				switch (line[i].Kind)
				{

					case TokenKind.Eol:
						_ = builder.AppendLine();

						continue;

					case TokenKind.SpecialActionSymbol:
						_ = builder.Append('@');

						continue;

					case TokenKind.LogicPathValue when !line[i].IsOffLimits:
						_ = builder.Append(context[line[i].BufferRange]);

						continue;

					default:
						if (line[i].IsOffLimits)
							continue;

						_ = builder.Append(context[line[i].BufferRange]);
						_ = builder.Append(' ');

						continue;

				}

			}

			return builder.ToString();

		}

		/// <inheritdoc />
		public static SyntaxLine TokenizeLine(DualTextBuffer buffer)
		{

			buffer.SkipWhitespace();
			int pos = buffer.CaretPosition;

			if (buffer.CaretPosition >= buffer.Length)
				return new(new(TokenKind.Eol, ^1, 0));

			switch (buffer[buffer.CaretPosition])
			{

				case '#':
					return new(
						new(TokenKind.CommentSymbol, pos, pos++),
						new(TokenKind.CommentText, pos, buffer.Length),
						new(TokenKind.Eol, ^1, 0)
					);

				case '@':
					_ = buffer.Read();
					_ = buffer.ReadUntilWhitespace(false);
					int beforeWhitespaceRemoval = buffer.CaretPosition;

					buffer.SkipWhitespace();

					int argumentNameStartIdx = buffer.CaretPosition;
					_ = buffer.ReadUntilWhitespace(false);
					int argumentNameEndIdx = buffer.CaretPosition;

					return new(
						new(TokenKind.SpecialActionSymbol, pos, pos + 1),
						new(TokenKind.SpecialActionName, pos + 1, beforeWhitespaceRemoval),
						new(TokenKind.SpecialActionArgument, argumentNameStartIdx, argumentNameEndIdx),
						new(TokenKind.Eol, ^1, 0),
						SyntaxToken.Empty
					);

			}

			var op = buffer.ReadUntilWhitespace(false);
			SyntaxLine line = new();
			line.Clear();

			if (op.IsEquals("->"))
				line.Add(new(TokenKind.ReturnOperator, pos, buffer.CaretPosition));

			else if (op.IsEquals("!>"))
				line.Add(new(TokenKind.ThrowErrorOperator, pos, buffer.CaretPosition));

			while (buffer.CaretPosition < buffer.Length)
			{

				if (buffer[buffer.CaretPosition] == '"' || (buffer[buffer.CaretPosition] == ' ' && buffer[buffer.CaretPosition + 1] == '"'))
				{

					if (buffer[buffer.CaretPosition] != '"')
						_ = buffer.Read();

					_ = buffer.Read();

					var retValRange = ReadQuotedString(buffer);

					line.Add(new(TokenKind.LogicPathValue, retValRange));
					line.Add(new(TokenKind.Eol, ^1, 0));

					break;

				}

				var token = TokenizeLogicPathComponent(buffer);

				if (token is not null)
					line.Add((SyntaxToken)token);

			}

			return line;

		}

		/// <inheritdoc />
		public static SyntaxToken? TokenizeLogicPathComponent(DualTextBuffer buffer)
		{

			buffer.SkipWhitespace();
			int startIndex = buffer.CaretPosition;
			var chars = buffer.ReadUntilWhitespace(false);
			int curPos = buffer.CaretPosition;

			if (chars.IsEquals("any"))
				return new(TokenKind.WildcardKeyword, startIndex, curPos);

			if (chars.IsEquals("defined"))
				return new(TokenKind.DefinedKeyword, startIndex, curPos);

			if (chars.IsAsciiEqualsIgnoreCase_10(
					"windows", "osx", "linux", "freebsd", "debian",
					"ubuntu", "archlinux", "fedora"
				))
				return new(TokenKind.SystemName, startIndex, curPos);

			if (chars.IsAsciiEqualsIgnoreCase_5("x64", "x86", "arm32", "arm64", "loongarch64"))
				return new(TokenKind.ArchitectureIdentifier, startIndex, curPos);

			if (Int32.TryParse(chars.Span, out _))
				return new(TokenKind.MajorVersionId, startIndex, curPos);

			if (chars.IsEquals_8(
					"==", "!=", "<", ">", "<=",
					">="
				))
			{

				if (chars.IsEquals("=="))
					return new(TokenKind.EqualTo, startIndex, curPos);

				if (chars.IsEquals("!="))
					return new(TokenKind.NotEqualTo, startIndex, curPos);

				if (chars.IsEquals(">"))
					return new(TokenKind.GreaterThan, startIndex, curPos);

				if (chars.IsEquals("<"))
					return new(TokenKind.LessThan, startIndex, curPos);

				if (chars.IsEquals(">="))
					return new(TokenKind.GreaterThanOrEqualTo, startIndex, curPos);

				if (chars.IsEquals("<="))
					return new(TokenKind.LessThanOrEqualTo, startIndex, curPos);

			}

			if (!chars.Span.TrimStart().StartsWith(QUOTE))
				return SyntaxToken.Empty;

			return null;

		}

		#region Helpers

		private static Range ReadQuotedString(DualTextBuffer buffer)
		{

			int start = buffer.CaretPosition;
			int finalQuoteIndex = buffer.Text.Span[buffer.CaretPosition..].LastIndexOf('"');

			if (finalQuoteIndex == -1)
				return new(^1, 0);

			finalQuoteIndex += buffer.CaretPosition; // absolute count

			while (buffer.CaretPosition < buffer.Length)
			{

				if (buffer.CaretPosition == finalQuoteIndex)
					break;

				_ = buffer.Read();

			}

			return new(start, buffer.CaretPosition); // ignores last double quote

		}

		#endregion

	}

}
