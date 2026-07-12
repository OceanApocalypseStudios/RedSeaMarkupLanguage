using System;
using System.IO;

using OceanApocalypseStudios.RSML.Execution;
using OceanApocalypseStudios.RSML.Language.Lexing;
using OceanApocalypseStudios.RSML.Language.Parsing;
using OceanApocalypseStudios.RSML.Sdk.Exceptions;
using OceanApocalypseStudios.RSML.Sdk.Extensibility;
using OceanApocalypseStudios.RSML.Sources;


namespace OceanApocalypseStudios.RSML.Sdk;

public sealed class ToolchainExecutionPlanBuilder
{
	private Lexer lexer;
	private Parser parser;
	private Interpreter interpreter;

	private bool hasBuilt = false;

	public ToolchainExecutionPlanBuilder Override(IToolchainComponent component)
	{
		// todo: implement this
		return this;
	}

	public ToolchainExecutionPlanBuilder Override<TToolchainComponent>()
		where TToolchainComponent : IToolchainComponent, new()
	{
		// todo: implement this
		return this;
	}

	public ToolchainExecutionPlanBuilder Register(ILanguageExtension extension)
	{
		// todo: implement this
		return this;
	}

	public ToolchainExecutionPlanBuilder Register<TExtension>()
		where TExtension : ILanguageExtension, new()
	{
		// todo: implement this
		return this;
	}

	public ToolchainExecutionPlanBuilder WithDataSource(ISource source)
	{
		// todo: implement this
		return this;
	}

	public ToolchainExecutionPlanBuilder WithDataSource(IBufferReader reader)
	{
		// todo: implement this
		return this;
	}

	public ToolchainExecutionPlanBuilder WithDataSource<TItem>(IBuffer<TItem> buffer)
	{
		// todo: implement this
		return this;
	}

	public ToolchainExecutionPlanBuilder WithDataSource(string? data)
	{
		// todo: implement this
		return this;
	}

	public ToolchainExecutionPlanBuilder WithDataSource(ReadOnlyMemory<char> data)
	{
		// todo: implement this
		return this;
	}

	public ToolchainExecutionPlanBuilder WithDataSource(FileInfo file)
	{
		// todo: implement this
		return this;
	}

	public ToolchainExecutionPlanBuilder WithDataSource(Stream stream)
	{
		// todo: implement this
		return this;
	}

	public ToolchainExecutionPlanBuilder WithConfiguration(ToolchainConfiguration configuration)
	{
		// todo: implement this
		return this;
	}

	public ToolchainExecutionPlan Build()
	{
		if (hasBuilt)
			throw new SdkException("The toolchain execution plan has already been created and cannot be built again.");

		// todo: implement build
		hasBuilt = true;
		throw new NotImplementedException();
	}
}
