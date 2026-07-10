## Unreleased

### 🚀 Features

- *(buffer)* Add and test `TryGetLineFromIndex` method to the read-only string buffer (#57)
- *(buffer)* Add methods for calculating the length of a line in the buffer (#58)
- *(buffer)* [**breaking**] Add array-based methods to the read-only buffer interface and class, for convenience (#53 closed by #59)
- *(buffer)* Add properties for checking if a source is read-only and checking its cursor position

### 🐛 Bug Fixes

- *(buffer)* Fix methods and line counting mechanics in the read-only string buffer
- *(test)* Fix MTP setup

### Other

- Add more tests

### 🚧 Refactor

- *(cache)* Move `ISupportsCache` to a dedicated cache directory (and namespace)

### 📚 Documentation

- Improve documentation partially
- Migrate from mkdocs to DocFX
- Mark documentation as to be done
- *(blog)* Add the very first blog article and organize documentation better

### 🧪 Testing

- [**breaking**] Add test projects for testing RSML, RSML's SDK and RSML's Native exports

### ⚙️ Miscellaneous Tasks

- Migrate to MTP
- Move the MTP migration property to a props file
- Comment out the release step (the only goal is testing)
- Fix workflow toa void parser error on Windows ARM64
- Update workflow to avoid linker issues
- Prefer bash over PowerShell to avoid YAML escaping issues
- Improve README.md and add test badges
- Debug release workflow on macOS
- Debug release workflow on Linux ARM64
- Make the CLI non-packable
- Add platform support to the RSML solution
- Remove docs output from the repository
- Update workflows and add security information
- Fix Dependabot workflow issue
- Setup code owners for the repository
- Update release workflow to use Trusted Publishers over API keys
## 2.0.0 @ 2025-09-09

### ⚡ Performance

- Its actually good now
## 1.0.0 @ 2025-05-28
