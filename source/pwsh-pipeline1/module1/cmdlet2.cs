namespace cmdlet1;

using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

[Cmdlet(VerbsCommunications.Read, "Pipe")]
[OutputType(typeof(string))]
public class ReadPipeCommand : PSCmdlet
{
    [Parameter(
        Mandatory = true,
        Position = 0,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
    public string InputObject { get; set; }

    [Parameter(
        Mandatory = true,
        Position = 1,
        ValueFromPipeline = false,
        ValueFromPipelineByPropertyName = false)]
    public string Tag { get; set; }

    protected override void BeginProcessing()
    {
        WriteWarning($"{DateTime.Now:mm:ss.fffffff} {Tag} {nameof(BeginProcessing)}: InputObject=[{InputObject}]");
    }
    protected override void ProcessRecord()
    {
        WriteWarning($"{DateTime.Now:mm:ss.fffffff} {Tag} {nameof(ProcessRecord)}: InputObject=[{InputObject}]");
        WriteObject(InputObject);
    }
    protected override void EndProcessing()
    {
        WriteWarning($"{DateTime.Now:mm:ss.fffffff} {Tag} {nameof(EndProcessing)}: InputObject=[{InputObject}]");
    }
}