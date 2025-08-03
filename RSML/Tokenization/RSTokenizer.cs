using System;
using System.Buffers;

using RSML.Language;


namespace RSML.Tokenization
{

	/// <summary>
	/// Standard RSML tokenizer.
	/// </summary>
	public readonly struct RsTokenizer : ITokenizer
	{

		private static readonly RsToken[] emptyToken = [new(RsTokenType.CommentText, "")];

		private static readonly RsToken hashToken = new(RsTokenType.CommentStart, "#");

		private static readonly RsToken atToken = new(RsTokenType.SpecialActionHandler, "@");

		private static ReadOnlySpan<RsToken> TokenizeLogicPath(
			OperatorType @operator,
			ReadOnlySpan<char> logicPath,
			in LanguageStandard languageStandard
		)
		{

			var opStr = @operator switch
			{

				OperatorType.Primary   => languageStandard.PrimaryOperatorSymbol,
				OperatorType.Secondary => languageStandard.SecondaryOperatorSymbol,
				_                      => languageStandard.TertiaryOperatorSymbol

			};

			int opIdx = logicPath.IndexOf(opStr);

			var left = logicPath[..opIdx].TrimEnd();
			var right = logicPath[(opIdx + opStr.Length)..].TrimStart();

			if (left.Length < 1 || right.Length < 1)
				return new(emptyToken);

			RsToken tokenLeft = new(RsTokenType.LogicPathLeft, left);

			RsToken tokenOp = new(
				@operator switch
				{

					OperatorType.Primary   => RsTokenType.PrimaryOperator,
					OperatorType.Secondary => RsTokenType.SecondaryOperator,
					_                      => RsTokenType.TertiaryOperator

				},
				opStr
			);

			RsToken tokenRight = new(RsTokenType.LogicPathRight, right);

			var buffer = ArrayPool<RsToken>.Shared.Rent(3);
			buffer[0] = tokenLeft;
			buffer[1] = tokenOp;
			buffer[2] = tokenRight;
			ArrayPool<RsToken>.Shared.Return(buffer);

			return buffer;

		}

		/// <inheritdoc />
		public ReadOnlySpan<RsToken> TokenizeLine(ReadOnlySpan<char> line, in LanguageStandard languageStandard)
		{

			line = line.Trim();

			if (line.Length < 1)
				return new(emptyToken);

			switch (line[0])
			{

				case '#':
				{

					var doubleBuffer = ArrayPool<RsToken>.Shared.Rent(2);
					RsToken textToken = new(RsTokenType.CommentText, line[1..]);

					doubleBuffer[0] = hashToken;
					doubleBuffer[1] = textToken;

					ArrayPool<RsToken>.Shared.Return(doubleBuffer);

					return doubleBuffer.AsSpan(0, 2);

				}

				case '@' when line.Length > 1:
				{

					int firstSpace = line.IndexOf(' ');

					if (firstSpace == -1)
					{

						var doubleBuffer = ArrayPool<RsToken>.Shared.Rent(2);
						RsToken actionNameTkn = new(RsTokenType.SpecialActionName, line[1..].TrimEnd());

						doubleBuffer[0] = atToken;
						doubleBuffer[1] = actionNameTkn;
						ArrayPool<RsToken>.Shared.Return(doubleBuffer);

						return doubleBuffer.AsSpan(0, 2);

					}

					var buffer = ArrayPool<RsToken>.Shared.Rent(3);
					RsToken actionNameToken = new(RsTokenType.SpecialActionName, line[1..].TrimEnd());
					RsToken actionArgToken = new(RsTokenType.SpecialActionArgument, line[(firstSpace + 1)..].TrimEnd());

					buffer[0] = atToken;
					buffer[1] = actionNameToken;
					buffer[2] = actionArgToken;

					ArrayPool<RsToken>.Shared.Return(buffer);

					return buffer.AsSpan(0, 3);

				}

			}

			if (line.IndexOf(languageStandard.PrimaryOperatorSymbol) >= 0)
				return TokenizeLogicPath(OperatorType.Primary, line, languageStandard);

			if (line.IndexOf(languageStandard.SecondaryOperatorSymbol) >= 0)
				return TokenizeLogicPath(OperatorType.Secondary, line, languageStandard);

			if (line.IndexOf(languageStandard.TertiaryOperatorSymbol) >= 0)
				return TokenizeLogicPath(OperatorType.Tertiary, line, languageStandard);

			var comBuffer = ArrayPool<RsToken>.Shared.Rent(1);
			RsToken commentTkn = new(RsTokenType.CommentText, line);
			comBuffer[0] = commentTkn;
			ArrayPool<RsToken>.Shared.Return(comBuffer);

			return comBuffer.AsSpan(0, 1);

		}

	}

}
