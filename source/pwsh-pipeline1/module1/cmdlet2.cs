namespace module1;

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

    [Parameter(
        Mandatory = false,
        Position = 3,
        ValueFromPipeline = false,
        ValueFromPipelineByPropertyName = false)]
    public SwitchParameter T { get; set; }

    protected override void BeginProcessing()
    {
        var line=$"{DateTime.Now:mm:ss.fffffff} {Tag} {nameof(BeginProcessing)}: InputObject=[{InputObject}]";
        WriteWarning(line);
        if(T.IsPresent) WriteObject(line);
    }
    protected override void ProcessRecord()
    {
        var line=$"{DateTime.Now:mm:ss.fffffff} {Tag} {nameof(ProcessRecord)}: InputObject=[{InputObject}]";
        WriteWarning(line);
        WriteObject(line);
    }
    protected override void EndProcessing()
    {
        var line=$"{DateTime.Now:mm:ss.fffffff} {Tag} {nameof(EndProcessing)}: InputObject=[{InputObject}]";
        WriteWarning(line);
        if(T.IsPresent) WriteObject(line);
    }
}