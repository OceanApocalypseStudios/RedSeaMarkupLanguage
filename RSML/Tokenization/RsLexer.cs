using System;

using RSML.Exceptions;
using RSML.Language;


namespace RSML.Tokenization
{

	/// <summary>
	/// Standard RSML tokenizer.
	/// </summary>
	public sealed class RsLexer : ILexer
	{

		/// <summary>
		/// The API version this tokenizer is built for.
		/// </summary>
		public const string ApiVersion = "2.0.0";

		/// <summary>
		/// The current line number.
		/// </summary>
		public int LineNumber { get; private set; }

		// The goal is to minimize allocations, motherfucker.
		// Hence, the initialization of an array here instead of in every loop.
		private static readonly RsToken[] eolToken = [ new(RsTokenType.Eol, Environment.NewLine) ];
		private static readonly RsToken atToken = new(RsTokenType.SpecialActionHandler, '@');
		private static readonly RsToken hashToken = new(RsTokenType.CommentSymbol, '#');
		private static readonly RsToken errorToken = new(RsTokenType.ThrowErrorOperator, "!>");
		private static readonly RsToken returnToken = new(RsTokenType.ReturnOperator, "->");
		private static readonly RsToken definedToken = new(RsTokenType.DefinedKeyword, "defined");
		private static readonly RsToken[] anyToken = [ new(RsTokenType.WildcardKeyword, "any") ];

		private static readonly string[] validComparators = [ "!=", "==", ">=", "<=", ">", "<" ]; // ! in order of priorities - very important

		/// <summary>
		/// Initializes a tokenizer at a given line number
		/// or line number 1.
		/// </summary>
		/// <param name="lineNum">The custom line number to start at or 1 if untouched</param>
		public RsLexer(int lineNum = 1) { LineNumber = lineNum - 1; }

		#region Helpers

		private bool TokenizeReturnValue(ReadOnlySpan<char> valueSpan, out RsToken? token)
		{

			if (valueSpan[0] != '"') // null token
			{

				token = null;

				return false;

			}

			if (valueSpan.Length < 3) // exception here
			{

				InvalidRsmlSyntax.Throw(
					LineNumber,
					$"Malformed logic path at line {LineNumber}. The return value must be enclosed in double quotes and have at least 1 character.",
					"Malformed logic path. The return value must be enclosed in double quotes and be at least 1 character long."
				);

			}

			var lastQuoteIdx = valueSpan.LastIndexOf('"');

			if (lastQuoteIdx <= 0)
			{

				InvalidRsmlSyntax.Throw(
					LineNumber, $"Malformed logic path at line {LineNumber}. The return value must be enclosed in double quotes.",
					"Malformed logic path. The return value must be enclosed in double quotes."
				);

			}

			token = new(RsTokenType.Value, valueSpan[1..lastQuoteIdx]);

			return true;

		}

		private int ValidateBasicLogicPathStructure(ReadOnlySpan<char> line)
		{

			// a logic path always contains at least 1 space
			if (!line.Contains(' '))
				InvalidRsmlSyntax.Throw(LineNumber, $"Malformed logic path at line {LineNumber}.", "Malformed logic path.");

			// same with the double quote, must appear at least twice
			var firstQuoteIdx = line.IndexOf('"');

			// an operator is 2 characters long,
			// but it must always be succeeded by a space
			// meaning the quote can't be in indexes 0, 1 or 2
			if (firstQuoteIdx < 3)
			{

				InvalidRsmlSyntax.Throw(
					LineNumber,
					$"Malformed logic path at line {LineNumber}. A logic path's value must be enclosed in double quotes.",
					"Malformed logic path. A logic path's value must be enclosed in double quotes."
				);

			}

			return firstQuoteIdx;

		}

		private void ValidateNumberOfEntries(int partsCount)
		{

			switch (partsCount)
			{

				// this means there's only 1 entry - the return value
				case < 1:
					InvalidRsmlSyntax.Throw(
						LineNumber, $"Malformed logic path at line {LineNumber}. A valid logic path must contain at least 2 valid arguments.",
						"Malformed logic path. A valid logic path must contain at least 2 valid arguments."
					);

					break;

				// this means there's more than 5 entries
				case > 4:
					InvalidRsmlSyntax.Throw(
						LineNumber, $"Malformed logic path at line {LineNumber}. A valid logic path must contain less than 5 arguments.",
						"Malformed logic path. A valid logic path must contain less than 5 arguments."
					);

					break;

			}

		}

		private RsToken TokenizeSystemName(Span<char> systemName)
		{

			if (systemName.IsEquals("any"))
				return anyToken[0];

			if (systemName.IsEquals("defined"))
			{

				InvalidRsmlSyntax.Throw(
					LineNumber, $"Malformed logic path at line {LineNumber}. Invalid use of 'defined'.",
					"Malformed logic path. Invalid use of 'defined'."
				);

			}

			if (!systemName.IsEquals(
					StringComparison.Ordinal, "windows", "osx", "linux", "freebsd",
					"debian", "arch", "alpine", "gentoo"
				))
			{

				InvalidRsmlSyntax.Throw(
					LineNumber, $"Malformed logic path at line {LineNumber}. System name is not allowed.",
					"Malformed logic path. System name is not allowed."
				);

			}

			return new(RsTokenType.SystemName, systemName);

		}

		private RsToken TokenizeArchitectureIdentifier(Span<char> archId)
		{

			if (archId.IsEquals("any"))
				return anyToken[0];

			if (archId.IsEquals("defined"))
			{

				InvalidRsmlSyntax.Throw(
					LineNumber, $"Malformed logic path at line {LineNumber}. Invalid use of 'defined'.",
					"Malformed logic path. Invalid use of 'defined'."
				);

			}

			if (!archId.IsEquals(StringComparison.Ordinal, "x64", "arm64", "arm32", "x86"))
			{

				InvalidRsmlSyntax.Throw(
					LineNumber, $"Malformed logic path at line {LineNumber}. Architecture ID is not allowed.",
					"Malformed logic path. Architecture ID is not allowed."
				);

			}

			return new(RsTokenType.ArchitectureIdentifier, archId);

		}

		private static RsTokenType GetComparatorTokenType(string comp) =>
			comp switch
			{

				"==" => RsTokenType.Equals,
				">=" => RsTokenType.GreaterOrEqualsThan,
				"<=" => RsTokenType.LessOrEqualsThan,
				"!=" => RsTokenType.Different,
				">"  => RsTokenType.GreaterThan,
				"<"  => RsTokenType.LessThan,
				_    => throw new ArgumentException("Invalid comparator.", nameof(comp))

			};

		private RsToken[] ParseMajorVersion(ReadOnlySpan<char> span)
		{

			if (span.IsEquals("any"))
				return anyToken;

			if (span.IsEquals("defined"))
				return [ definedToken ];

			string comparator = "=="; // assume equals by default
			int compLen = 0;

			foreach (var cmp in validComparators)
			{

				if (!span.StartsWith(cmp.AsSpan(), StringComparison.Ordinal))
					continue;

				comparator = cmp;
				compLen = cmp.Length;

				break;

			}

			// the actual number
			var numberPart = span[compLen..];

			if (!Int32.TryParse(numberPart, out int verNum))
			{

				InvalidRsmlSyntax.Throw(
					LineNumber,
					$"Malformed logic path at line {LineNumber}. Major version must be a valid integer, 'any', 'defined' or an integer with a valid comparison.",
					"Malformed logic path. Invalid major version."
				);

			}

			return [ new(GetComparatorTokenType(comparator), comparator), new(RsTokenType.MajorVersionId, verNum.ToString()) ];

		}

		#endregion

		private RsToken[] TokenizeLogicPath(ReadOnlySpan<char> line, bool errorIf)
		{

			// first let's validate the basic structure of the line
			var firstQuoteIdx = ValidateBasicLogicPathStructure(line);

			// now for the actual tokenization!!!!!
			// we split by the space character
			// BUT NOT THE WHOLE STRING/SPAN!!
			// only up to the first quote
			var preQuoteSpan = line[..firstQuoteIdx];
			Span<Range> ranges = stackalloc Range[preQuoteSpan.Count(' ')];
			int partsCount = preQuoteSpan.Split(ranges, ' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

			// so we now should have either 1 entry
			// 2 entries
			// 3 entries
			// or 4 entries
			// BUT NEVER 5
			ValidateNumberOfEntries(partsCount);

			var systemNameSpan = partsCount > 1 ? preQuoteSpan[ranges[1]] : [ ];

			var architectureIdSpan = partsCount > 2
										 ? partsCount > 3
											   ? preQuoteSpan[ranges[3]]
											   : preQuoteSpan[ranges[2]]
										 : [ ];

			var versionMajorSpan = partsCount > 3 ? preQuoteSpan[ranges[2]] : [ ];

			Span<char> systemNameSpanLower = stackalloc char[systemNameSpan.Length];
			Span<char> architectureIdSpanLower = stackalloc char[architectureIdSpan.Length];
			Span<char> versionMajorSpanLower = stackalloc char[versionMajorSpan.Length];

			systemNameSpan.ToLowerInvariant(systemNameSpanLower);
			architectureIdSpan.ToLowerInvariant(architectureIdSpanLower);
			versionMajorSpan.ToLowerInvariant(versionMajorSpanLower);

			var afterQuoteSpan = line[firstQuoteIdx..];

			/*
			 * Allowed logic path syntaxes
			 * --------------------------
			 * Minimum entries: 2
			 * Maximum entries: 5
			 * For now, maximum entries: 4
			 *
			 * However, we hide the return value for now.
			 *
			 *	   0		      1			       2			      3			       [4]
			 *
			 * <operator> [<return--value>]
			 * <operator>   <system-name>   [<return--value>]
			 * <operator>   <system-name>   <architecture-id>  [<return--value>]
			 * <operator>   <system-name>    <major-version>   <architecture-id> [<return--value>]
			 *
			 */

			if (TokenizeReturnValue(afterQuoteSpan, out var returnValueToken))
			{

				// operator + return value
				if (partsCount == 1)
					return [ errorIf ? errorToken : returnToken, anyToken[0], anyToken[0], anyToken[0], (RsToken)(returnValueToken!), eolToken[0] ];

				var systemToken = TokenizeSystemName(systemNameSpanLower);

				// operator + system name + return value
				if (partsCount == 2)
					return [ errorIf ? errorToken : returnToken, systemToken, anyToken[0], anyToken[0], (RsToken)(returnValueToken!), eolToken[0] ];

				// operator + system name + architecture + return value
				var archToken = TokenizeArchitectureIdentifier(architectureIdSpanLower);

				switch (partsCount)
				{

					case 3:
						return [ errorIf ? errorToken : returnToken, systemToken, anyToken[0], archToken, (RsToken)(returnValueToken!), eolToken[0] ];

					// operator + system name + major version + architecture + return value
					case 4:
						var majorVersionTokens = ParseMajorVersion(versionMajorSpanLower);

						return
						[
							errorIf ? errorToken : returnToken, systemToken, majorVersionTokens[0], majorVersionTokens[1], archToken,
							(RsToken)(returnValueToken!), eolToken[0]
						];

					default:
						InvalidRsmlSyntax.Throw(
							LineNumber,
							$"Malformed logic path at line {LineNumber}. Unexpected number of arguments.",
							"Malformed logic path. All overloads failed."
						);

						break;

				}

			}

			InvalidRsmlSyntax.Throw(
				LineNumber,
				$"Malformed logic path at line {LineNumber}. All overloads failed.",
				"Malformed logic path. All overloads failed."
			);

			return null!; // won't happen, but it pleases .NET in ways nobody can understand

		}

		private RsToken[] TokenizeSpecialAction(ReadOnlySpan<char> line, int nextNewline)
		{

			// ok so now we have to find the name
			// now unlike before, v2.0.0 is much, MUCH
			// stricter
			// so like no more bullshit like this:
			// @@ "This is allowed"
			// well NOT ANYMORE DUMBASS
			var nextSpace = line.IndexOf(' ');

			// depending on where the next space is
			// or if it EVEN FUCKING EXISTS
			// we decide the NextStepToTakeTM
			// shit
			return nextSpace switch
			{

				1 => // malformed bullshit (space right after @)
					throw new InvalidRsmlSyntax(
						LineNumber > 0
							? $"Malformed special action in line {LineNumber}. A special action must have a name."
							: "Malformed special action. A special action must have a name."
					),

				2 when line[1] == '@' => // malformed ass (@ is not an allowed name)
					throw new InvalidRsmlSyntax(
						LineNumber > 0
							? $"Malformed special action in line {LineNumber}. A special action must not be named '@'."
							: "Malformed special action. A special action must not be named '@'."
					),

				< 0 => // no argument (no space)
				[
					atToken, new(RsTokenType.SpecialActionName, line[1..nextNewline]), new(RsTokenType.SpecialActionArgument, ""), eolToken[0]
				],

				_ => // literally anything fucking else
				[
					atToken, new(RsTokenType.SpecialActionName, line[1..nextSpace]),
					new(RsTokenType.SpecialActionArgument, line[(nextSpace + 1)..nextNewline]), eolToken[0]
				]

			};

		}

		/// <inheritdoc />
		public RsToken[] TokenizeLine(ReadOnlySpan<char> line)
		{

			++LineNumber;

			// Ok imma comment the shit out of this
			// just so you know what the process is
			// or something stupid like that k?
			if (line.IsEmpty)
				return eolToken;

			// this isn't supposed to happen
			// but some dumbass user probably going to attempt this
			// so just to be safe
			var nextNewline = line.IndexOf('\n');

			// carriage returns are a thing tho
			if (nextNewline > 0)
			{

				if (line[nextNewline - 1] == '\r')
					nextNewline--;

			}

			// we trim to get rid of any whitespace that might exist
			line = nextNewline >= 0 ? line.Trim() : line[..nextNewline].Trim();

			// if it's empty or just whitespace, it's an EOL token
			if (line.IsEmpty || line.IsWhiteSpace())
				return eolToken;

			// technically you should only pass LINES here and not several ones
			// but ofc someone's going to try to break this piece of shit
			if (line.IsNewLinesOnly())
			{

				RsToken[] result = new RsToken[line.Length];
				result.AsSpan().Fill(eolToken[0]);

				return result;

			}

			// the first character of the line
			// tells us a shitton of things
			switch (line[0])
			{

				// if it's a comment, it's a FUCKING comment!
				// don't FUCKING bother with doing extra checks
				// we're supposed to be fast, remember?
				case '#':
					return [ hashToken, new(RsTokenType.CommentText, line[1..]), eolToken[0] ];

				// special actions are cool until you do
				// Thread.Sleep(1000); like a FUCKING dumbass
				// special actions are not supposed to be used like that, y'know
				// ugh
				case '@':
					return TokenizeSpecialAction(line, nextNewline);

			}

			// ok so now we check for operators
			// easy-peasy motherfucker
			// first let's see the return operator cuz it's more important and doesn't suck ass
			// ReSharper disable StringLiteralTypo
			if (line.StartsWith("-> "))

				// ReSharper restore StringLiteralTypo
				return TokenizeLogicPath(line, false);

			// error-throw op
			// ReSharper disable StringLiteralTypo
			if (line.StartsWith("!> "))

				// ReSharper restore StringLiteralTypo
				return TokenizeLogicPath(line, true);

			InvalidRsmlSyntax.Throw(
				LineNumber,
				$"Malformed RSML at line {LineNumber}. A line must either be whitespace, special action, logic path or comment.",
				"Malformed RSML line. A line must either be whitespace, special action, logic path or comment."
			);

			return null!; // won't run

		}

	}

}
