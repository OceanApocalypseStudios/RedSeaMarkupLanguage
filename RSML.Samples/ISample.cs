using RSML.Evaluation;
using RSML.Machine;


namespace RSML.Samples
{

	internal interface ISample
	{

		string Content { get; }

		LocalMachine MachineData { get; }

		EvaluationResult EvaluateSample();

	}

}
