param(
     [Parameter(Mandatory=$False)]
     [string]
     $DeploymentName = "ContainersEverywhere",
     [Parameter(Mandatory=$False)]
     [string]
     $ComposeFilePath = "./docker-compose-asf.yml"
 )


 function Connect(){
    Connect-ServiceFabricCluster -ConnectionEndpoint containerseverywhere.westeurope.cloudapp.azure.com:19000 -X509Credential -FindType FindByThumbprint -FindValue D77444F0BD5B9CAAC0C0C9960478C2F771E461C2  -ServerCertThumbprint D77444F0BD5B9CAAC0C0C9960478C2F771E461C2 -StoreLocation CurrentUser
 }

 function Remove(){
    $existsing = Get-ServiceFabricComposeDeploymentStatus -DeploymentName $DeploymentName
    if ($existsing)
    {
        Remove-ServiceFabricComposeDeployment -DeploymentName $DeploymentName -Force

        do
        {
            Start-Sleep -Seconds 3
            $existsing = Get-ServiceFabricComposeDeploymentStatus -DeploymentName $DeploymentName
        }
        while ($existingApplication -ne $null)
    }
 }

 function Deploy(){
    
    $dockerRegistryEndpointName = Get-VstsInput -Name dockerRegistryEndpointName -Require
    $dockerRegistryEndpoint = Get-VstsEndpoint -Name $dockerRegistryEndpointName -Require
    $authParams = $dockerRegistryEndpoint.Auth.Parameters
    $username = $authParams.username
    $password = $authParams.password
    New-ServiceFabricComposeDeployment -DeploymentName $DeploymentName -Compose $ComposeFilePath -RegistryUserName $username -RegistryPassword $password
}