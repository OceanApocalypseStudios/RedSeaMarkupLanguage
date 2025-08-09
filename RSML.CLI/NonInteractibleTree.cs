using System.Collections.Generic;

using Spectre.Console;
using Spectre.Console.Rendering;


namespace RSML.CLI
{

	public class NonInteractibleTree(
		string label
	) : IRenderable
	{

		private readonly Tree tree = new(label);

		public Measurement Measure(RenderOptions _, int maxWidth) => new(1, maxWidth);

		public IEnumerable<Segment> Render(RenderOptions _, int maxWidth) => tree.GetSegments(AnsiConsole.Create(new()));

		public TreeNode AddNode(IRenderable renderable) => tree.AddNode(renderable);

		public TreeNode AddNode(string markup) => tree.AddNode(markup);

		public TreeNode AddNode(TreeNode node) => tree.AddNode(node);

		public void AddNodes(params TreeNode[] nodes) => tree.AddNodes(nodes);

	}

}
