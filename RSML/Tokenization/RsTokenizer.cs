using System;

using RSML.Exceptions;


namespace RSML.Tokenization
{

	/// <summary>
	/// Standard RSML tokenizer.
	/// </summary>
	public class RsTokenizer : ITokenizer
	{

		// The goal is to minimize heap allocations, motherfucker.
		// Hence, the initialization of an array here instead of in every loop.
		private readonly RsToken[] eolToken = [ new(RsTokenType.Eol, Environment.NewLine) ];
		private readonly RsToken atToken = new(RsTokenType.SpecialActionHandler, '@');
		private readonly RsToken hashToken = new(RsTokenType.CommentSymbol, '#');

		/// <inheritdoc />
		public RsToken[] TokenizeLine(ReadOnlySpan<char> line, int lineNum)
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
								lineNum > 0
									? $"Malformed special action in line {lineNum}. A special action must have a name."
									: "Malformed special action. A special action must have a name."
							),

						2 when line[1] == '@' => // malformed ass (@ is not an allowed name)
							throw new InvalidRsmlSyntax(
								lineNum > 0
									? $"Malformed special action in line {lineNum}. A special action must not be named '@'."
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

			// ok so now we check for operators
			// easy-peasy motherfucker
			// first let's see the return operator cuz it's more important and doesn't suck ass
			if (line.StartsWith("-> ") || line.StartsWith("=> ") || line.StartsWith("return ") || line.StartsWith("returnif "))
			{

				// todo: tokenize logic path (return operator)
				throw new NotImplementedException("Logic paths not implemented yet."); // cuz im a dumbass

			}

			// error-throw op
			if (line.StartsWith("!> ") || line.StartsWith("error ") || line.StartsWith("errorif "))
			{

				// todo: tokenize logic path (error-throw operator)
				throw new NotImplementedException("Logic paths not implemented yet."); // cuz im a dumbass

			}

			throw lineNum switch
			{

				// anything else is a fucking error
				// cuz fuck implicit comments
				> 0 => new InvalidRsmlSyntax(
					$"Malformed RSML at line {lineNum}. A line must either be whitespace, special action, logic path or comment."
				),

				_ => new("Malformed RSML line. A line must either be whitespace, special action, logic path or comment.")
				// new what? oh, it's a C# feature - implicit exception from previous switch case
				// or something I don't know
				// ...
				// yeah I only found out today thanks to Rider
				// the things we learn lol

			};

		}

	}

}
