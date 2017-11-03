param(
     [Parameter(Mandatory=$False)]
     [string]
     $DeploymentName = "ContainersEverywhere",
     [Parameter(Mandatory=$False)]
     [string]
     $ComposeFilePath = "./docker-compose-asf.yml",
     [Parameter(Mandatory=$False)]
     [string]
     $DockerRegistryEndpointName ="ContainerRegistry",
     [Parameter(Mandatory=$False)]
     [string]
     $CertThumbPrint = "D77444F0BD5B9CAAC0C0C9960478C2F771E461C2",
     [Parameter(Mandatory=$False)]
     [string]
     $ClusterEndpoint = "containerseverywhere.westeurope.cloudapp.azure.com:19000"
 )

 #for local testing
 function Connect(){
    Connect-ServiceFabricCluster -ConnectionEndpoint $ClusterEndpoint -X509Credential -StoreLocation CurrentUser -FindType FindByThumbprint -FindValue $CertThumbPrint -ServerCertThumbprint $CertThumbPrint 
 }

 #remove existing compose deployment and wait for completion
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
        while ($existsing -ne $null)
    }
 }

 #Rollout new compose deployment
 function Deploy(){
    
    $DockerRegistryEndpointName = Get-VstsInput -Name $DockerRegistryEndpointName -Require
    $dockerRegistryEndpoint = Get-VstsEndpoint -Name $DockerRegistryEndpointName -Require
    $authParams = $dockerRegistryEndpoint.Auth.Parameters
    $username = $authParams.username
    $password = $authParams.password
    New-ServiceFabricComposeDeployment -DeploymentName $DeploymentName -Compose $ComposeFilePath -RegistryUserName $username -RegistryPassword $password
}

#$connected = Get-ServiceFabricApplication -ErrorAction Continue
#if(!$connected)
#{
#    Connect
#}
#
#$connected = Get-ServiceFabricApplication -ErrorAction Continue
#if(!$connected)
#{
#    Write-Error "Failed to connect to cluster"
#    exit
#}

Remove
Deploy