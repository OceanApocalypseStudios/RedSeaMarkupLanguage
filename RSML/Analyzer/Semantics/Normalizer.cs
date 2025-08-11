using System.Collections.Generic;
using System.Linq;

using RSML.Analyzer.Syntax;
using RSML.Toolchain;


namespace RSML.Analyzer.Semantics
{

	/// <summary>
	/// The official RSML normalizer.
	/// </summary>
	public sealed class Normalizer : INormalizer
	{

		private static readonly SyntaxToken wildcard = TokenBank.wildcard;
		private static readonly SyntaxToken eol = TokenBank.eolToken;

		/// <inheritdoc />
		public string StandardizedVersion => "2.0.0";

		/// <summary>
		/// Creates a new Normalizer instance.
		/// </summary>
		public Normalizer() { }

		/// <inheritdoc />
		public IEnumerable<SyntaxToken> NormalizeLine(IEnumerable<SyntaxToken> tokens, out int length)
		{

			var actualTokens = tokens.ToArray();

			if (actualTokens.Length == 0)
			{

				length = 0;

				return [ ];

			}

			// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
			switch (actualTokens[0].Kind)
			{

				case TokenKind.Eol when actualTokens.Length == 1:
				case TokenKind.Eof when actualTokens.Length == 1:
					length = 1;
					return actualTokens;

				case TokenKind.CommentSymbol:
					return actualTokens.Length switch
					{
						2 => ReturnHelper([ actualTokens[0], new(TokenKind.CommentText, ""), eol ], out length),
						3 => ReturnHelper(actualTokens, out length),
						_ => ReturnHelper([ ], out length)
					};

				case TokenKind.SpecialActionSymbol:
					return actualTokens.Length switch
					{
						3 => ReturnHelper([ actualTokens[0], actualTokens[1], new(TokenKind.SpecialActionArgument, ""), eol ], out length),
						4 => ReturnHelper(actualTokens, out length),
						_ => ReturnHelper([ ], out length)
					};

				case TokenKind.ReturnOperator:
				case TokenKind.ThrowErrorOperator:
					return actualTokens.Length switch
					{
						// eol matters
						3 => ReturnHelper([ actualTokens[0], wildcard, wildcard, wildcard, actualTokens[1], eol ], out length),
						4 => ReturnHelper([ actualTokens[0], actualTokens[1], wildcard, wildcard, actualTokens[2], eol ], out length),
						5 => ReturnHelper([ actualTokens[0], actualTokens[1], wildcard, actualTokens[2], actualTokens[3], eol ], out length),
						6 => ReturnHelper([ actualTokens[0], actualTokens[1], actualTokens[2], actualTokens[3], actualTokens[4], eol ], out length),
						7 => ReturnHelper(
							[
								actualTokens[0], actualTokens[1], actualTokens[2], actualTokens[3], actualTokens[4], actualTokens[5],
								eol
							], out length
						),
						_ => ReturnHelper([ ], out length)
					};

				default:
					length = 0;

					return [ ];

			}

		}

		private static SyntaxToken[] ReturnHelper(SyntaxToken[] tokens, out int len)
		{

			len = tokens.Length;

			return tokens;

		}

	}

}
