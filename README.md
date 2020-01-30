Sample code works like so:

Clone the repo to a folder of your desire

```
git clone https://github.com/DataKnox/PwshMerakiSDK.git
```

Open up your command line and navigate to..
```
cd $basefolder/PwshMerakiSDK/bin/Debug/netstandard2.0/GetMerakiOrgsCmdlet.dll   
```

Run dotnet build. Note this was created using .Net Core 3.1 and tested with Powershell Core 7
```
dotnet build
```
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
Import-Module $basefolder/MerakiSDK/PwshMerakiSDK/bin/Debug/netstandard2.0/GetMerakiOrgsCmdlet.dll
```
Test your script
```
$token = "6bec40cf957de430a6f1f2baa056b99a4fac9ea0"
Get-MerakiOrgs -Token $token | Where-Object { $_.name -eq "Devnet Sandbox" } | `
    Get-MerakiNets -Token $token | Where-Object { $_.name -eq "DNSMB3" } | `
    Get-MerakiDevices -Token $token 
```
