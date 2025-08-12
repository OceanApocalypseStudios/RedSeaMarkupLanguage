# Goals by version
Starting with the next one :)

## RSML v2.0.0-alpha
It's close. *Very* close.

### Testing

- [X] Test even more potential edge cases, like the `==10` edge case that caused an infinite loop *(that one's fixed
  tho)*

### Performance

- [X] Compiled RSML via ~~Source Generators~~ (ended up going for MSBuild Tasks for compatibility reasons)
- [X] **Benchmark RSML for C#**
	- [ ] Optimize if necessary

### Native

- [ ] Export metadata
- [ ] Export `RsLexer`
- [ ] Export `RsReader`
- [ ] Export `RsValidator`
- [ ] Export `RsEvaluator`

### Language Specification

- [X] **Lexical Structure**
	- [X] Character Set
	- [X] Tokens
	- [X] Comments
	- [X] Whitespace

- [ ] **Syntax**
	- [ ] Markup Structure
	- [ ] Statements
	- [ ] Operators

- [ ] **Semantics**
	- [ ] Syntax Errors

- [ ] **Backwards-Compatibility** (in this case, none)

- [ ] **Example Usage**
	- [ ] Hello, World!
	- [ ] Common Patterns

- [ ] **Implementation Notes**

- [ ] **Appendix**
	- [ ] Glossary
	- [ ] Supported Programming Languages

- [ ] **References**

### CommandLine Interface

- [X] Improve on Specification Support (`--specification-support`)
- [X] Improve the help screen
- [ ] Add actual evaluation support from `stdin`
- [ ] Add tokenization support *(somehow)*
- [ ] Add semantic validation support *(somehow)*
- [ ] Add links to documentation

### Documentation

- [ ] RSML for C# Documentation
- [ ] Native RSML Docs
- [ ] Improvements to documentation organization (in particular, migrating to Microsoft Learn-style documentation)
