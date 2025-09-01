namespace OceanApocalypseStudios.RSML.Actions
{

	/// <summary>
	/// Structure that holds some default special action behaviors.
	/// </summary>
	public readonly struct SpecialActionBehavior
	{

		/// <summary>
		/// Declares no behavioral effect.
		/// </summary>
		public const byte NoBehavior = 0;

		/// <summary>
		/// Declares success effect. This is interpreted as
		/// no behavioral effect.
		/// </summary>
		public const byte Success = 0;

		/// <summary>
		/// Declares an error effect. As of this version, anything
		/// between 1 and 249, both ends included, is considered
		/// an error effect.
		/// </summary>
		public const byte Error = 1;

		/// <summary>
		/// Stops evaluation at that point.
		/// </summary>
		public const byte StopEvaluation = 250;

		/// <summary>
		/// Resets all special actions.
		/// </summary>
		public const byte ResetSpecials = 251;

		/// <summary>
		/// Resets all operators.
		/// </summary>
		public const byte ResetOperators = 252;

	}

}
