using RSML.Parser;


namespace RSML.Samples
{

	internal interface ISample
	{

		string Content { get; }

		EvaluationProperties Properties { get; }

		EvaluationResult EvaluateSample();

	}

}
