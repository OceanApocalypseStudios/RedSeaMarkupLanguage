namespace OceanApocalypseStudios.RSML.Middlewares
{

	/// <summary>
	/// A middleware that is given a context and stops execution or allows it.
	/// </summary>
	public delegate bool Middleware(MiddlewareContext context);

}
