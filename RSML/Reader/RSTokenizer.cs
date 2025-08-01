using System;

using RSML.Language;


namespace RSML.Reader
{

	/// <summary>
	/// Standard RSML tokenizer.
	/// </summary>
	public readonly struct RSTokenizer : ITokenizer
	{

		private static RSToken EmptyToken => new(RSTokenType.CommentText, "");

		private static ReadOnlySpan<RSToken> TokenizeLogicPath(OperatorType @operator, ReadOnlySpan<char> logicPath, in LanguageStandard languageStandard)
		{

			var opStr = @operator == OperatorType.Primary
							? languageStandard.PrimaryOperatorSymbol
							: (@operator == OperatorType.Secondary)
								? languageStandard.SecondaryOperatorSymbol
								: languageStandard.TertiaryOperatorSymbol;
			var opIdx = logicPath.IndexOf(opStr);

			ReadOnlySpan<char> left = logicPath[..opIdx].TrimEnd();
			ReadOnlySpan<char> right = logicPath[(opIdx + opStr.Length)..].TrimStart();

			if (left.Length < 1)
				return new([EmptyToken]);

			if (right.Length < 1)
				return new([EmptyToken]);

			return new([
				new(RSTokenType.LogicPathLeft, left.ToString()),
				new(
					@operator == OperatorType.Primary
						? RSTokenType.PrimaryOperator
						: (@operator == OperatorType.Secondary)
							? RSTokenType.SecondaryOperator
							: RSTokenType.TertiaryOperator,
					opStr.ToString()
				),
				new(RSTokenType.LogicPathRight, right.ToString())
			]);

		}

		/// <inheritdoc />
		public ReadOnlySpan<RSToken> TokenizeLine(ReadOnlySpan<char> line, in LanguageStandard languageStandard)
		{

			line = line.Trim();

			if (line.Length < 1)
				return new([EmptyToken]);

			if (line[0] == '#')
				return new([new(RSTokenType.CommentStart, "#"), new(RSTokenType.CommentText, line[1..].ToString())]);

			if (line[0] == '@' && line.Length > 1)
			{

				var firstSpace = line.IndexOf(' ');

				if (firstSpace == -1)
					return new([
						new(RSTokenType.SpecialActionHandler, "@"),
						new(RSTokenType.SpecialActionName, line[1..].TrimEnd().ToString())
					]);

				return new([
					new(RSTokenType.SpecialActionHandler, "@"),
					new(RSTokenType.SpecialActionName, line[1..firstSpace].ToString()),
					new(RSTokenType.SpecialActionArgument, line[(firstSpace + 1)..].TrimEnd().ToString()),
				]);

			}

			if (line.Contains(languageStandard.PrimaryOperatorSymbol, StringComparison.InvariantCulture))
				return TokenizeLogicPath(OperatorType.Primary, line, languageStandard);

			if (line.Contains(languageStandard.SecondaryOperatorSymbol, StringComparison.InvariantCulture))
				return TokenizeLogicPath(OperatorType.Secondary, line, languageStandard);

			if (line.Contains(languageStandard.TertiaryOperatorSymbol, StringComparison.InvariantCulture))
				return TokenizeLogicPath(OperatorType.Tertiary, line, languageStandard);

			return new([ new(RSTokenType.CommentText, line.ToString()) ]);

		}

	}

}
