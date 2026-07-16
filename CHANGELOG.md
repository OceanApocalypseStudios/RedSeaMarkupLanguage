## v3.0.0-prerelease1

### 🚀 Features
- *(api)* [**breaking**] Add the extensibility project
- *(buffer)* [**breaking**] Implement the read-only span buffer for minimum allocations
- *(toolchain)* Implement all members from `IToolchainComponent` in the classes that implement it
- *(buffer)* Remove `GetWord`-like methods and implement `GetSourceSpan` in the read-only string buffer
- *(buffer)* Add more equality checks for the buffers
- *(extensions)* Start working on extension support little by little
- *(buffer)* Add properties for checking if a source is read-only and checking its cursor position
- [**breaking**] Add multi-target support for .NET 8.0, .NET 472 and .NET 481
- *(buffer)* [**breaking**] Add array-based methods to the read-only buffer interface and class, for convenience (#53 closed by #59)
- *(buffer)* Add methods for calculating the length of a line in the buffer (#58)
- *(buffer)* Add and test `TryGetLineFromIndex` method to the read-only string buffer (#57)
- Featuring nuget trends

### 🐛 Bug Fixes
- [**breaking**] Fixed major bug where `SourceSpan` did not check if the start index was less than the end index
- *(buffer)* Fix last line specific logic when counting until the end of the line
- *(buffer)* Fix out of range calculations not taking negative indexes into account
- *(buffer)* [**breaking**] Fix `CountUntilLineSeparator` returning off-by-one and negative results
- *(buffer)* Made a condition more obvious on what the consequences are
- *(test)* Fix MTP setup
- *(buffer)* Fix methods and line counting mechanics in the read-only string buffer
- Fix and test native exports (1/?)
- Fix maybe??

### 🚧 Refactor
- Place all internal-use extension members in a single file
- Suppress minor issues and remove unnecessary extension methods
- *(buffer)* Rename the read-only string buffer to `ReadOnlyCharBuffer` to indicate `TItem` is `System.Char` instead of `System.String`
- *(buffer)* Add `GetSourceSpan` method and remove variations of `GetWord`
- Rename `CountUntilLineSeparator` to `CountUntilEndOfLine`
- Rename `InternalUtils` to `Constants` and move the OAS item interface to a better namespace
- Use C# 14 extension blocks instead of static extension methods
- *(sdk)* Remove `IInjectable` for not being necessary anymore
- *(sdk)* Reorganized the RSML API and SDK
- Move types around namespaces and add directories to have code in the future
- *(buffer)* Move the buffers to the parent `Sources` directory (and namespace)
- *(cache)* Move `ISupportsCache` to a dedicated cache directory (and namespace)
- *(buffer)* Remove unnecessary methods and remove `Try*` pattern from `TryGetSourceLocation`

### ⚡ Performance
- *(buffer)* [**breaking**] Use result value types over exceptions for performance
- Perform a solution-wise code cleanup
- Perform solution-wise code cleanup and test the native normalizer (#29)

### 📚 Documentation
- *(buffer)* Document exceptions thrown by methods and EOF conventions
- *(buffer)* Document behavior where EOF convention is not followed by location-finding method
- *(api)* Update .NET API documentation
- *(blog)* Add article explaining the amount of tests implemented
- *(buffer)* Document `GetLine`'s EOF conventions
- *(buffer)* Add notes about EOF conventions in XML documentation of some methods
- *(blog)* Add the very first blog article and organize documentation better
- Mark documentation as to be done
- Migrate from mkdocs to DocFX
- Remove my public email address from the RSML docs
- Improve documentation partially
- Fix broken shortcodes
- Docs have been moved here

### 🎨 Styling
- *(docs)* Improved light theme for the documentation website

### 🧪 Testing
- *(buffer)* Finish testing the read-only string buffer and fix some faulty behaviors while computing line starts
- *(buffer)* Test constructors and slicing methods for the read-only string buffer
- *(buffer)* Implement tests for `GetLineFromIndex` and `GetSourceLocation`
- *(buffer)* Test error-throwing buffer logic
- *(buffers)* Test fixed version of `CountUntilEndOfLine` and add more tests for it
- *(buffer)* Add tests for verifying if `GetLengthOfLine` thrown when empty or out of range
- *(buffer)* Add tests for equality checks within the read-only string buffer
- *(buffer)* Test more counting functions and further test `CountWhile`
- *(buffer)* Heavily test several buffer functions related to counting
- [**breaking**] Add test projects for testing RSML, RSML's SDK and RSML's Native exports
- Test `CountLinesBefore` method and close #51
- Test the native evaluator (#29) and improve native <-> managed conversions
- Test the native validator as requested in #29

### ⚙️ Miscellaneous
- Include new extensibility project to security information
- *(deps.nuget)* Bump the minor-and-patch group with 3 updates (#66)
- Reduce GitHub Actions usage time by running automated tests only on .NET 10.0
- Update git attributes with new content and normalize file encodings
- Update changelogs with latest commits
- Remove useless comments and update security policy
- Remove SDK project and have the SDK in the RSML project instead
- Update changelog with latest available data
- Update release workflow to use Trusted Publishers over API keys
- Setup code owners for the repository
- Fix Dependabot workflow issue
- Update workflows and add security information
- Remove docs output from the repository
- Add platform support to the RSML solution
- Make the CLI non-packable
- Debug release workflow on Linux ARM64
- Debug release workflow on macOS
- Improve README.md and add test badges
- Prefer bash over PowerShell to avoid YAML escaping issues
- Update workflow to avoid linker issues
- Fix workflow toa void parser error on Windows ARM64
- Make RSML.Native not pack on build
- Comment out the release step (the only goal is testing)
- Move the MTP migration property to a props file
- Migrate to MTP
- Fix and update workflows
- Remove old benchmarking results

> [!NOTE]
> Previous to `v3.0.0-prerelease1`, [Conventional Commits](conventionalcommits.org/en/v1.0.0/) were not enforced in the repository, hence the lack of content below this note.

## 2.0.0 @ 2025-09-09

### 🚧 Refactor
- Refactoringsss

### ⚡ Performance
- Its actually good now

## 2.0.0-prerelease8 @ 2025-08-18

### ⚡ Performance
- PERF. OR. MANCE.
- Performance goes boom

### 📚 Documentation
- Docs (part 3) not much done tho
- Docs (part 2)

### 🧪 Testing
- Tests not passing life sux

## 1.0.1 @ 2025-05-31

### 📚 Documentation
- Docs part 1

## 1.0.0 @ 2025-05-28

### 🐛 Bug Fixes
- Fixed it
- Fixed it (hopefully)
