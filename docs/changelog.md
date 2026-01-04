---
hide:
  - navigation
---

# Changelog
## Red Sea Markup Language (v2.0.0+)

### 2.0.0 <small>September 9, 2025</small> { id="2.0.0" }
- Retired `RSML.Performance` for not being needed anymore
- Improved performance: as of this release, RSML is more than **100** times faster
- Improved the CLI
- Further developed the C ABI
- Implemented the `DualTextBuffer` class for performant buffering
- Closes #11: Rewrite Middleware logic
- Closes #12: Benchmark further and locate badly optimized spots
- Closes #15: Change namespaces to match studio's naming scheme
- Merged #16: Release v2.0.0

### 2.0.0-prerelease8 <small>August 18, 2025</small> { id="2.0.0-prerelease8" }
!!! warning
    This is a pre-release. Use at your own risk.

- Completely redesigned the language from scratch
- Removed the use of Regex for performance and simplicity reasons
- Added support for Notepad++ syntax highlighting
- Added benchmarking project
- Redesigned the CLI from scratch
- Started work on the C ABI for interoperability reasons
- Created a `.Performance` project due to performance on main project being absolutely awful
- Added testing project
- Redesigned Special Actions
- Added a parser, an evaluator, a semantic validator and a normalizer for syntactic and semantic reasons
- Added Middleware support
- Closed #1: Work on the documentation
- Closed #5: Update README (languages)
- Closed #6: Deploy the docs
- Closed #7: Documentation versioning
- Closed #8: Documentation index
- Closed #9: Remove `NonInteractibleTree`


## Red Sea Markup Language (Legacy)
!!! warning
    The releases in this section are considered **deprecated** and are largely out of support.

### 1.0.4 &amp; 1.0.5 <small>June 21, 2025</small> { id="1.0.5" }
- Removed reflection entirely, in order to produce an AOT-compatible CLI
- Set the project to be trimmed on build
- Moved from `RSML.Docs.vbproj` to `mkdocs` (Material theme)
- Closed #3: AOT this
- Merged #4: Update README.md with badge formatting and documentation clarifications

### 1.0.3 <small>June 14, 2025</small> { id="1.0.3" }
- Added more constructors to `RSParser` class
- Added async loading options for RSML documents
- Added the underlying expansion of `any` into `.+` as an opt-in but highly recommended feature
- Introduced more specialized commands for the different possibilities the new RSML features carried
- Closed #2: CLI bug, workflow not working and setup file

### 1.0.2 <small>June 12, 2025</small> { id="1.0.2" }
- Added Special Action support
- Added the possibility of parsing with custom RIDs
- Further documented RSML

### 1.0.1 <small>May 31, 2025</small> { id="1.0.1" }
- Fixed a critical bug where the parser would not restrict the system name Regex
- Improved the documentation building process using the custom `RSML.Docs.vbproj`

### 1.0.0 <small>May 28, 2025</small> { id="1.0.0" }
- Initial RSML Release


## MF's Crossroad Solution

### MF1.0 <small>Apr 19, 2025</small> { id="MF1.0" }
!!! warning
    MFRoad is no longer supported and has been publicly archived.

- Separated MFRoad from dying project ContenterX and archived it
