using System;

using RSML.Exceptions;
using RSML.Language;


namespace RSML.Tokenization
{

	/// <summary>
	/// Standard RSML tokenizer.
	/// </summary>
	public sealed class RsTokenizer : ITokenizer
	{

		/// <summary>
		/// The current line number.
		/// </summary>
		public int LineNumber { get; set; }

		// The goal is to minimize allocations, motherfucker.
		// Hence, the initialization of an array here instead of in every loop.
		private static readonly RsToken[] eolToken = [ new(RsTokenType.Eol, Environment.NewLine) ];
		private static readonly RsToken atToken = new(RsTokenType.SpecialActionHandler, '@');
		private static readonly RsToken hashToken = new(RsTokenType.CommentSymbol, '#');
		private static readonly RsToken errorToken = new(RsTokenType.ThrowErrorOperator, "!>");
		private static readonly RsToken returnToken = new(RsTokenType.ReturnOperator, "->");
		private static readonly RsToken definedToken = new(RsTokenType.DefinedKeyword, "defined");
		private static readonly RsToken anyToken = new(RsTokenType.WildcardKeyword, "any");

		/// <summary>
		/// Initializes a tokenizer at a given line number
		/// or line number 1.
		/// </summary>
		/// <param name="lineNum">The custom line number to start at or 1 if untouched</param>
		public RsTokenizer(int lineNum = 1) { LineNumber = lineNum; }

		private bool TryTokenizeReturnValue(ReadOnlySpan<char> valueSpan, out RsToken? token)
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

			if (valueSpan[^1] != '"') // exception here
			{

				InvalidRsmlSyntax.Throw(
					LineNumber, $"Malformed logic path at line {LineNumber}. The return value must be enclosed in double quotes.",
					"Malformed logic path. The return value must be enclosed in double quotes."
				);

			}

			token = new(RsTokenType.Value, valueSpan[1..^1]);

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

		private RsToken[] TokenizeLogicPath(ReadOnlySpan<char> line, bool errorIf)
		{

			// first let's validate the basic structure of the line
			var firstQuoteIdx = ValidateBasicLogicPathStructure(line);

			// now for the actual tokenization!!!!!
			// we split by the space character
			// BUT NOT THE WHOLE STRING/SPAN!!
			// only up to the first quote
			var lineUpToQuote = line[..firstQuoteIdx];

			Span<Range> ranges = stackalloc Range[(line.Count(' '))];
			int entryCount = lineUpToQuote.Split(ranges, ' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

			// so we now should have either 1 entry
			// 2 entries
			// 3 entries
			// or 4 entries
			// BUT NEVER 5
			ValidateNumberOfEntries(entryCount);

			// todo: clean-up this method further
			// todo: apply the new way of splitting (keeping quotes)
			// todo: change the Windows, OSX, etc token types to just be RsTokenType.SystemName for scalability reasons
			// todo: same with architectures (already done is RsTokenType, just has to be cleaned up here and in other spots)

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

			// let's do 1 first
			// can either be return-value
			// or system-name
			// the check is simple
			if (TryTokenizeReturnValue(line[firstQuoteIdx..], out var token1))
				return [ errorIf ? errorToken : returnToken, anyToken, anyToken, anyToken, (RsToken)(token1!), eolToken[0] ];

			// ok so that means the first item must be a system name
			// but then again some dumbass gonna write some shitty
			// RSML that doesn't work
			// cuz fuck people
			if (!line[ranges[1]]
					.IsEquals(
						StringComparison.OrdinalIgnoreCase, "windows", "osx", "linux", "freebsd",
						"debian", "arch", "alpine", "gentoo"
					))
			{

				if (LineNumber > 0)
				{

					throw new InvalidRsmlSyntax(
						$"Malformed logic path at line {LineNumber}. The second entry of a logic path must either be a return value or an allowed system name."
					);

				}

				throw new InvalidRsmlSyntax(
					"Malformed logic path. The second entry of a logic path must either be a return value or an allowed system name."
				);

			}

			// ok this is correct
			Span<char> sysSpan = stackalloc char[line[ranges[1]].Length];
			line[ranges[1]].ToLowerInvariant(sysSpan);

			// we get the system from the span
			var tokenType = sysSpan.GetSystemTokenType();

			// ok now let's move on to the next fields
			// if it's another return value, we end here ofc
			if (TryTokenizeReturnValue(line[ranges[2]], out var token2))
				return [ errorIf ? errorToken : returnToken, new((RsTokenType)(tokenType!), sysSpan), (RsToken)(token2!) ];

			// but it probably isn't so let's continue
			// ok so now we have uh
			// fuck
			// it's either an architecture or a major version
			// and this depends on how many entries we got
			if (splitCount == 5)
			{

				// then this ugh is
				// the major version ugh
				if (!line[ranges[3]].IsEquals(StringComparison.Ordinal, "x64", "x86", "arm64", "arm32"))
				{

					if (LineNumber > 0)
					{

						throw new InvalidRsmlSyntax(
							$"Malformed logic path at line {LineNumber}. The third entry of a 4-entry long logic path must be an allowed architecture ID."
						);

					}

					throw new InvalidRsmlSyntax(
						"Malformed logic path. The third entry of a 4-entry long logic path must be an allowed architecture ID."
					);

				}

				RsTokenType archTokenType1 = (RsTokenType)(line[ranges[3]].GetArchTokenType()!); // we have to do this cuz Rider is fucking dumb

				// ok so now we know
				// that it's a valid architecture
				// architectures ARE case-sensitive so
				// we don't need to worry

				// ok so now, let's see that motherfucking version number
				if (line[ranges[2]].IsDigitOnly()) // it's a digit
				{

					if (TryTokenizeReturnValue(line[ranges[4]], out var token3))
					{

						return
						[
							errorIf ? errorToken : returnToken, new((RsTokenType)(tokenType!), sysSpan), new(RsTokenType.Equals, "=="),
							new(RsTokenType.MajorVersionId, line[ranges[2]]), new(archTokenType1, line[ranges[3]]), (RsToken)token3!
						];

					}

					if (LineNumber > 0)
						throw new InvalidRsmlSyntax(
							$"Malformed logic path at line {LineNumber}. All overloads for the operators have been exhausted."
						);

					throw new InvalidRsmlSyntax("Malformed logic path. All overloads for the operators have been exhausted.");

				}

				// todo: parse version number when it has conditional operators

			}

			// alright, so no version number which is good because version number is the hardest part to parse
			if (splitCount == 4)
			{

				// then this ugh is
				// the major version ugh
				if (!line[ranges[3]].IsEquals(StringComparison.Ordinal, "x64", "x86", "arm64", "arm32"))
				{

					if (LineNumber > 0)
					{

						throw new InvalidRsmlSyntax(
							$"Malformed logic path at line {LineNumber}. The third entry of a 4-entry long logic path must be an allowed architecture ID."
						);

					}

					throw new InvalidRsmlSyntax(
						"Malformed logic path. The third entry of a 4-entry long logic path must be an allowed architecture ID."
					);

				}

				RsTokenType archTokenType1 = (RsTokenType)(line[ranges[3]].GetArchTokenType()!); // we have to do this cuz Rider is fucking dumb

				// ok so now we know
				// that it's a valid architecture
				// architectures ARE case-sensitive so
				// we don't need to worry
				if (TryTokenizeReturnValue(line[ranges[4]], out var token3))
				{

					return
					[
						errorIf ? errorToken : returnToken, new((RsTokenType)(tokenType!), sysSpan), new(RsTokenType.Equals, "=="),
						new(RsTokenType.MajorVersionId, line[ranges[2]]), new(archTokenType1, line[ranges[3]]), (RsToken)token3!
					];

				}

			}

			if (LineNumber > 0)
				throw new InvalidRsmlSyntax($"Malformed logic path at line {LineNumber}. All overloads for the operators have been exhausted.");

			throw new InvalidRsmlSyntax("Malformed logic path. All overloads for the operators have been exhausted.");

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
					atToken, new(RsTokenType.SpecialActionName, line[1..nextNewline]), eolToken[0]
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
			if (line.StartsWith("-> ") || line.StartsWith("=> ") || line.StartsWith("return ") || line.StartsWith("returnif "))

				// ReSharper restore StringLiteralTypo
				return TokenizeLogicPath(line, false);

			// error-throw op
			// ReSharper disable StringLiteralTypo
			if (line.StartsWith("!> ") || line.StartsWith("error ") || line.StartsWith("errorif "))

				// ReSharper restore StringLiteralTypo
				return TokenizeLogicPath(line, true);

			throw LineNumber switch
			{

				// anything else is a fucking error
				// cuz fuck implicit comments
				> 0 => new InvalidRsmlSyntax(
					$"Malformed RSML at line {LineNumber}. A line must either be whitespace, special action, logic path or comment."
				),

				_ => new("Malformed RSML line. A line must either be whitespace, special action, logic path or comment.")

				// Uh, new what? Oh, it's a C# feature - implicit exception from previous switch case
				// or something I don't know
				// ...
				// yeah I only found out today thanks to Rider
				// the things we learn lol

			};

		}

	}

}
