using System;

using OceanApocalypseStudios.RSML.Language.Lexing;


namespace OceanApocalypseStudios.RSML.Sdk.Extensibility.Registries;

public class LexerRuleRegistry
{
	public int GetNextTokenKindId()
	{
		// todo: implement this
		throw new NotImplementedException();
	}

	public ILexerRule? GetOrRegisterRule(int kind, ILexerRule rule)
	{
		if (TryGetRule(kind, out var existingRule))
			return existingRule;

		RegisterToken(kind, rule);
		return null;
	}

	public void RegisterLiteralPrefix(int kind, string prefix, ILexerRule rule)
	{
		// todo: implement this
		throw new NotImplementedException();
	}

	public void RegisterKeyword(int kind, string keyword)
	{
		// todo: implement this
		throw new NotImplementedException();
	}

	public void RegisterToken(int kind, ILexerRule rule)
	{
		// todo: implement this
		throw new NotImplementedException();
	}

	public bool TryGetRule(int kind, out ILexerRule rule)
	{
		// todo: implement this
		throw new NotImplementedException();
	}
}
