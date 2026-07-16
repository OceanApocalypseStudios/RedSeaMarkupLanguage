using System.Collections;
using System.Collections.Generic;


namespace OceanApocalypseStudios.RSML.Language.Lexing;

public struct LexerRuleset : ICollection<ILexerRule>
{
	private List<ILexerRule> rules;

	public int Count => rules.Count;

	public bool IsReadOnly => false;

	public void Add(ILexerRule item) => rules.Add(item);
	public void Clear() => throw new System.NotImplementedException();
	public bool Contains(ILexerRule item) => throw new System.NotImplementedException();
	public void CopyTo(ILexerRule[] array, int arrayIndex) => throw new System.NotImplementedException();
	public IEnumerator<ILexerRule> GetEnumerator() => throw new System.NotImplementedException();
	public bool Remove(ILexerRule item) => throw new System.NotImplementedException();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
