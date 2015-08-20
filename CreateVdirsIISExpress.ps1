#
# Creates two virtual directories for CodeVision web site on IISExpress.
# Assumes initial IISEexpress configuration is done and the site is up on a given port.
#

function CreateVirtualDirectory($path, $physicalPath)
{    
   .$appcmd add APP /site.name:$siteName /path:$path /physicalPath:$physicalPath
   .$appcmd set app /app.name:$siteName$path /applicationPool:"Clr4IntegratedAppPool"
}

$appcmd = "c:\Program Files (x86)\IIS Express\appcmd.exe"

# Find site name on a given port
$port = 3500
if((.$appcmd list site http://localhost:$port | Out-String) -match '(?<=SITE\s*")(.*?)(?=.\s*\(id:)' -eq $false)
{    
    throw "No site on port " + $port
}
$siteName = $Matches[0]

# List apps for the site
$vdir1 = "searchindex"
if ((.$appcmd list apps /site.name:$siteName | Out-String) -match ($vdir1) -eq $true)
{
    Write-Host Virtual directories are already set up.
    break
}

#Grab root physical path
if((.$appcmd list vdir $siteName/ | Out-String) -match '(?<=physicalPath:)(.*?)(?=\))' -eq $false)
{    
    throw Could not find physical path for $siteName/
}
$rootPhysicalPath = $Matches[0]

#Determine root solution path
if($rootPhysicalPath -match '.*(?=\\CodeVision.Web)' -eq $false)
{    
    throw Could not find root solution path
}
$rootSolutionPath = $Matches[0]

#Created virtual directories
CreateVirtualDirectory -path /searchindex -physicalPath $rootSolutionPath\CodeVision.Tests\bin\Debug\Index
CreateVirtualDirectory -path /searchcontent -physicalPath $rootSolutionPath\CodeVision.Tests\Content

Write-Host Done:
.$appcmd list apps /site.name:$siteName

