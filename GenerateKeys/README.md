# GenerateKeys

This app is provided to generate (or re-generate) SSL certificates for use
with the Example apps when you use their `_ssl.cfg` configurations.

These Example `_ssl.cfg configurations use the following files,
but they are only valid for a few days.

* Examples/QuickFixn-TestCA.cer
* Examples/QuickFixn-TestClient.pfx
* Examples/QuickFixn-TestServer.pfx

To use the SSL configs, you will need to create updated versions of these files.

**This program creates these files.**

A full C# app may seem like overkill, but it's the only way we've yet found that
is cross-platform, i.e. works on Windows and Unix.
(Powershell scripts require a module that is Windows-only.)

## Usage Example

```
~/quickfixn/GenerateKeys$ dotnet run ../Examples/
Writing CACertificate: ../Examples/QuickFixn-TestCA.cer
Writing ServerCertificate: ../Examples/QuickFixn-TestServer.pfx
Writing ClientCertificate: ../Examples/QuickFixn-TestClient.pfx
```
