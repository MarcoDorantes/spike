namespace cmdlet1;

using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

[Cmdlet(VerbsCommunications.Read, "File")]
[OutputType(typeof(string))]
public class ReadFileCommand : PSCmdlet
{
    [Parameter(
        Mandatory = true,
        Position = 0,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
    public string FilePath { get; set; }

    protected override void BeginProcessing() { WriteWarning(nameof(BeginProcessing)); }
    protected override void ProcessRecord() { WriteWarning(nameof(ProcessRecord)); }
    protected override void EndProcessing() { WriteWarning(nameof(EndProcessing)); }
}