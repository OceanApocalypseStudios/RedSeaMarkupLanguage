using RSML.Evaluation;
using RSML.Machine;


namespace RSML.Samples.Samples
{

	public class Sample1 : ISample
	{


		/// <inheritdoc />
		public string Content => "-> \"This will work\"";

		/// <inheritdoc />
		public LocalMachine MachineData => new();

		/// <inheritdoc />
		public EvaluationResult EvaluateSample() => new RsEvaluator(Content).Evaluate(MachineData);

	}

}
