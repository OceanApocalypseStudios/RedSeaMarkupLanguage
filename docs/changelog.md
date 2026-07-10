# Changelog

## v3.0.0-prerelease1 (in development)
Currently in development, will lead to the biggest RSML release yet: **v3.0.0**.

### 🚀 Features
- *(buffer)* Add and test `TryGetLineFromIndex` method to the read-only string buffer (#57)
- *(buffer)* Add methods for calculating the length of a line in the buffer (#58)
- *(buffer)* [**breaking**] Add array-based methods to the read-only buffer interface and class, for convenience (#53 closed by #59)
- *(buffer)* Add properties for checking if a source is read-only and checking its cursor position

### 🐛 Bug Fixes
- *(buffer)* Fix methods and line counting mechanics in the read-only string buffer
- *(test)* Fix MTP setup

### 🚧 Refactor
- *(cache)* Move `ISupportsCache` to a dedicated cache directory (and namespace)

### 📚 Documentation
- Improve documentation partially

### 🧪 Testing
- Add more tests
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

## 2.0.0 @ 2025-09-09
Breaking changes for performance sake.

### ⚡ Performance
- Its actually good now

## 1.0.0 @ 2025-05-28
First public RSML release.
