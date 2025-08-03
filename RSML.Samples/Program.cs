using System;


namespace RSML.Samples
{

	internal class Program
	{

		private static void Main()
		{

			while (true)
			{

				Console.Write("Insert sample ID: ");
				int id = Convert.ToInt32(Console.ReadLine());
				Console.Write('\n');

				if (id < 1)
					return;

				var res = SampleRunner.Run(id);
				Console.WriteLine($"{res?.ToString()} : {res?.WasMatchFound}");

				_ = Console.ReadLine();
				Console.Clear();

			}

		}

	}

}
