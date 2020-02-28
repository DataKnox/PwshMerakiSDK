## Requirements

This Powershell module is a binary Powershell module, meaning it is written and compiled in C# into a DLL file. 
To make this Powershell module portable among all platforms, .Net Core v3.0 or higher is required. The C# code targets C# 8.0, and thus must be used with .Net Core v3

You can install the latest version of .Net Core here: https://dotnet.microsoft.com/download/dotnet-core

This code is also tested with Powershell Core v7, and is recommended to be used with Powershell Core v7 or higher.
You can get Powershell Core v7 here: https://github.com/PowerShell/PowerShell

## Notes

This repo is still a work in progress, and I endeavor to add a new Cmdlet every other day. Add, Set, and Delete cmdlets take a little more time to build because of their parameters.

## Instructions

Sample code works like so: Clone the repo to a folder of your desire

```
mkdir MerakiPosh
cd MerakiPosh
git clone https://github.com/DataKnox/PwshMerakiSDK.git
```

Now navigate into the newly cloned folder named PwshMerakiSDK
```
cd PwshMerakiSDK
```

Run dotnet build. Note this was created using .Net Core 3.1 and tested with Powershell Core 7
```
dotnet build
```
This compiles all of the .CS files into a DLL which can be imported by Powershell using Import-Module

Launch Powershell
```
pwsh
or
pwsh-preview
or
powershell
```

Import the module
```
Import-Module MerakiPosh/MerakiSDK/PwshMerakiSDK/bin/Debug/netstandard2.0/GetMerakiOrgsCmdlet.dll
```

View all available Cmdlets with 
```
Get-Command -module GetMerakiOrgsCmdlet
```
### Example: Test your script using the DevNet Sandbox token
The below code will retrieve a list of Orgs, filter it down to the DevNet Sandbox org, then retrieve a list of networks from that org, filter it down to DNSMB3, and then get the list of devices on that network
```
$token = "6bec40cf957de430a6f1f2baa056b99a4fac9ea0"
Get-MerakiOrgs -Token $token | Where-Object { $_.name -eq "Devnet Sandbox" } | `
    Get-MerakiNets -Token $token | Where-Object { $_.name -eq "DNSMB3" } | `
    Get-MerakiDevices -Token $token 
```

## About me

Knox the superman of superthings!

Find me here:
Twitter.com/data_knox
YouTube.com/c/DataKnox
