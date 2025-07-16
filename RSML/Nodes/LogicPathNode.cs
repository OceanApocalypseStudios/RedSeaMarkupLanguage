using System.IO;


namespace RSML.Nodes
{

	/// <summary>
	/// Represents a Red Sea Markup Language logic path node, which can
	/// be evaluated according to the OS and CPU architecture.
	/// </summary>
	public class LogicPathNode : RSNode
	{

		/// <inheritdoc />
		public override NodeType NodeType => NodeType.LogicPath;

		/// <inheritdoc />
		public LogicPathNode(string? node) : base(node ?? "") { }

		/// <inheritdoc />
		public LogicPathNode(TextReader reader) : base(reader) { }

		// todo: document this
		public override string? Evaluate(EvaluationProperties properties)
		{

			// todo: code this

		}

		/// <inheritdoc />
		public override bool Equals(object? obj) => base.Equals(obj);

		/// <inheritdoc />
		public override int GetHashCode() => base.GetHashCode();

		/// <inheritdoc />
		public override string ToString() => nodeData;

	}

}
