using System.Collections.Generic;

using RSML.Analyzer.Syntax;


namespace RSML.Middlewares
{

	/// <summary>
	/// A middleware that is given some tokens and returns a <see cref="MiddlewareResult" />.
	/// </summary>
	public delegate MiddlewareResult Middleware(IEnumerable<SyntaxToken> tokens);

}
