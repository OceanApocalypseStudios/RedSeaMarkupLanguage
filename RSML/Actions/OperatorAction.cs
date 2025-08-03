using System;

using RSML.Parser;


namespace RSML.Actions
{

	/// <summary>
	/// Delegate for RSML operator actions.
	/// </summary>
	/// <param name="parser">The RSML parser that invoked this delegate</param>
	/// <param name="right">The right side of the operator (the argument)</param>
	public delegate void OperatorAction(RsParser parser, ReadOnlySpan<char> right);

}
