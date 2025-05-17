param([System.IO.DirectoryInfo]$DeployFolder)

$repository = "$home\source\repos\spike"

#$result = dotnet build $repository\source\spec -c Release
#if ( $? ) { "Build spec done." } else { throw "$result" }

#$result = dotnet vstest $repository\source\spec\bin\Release\net8.0-windows\spec.dll
#if ( $? ) { "Execute spec done." } else { throw "$result" }

$name = 'module1'
$proj = "source\pwsh-pipeline1\$name"
$file = "$name.dll"
$manifest_name = "$name.psd1"
$manifest_file = Join-Path "$repository\$proj" $manifest_name
$result = dotnet build $repository\$proj -c Release
if ( ! $? ) { throw "$result" } else { "Build done." }
if ($DeployFolder.Exists)
{
	"Deploying to $DeployFolder ..."

    $target_folder = Join-Path $DeployFolder $name
    if(Test-Path $target_folder)
    {
        rm -Force -Recurse $target_folder
	    if ( $? ) { "$target_folder clean done." } else { throw "$result" }
    }

	$result = dotnet publish $repository\$proj -c Release --force --runtime win-x64 --self-contained -o $target_folder
	if ( $? ) { "Deployed to $target_folder" } else { throw "$result" }

	"Copying $manifest_file to $target_folder ..."
	$result = Copy-Item $manifest_file $target_folder -PassThru
	if ( ! $? ) { throw "Cannot copy $result" }
	else
	{
		"`nIn other process:`n`npushd $target_folder`nImport-Module $(Join-Path $target_folder $manifest_name)"
	}
} else { "No deploy required: DeployFolder ($DeployFolder) not found." }
