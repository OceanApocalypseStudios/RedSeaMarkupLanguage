using System;
using System.Reflection;
using RSML.Evaluation;


namespace RSML.Samples
{

	internal static class SampleRunner
	{

		internal static EvaluationResult Run<TSample>()
			where TSample : ISample, new() =>
			Activator.CreateInstance<TSample>().EvaluateSample();

		internal static EvaluationResult? Run(int sampleId)
		{

			var sample = Assembly.GetExecutingAssembly()
								 ?.GetType($"RSML.Samples.Samples.Sample{sampleId}")
								 ?.GetConstructor(Type.EmptyTypes)
								 ?.Invoke([ ]) as ISample;

			return sample?.EvaluateSample();

		}

	}

}
