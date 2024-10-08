namespace InstanceModelSpec;

class Window
{
    public Window()
    {
    }

    public void Open(){}
    public void Close(){}
}

class Engine
{
    public Engine()
    {
        window = new();
    }

    public Window window {get;set;}

    public void Execute()
    {
        window.Open();
        window.Close();
    }
}

class HostProcess
{
    public HostProcess()
    {
        engine = new();
    }

    public Engine engine {get;set;}

    public void Execute()
    {
        engine.Execute();
    }
}

[TestClass]
public class BasicStructureSpec
{
   [TestMethod]
    public void struct1()
    {
        HostProcess process = new();
        process.Execute();
        Assert.IsNotNull(process);
        Assert.IsNotNull(process.engine);
        Assert.IsNotNull(process.engine.window);
    }
}