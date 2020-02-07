$token = "6bec40cf957de430a6f1f2baa056b99a4fac9ea0"
$net =Get-MerakiOrgs -Token $token | Where-Object { $_.name -eq "Devnet Sandbox" } | Get-MerakiNets -Token $token | Where-Object { $_.name -eq "DNSMB4-s.jayaraj007007@gmail.com" } 

Get-MerakiOrgs -Token $token | Where-Object { $_.name -eq "Devnet Sandbox" } | Get-MerakiNets -Token $token | Where-Object { $_.name -eq "DNSMB3" } | Get-MerakiClients -Token $token

