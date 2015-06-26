#
# Removes two custom virtual directories from Code Vision site on IISExpress
#
$appcmd = "c:\Program Files (x86)\IIS Express\appcmd.exe"

# Find site name on a given port
$port = 3500
if((.$appcmd list site http://localhost:$port | Out-String) -match '(?<=SITE\s*")(.*?)(?=.\s*\(id:)' -eq $false)
{    
    throw "No site on port " + $port
}
$siteName = $Matches[0]

.$appcmd delete app $siteName/searchindex
.$appcmd delete app $siteName/searchcontent
