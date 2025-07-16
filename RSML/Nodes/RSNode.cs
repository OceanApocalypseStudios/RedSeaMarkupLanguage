using System;
using System.IO;
using System.Linq;


namespace RSML.Nodes
{

	/// <summary>
	/// The most basic representation of a generic Red Sea Markup Language
	/// node (single-line).
	/// </summary>
	public class RSNode
	{

		/// <summary>
		/// The data present in the node.
		/// </summary>
		public string nodeData = "";

		/// <summary>
		/// The node type of this RedSea node.
		/// </summary>
		public virtual NodeType NodeType { get; } = NodeType.Generic;

		/// <summary>
		/// Creates a new instance of a node with a string.
		/// Only the first line of the string will be used as
		/// the actual node itself.
		/// </summary>
		/// <param name="node">
		/// A string, of which the first line wil be interpreted as the node
		/// </param>
		public RSNode(string node)
		{

			nodeData = node.ReplaceLineEndings()
						   .Contains(Environment.NewLine) ? nodeData.Split(Environment.NewLine)
																	.FirstOrDefault("") : nodeData;

		}

		/// <summary>
		/// Creates a new instance of a node with an implementation
		/// of a <see cref="TextReader" />. The next line to be consumed
		/// will be used as the actual node itself. The node will fallback to
		/// <see cref="String.Empty" /> if there are no more lines to read.
		/// </summary>
		/// <param name="reader">A <see cref="TextReader" /> to read from</param>
		public RSNode(TextReader reader)
		{

			nodeData = reader.ReadLine() ?? "";

		}

		/// <summary>
		/// Creates a new instance of a node with a <see cref="Span{T}" />,
		/// where <c>T</c> is <see cref="Char" />.
		/// </summary>
		/// <param name="characters">The <see cref="Span{Char}"/>.</param>
		public RSNode(Span<char> characters) : this(characters.ToString()) { }

		/// <summary>
		/// Evaluates the node using immediate evaluation.
		/// </summary>
		/// <param name="properties">The properties of the evaluation</param>
		/// <returns>
		/// Null, as <see cref="RSNode" /> is not meant to be used directly - use
		/// one of its subclasses instead
		/// </returns>
		public virtual string? Evaluate(EvaluationProperties properties) => null;

		/// <summary>
		/// Returns the actual node as a string.
		/// </summary>
		/// <returns>The node in the form of a string</returns>
		public override string ToString() => nodeData;

		/// <summary>
		/// Checks if this node equals another representation
		/// of a node, which can be of types <see cref="String" />
		/// or <see cref="RSNode" />.
		/// </summary>
		/// <param name="obj">The object to check for equality against</param>
		/// <returns><c>true</c> if the objects represent the same node</returns>
		public override bool Equals(object? obj)
		{

			if (obj is RSNode node)
				return Equals(node);

			if (obj is string str)
				return Equals(str);

			return nodeData == obj?.ToString();

		}

		/// <summary>
		/// Checks if this node equals a <see cref="String" />.
		/// </summary>
		/// <param name="node">The string to check against</param>
		/// <returns><c>true</c> if the object represents the same node</returns>
		protected internal bool Equals(string node) => nodeData == node;

		/// <summary>
		/// Checks if this node equals a <see cref="RSNode" />.
		/// </summary>
		/// <param name="node">The node to check against</param>
		/// <returns><c>true</c> if the object represents the same node</returns>
		protected internal bool Equals(RSNode node) => this == node;

		/// <summary>
		/// Retrieves the hash code of the underlying string.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() => nodeData.GetHashCode();

	}

}
