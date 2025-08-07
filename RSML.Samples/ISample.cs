using RSML.Machine;
using RSML.Parser;


namespace RSML.Samples
{

	internal interface ISample
	{

		string Content { get; }

		LocalMachine MachineData { get; }

		EvaluationResult EvaluateSample();

	}

}
