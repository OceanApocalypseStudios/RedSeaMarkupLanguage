using RSML.Evaluation;


namespace RSML.Actions
{

	/// <summary>
	/// Delegate for RSML special actions.
	/// </summary>
	/// <param name="evaluator">The RSML evaluator that invoked this delegate</param>
	/// <param name="actionArgument">The string argument passed to this delegate</param>
	/// <returns>A behavior of a special action</returns>
	/// <remarks>For the behaviors, you might want to see <see cref="SpecialActionBehavior" />'s constants.</remarks>
	public delegate byte SpecialAction(IEvaluator? evaluator, string actionArgument);

}
