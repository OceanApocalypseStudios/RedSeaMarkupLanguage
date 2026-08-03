# OAS Security Policy
OceanApocalypse (OAS) takes security and performance very seriously across all repositories under the [OceanApocalypse](https://github.com/OceanApocalypse/) organization.

If you believe you have found a security vulnerability, please report it to us as shown below.

## What classifies as a vulnerability?
The following all classify as a security issue:
* **Remote code execution:** flaws that allow unauthorized code execution on servers or client machines;
* **Authentication issues:** broken access control allowing privilege escalation, horizontal data leaks (accessing another user's data), or MFA bypasses;
* **Injection Vulnerabilities:** SQL injections, command injections, LDAP injections, or flawed expression injections.
* **Exposure of sensitive data:** exposure of secrets and other sensitive data.
* **Other security issues.**

### What does NOT classify as a vulnerability?
* **Exposure of public data.**
* **Third-party dependency vulnerabilities:** we handle these with tools like Dependabot.
* **Anything not related to security:** performance issues, security-unrelated bugs, etc.

### Reporting a vulnerability
On GitHub, go to the **Security** tab (or **Security and quality**) and click **Report vulnerability**. Do **NOT** create a public issue.

Fill in the details and explain the vulnerability as best as possible.

## Supported _Red Sea Modern Language_ Versions
The following table displays the versions of RSML that have on-going support and have constant security updates ( :white_check_mark: ) and the ones that don't ( :x: ).

| Project                | Version              | Supported          | Notes                                                                                             |
| -------                | -------------------- | ------------------ | ------------------------------------------------------------------------------------------------- |
| RSML                   | 3.0.x                | :white_check_mark: |                                                                                                   |
| RSML                   | 2.1.0 _(unreleased)_ | :x:                |                                                                                                   |
| RSML                   | <= 2.0.x             | :x:                |                                                                                                   |
| RSML.CLI               | 3.0.x                | :white_check_mark: |                                                                                                   |
| RSML.CLI               | 2.1.0 _(unreleased)_ | :x:                |                                                                                                   |
| RSML.CLI               | <= 2.0.x             | :x:                |                                                                                                   |
| RSML for Visual Studio | 0.0.x                | :white_check_mark: | Report in [this repository](https://github.com/OceanApocalypse/RSML-VisualStudio) instead. |
| RSML for VS Code       | 0.0.x                | :white_check_mark: | Report in [this repository](https://github.com/OceanApocalypse/RSML-VSCode) instead.       |
