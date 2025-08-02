using System;
using System.Buffers;
using RSML.Language;

namespace RSML.Tokenization
{

	/// <summary>
	/// Standard RSML tokenizer.
	/// </summary>
	public readonly struct RSTokenizer : ITokenizer
	{

		private static RSToken EmptyToken => new(RSTokenType.CommentText, "");
		private static RSToken HashToken => new(RSTokenType.CommentStart, "#");
		private static RSToken AtToken => new(RSTokenType.SpecialActionHandler, "@");

		private static ReadOnlySpan<RSToken> TokenizeLogicPath(OperatorType @operator, ReadOnlySpan<char> logicPath, in LanguageStandard languageStandard)
		{

			var opStr = @operator == OperatorType.Primary
							? languageStandard.PrimaryOperatorSymbol
							: @operator == OperatorType.Secondary
								? languageStandard.SecondaryOperatorSymbol
								: languageStandard.TertiaryOperatorSymbol;
			var opIdx = logicPath.IndexOf(opStr);

			var left = logicPath[..opIdx].TrimEnd();
			var right = logicPath[(opIdx + opStr.Length)..].TrimStart();

			if (left.Length < 1)
			{

				var singleBuffer = ArrayPool<RSToken>.Shared.Rent(1);

				if (singleBuffer[0] != EmptyToken)
					singleBuffer[0] = EmptyToken;

				ArrayPool<RSToken>.Shared.Return(singleBuffer);
				return singleBuffer.AsSpan(0, 1);

			}

			if (right.Length < 1)
			{

				var singleBuffer = ArrayPool<RSToken>.Shared.Rent(1);

				if (singleBuffer[0] != EmptyToken)
					singleBuffer[0] = EmptyToken;

				ArrayPool<RSToken>.Shared.Return(singleBuffer);
				return singleBuffer.AsSpan(0, 1);

			}

			var buffer = ArrayPool<RSToken>.Shared.Rent(3);

			RSToken tokenLeft = new(RSTokenType.LogicPathLeft, left);
			RSToken tokenOp = new(
				@operator == OperatorType.Primary
					? RSTokenType.PrimaryOperator
					: @operator == OperatorType.Secondary
						? RSTokenType.SecondaryOperator
						: RSTokenType.TertiaryOperator,
				opStr
			);
			RSToken tokenRight = new(RSTokenType.LogicPathRight, right);

			if (buffer[0] != tokenLeft)
				buffer[0] = tokenLeft;

			if (buffer[1] != tokenOp)
				buffer[1] = tokenOp;

			if (buffer[2] != tokenRight)
				buffer[2] = tokenRight;

			ArrayPool<RSToken>.Shared.Return(buffer);
			return buffer.AsSpan(0, 3);

		}

		/// <inheritdoc />
		public ReadOnlySpan<RSToken> TokenizeLine(ReadOnlySpan<char> line, in LanguageStandard languageStandard)
		{

			line = line.Trim();

			if (line.Length < 1)
			{

				var singleBuffer = ArrayPool<RSToken>.Shared.Rent(1);

				if (singleBuffer[0] != EmptyToken)
					singleBuffer[0] = EmptyToken;

				ArrayPool<RSToken>.Shared.Return(singleBuffer);
				return singleBuffer.AsSpan(0, 1);

			}

			if (line[0] == '#')
			{

				var doubleBuffer = ArrayPool<RSToken>.Shared.Rent(2);
				RSToken textToken = new(RSTokenType.CommentText, line[1..]);

				if (doubleBuffer[0] != HashToken)
					doubleBuffer[0] = HashToken;

				if (doubleBuffer[1] != textToken)
					doubleBuffer[1] = textToken;

				ArrayPool<RSToken>.Shared.Return(doubleBuffer);
				return doubleBuffer.AsSpan(0, 2);

			}

			if (line[0] == '@' && line.Length > 1)
			{

				var firstSpace = line.IndexOf(' ');

				if (firstSpace == -1)
				{

					var doubleBuffer = ArrayPool<RSToken>.Shared.Rent(2);
					RSToken actionNameTkn = new(RSTokenType.SpecialActionName, line[1..].TrimEnd());

					if (doubleBuffer[0] != AtToken)
						doubleBuffer[0] = AtToken;

					if (doubleBuffer[1] != actionNameTkn)
						doubleBuffer[1] = actionNameTkn;

					ArrayPool<RSToken>.Shared.Return(doubleBuffer);
					return doubleBuffer.AsSpan(0, 2);

				}

				var buffer = ArrayPool<RSToken>.Shared.Rent(3);
				RSToken actionNameToken = new(RSTokenType.SpecialActionName, line[1..].TrimEnd());
				RSToken actionArgToken = new(RSTokenType.SpecialActionArgument, line[(firstSpace + 1)..].TrimEnd());

				if (buffer[0] != AtToken)
					buffer[0] = AtToken;

				if (buffer[1] != actionNameToken)
					buffer[1] = actionNameToken;

				if (buffer[2] != actionArgToken)
					buffer[2] = actionArgToken;

				ArrayPool<RSToken>.Shared.Return(buffer);
				return buffer.AsSpan(0, 3);

			}

			if (line.IndexOf(languageStandard.PrimaryOperatorSymbol) >= 0)
				return TokenizeLogicPath(OperatorType.Primary, line, languageStandard);

			if (line.IndexOf(languageStandard.SecondaryOperatorSymbol) >= 0)
				return TokenizeLogicPath(OperatorType.Secondary, line, languageStandard);

			if (line.IndexOf(languageStandard.TertiaryOperatorSymbol) >= 0)
				return TokenizeLogicPath(OperatorType.Tertiary, line, languageStandard);

			var comBuffer = ArrayPool<RSToken>.Shared.Rent(1);
			RSToken commentTkn = new(RSTokenType.CommentText, line);

			if (comBuffer[0] != commentTkn)
				comBuffer[0] = commentTkn;

			ArrayPool<RSToken>.Shared.Return(comBuffer);
			return comBuffer.AsSpan(0, 1);

		}

	}

}
