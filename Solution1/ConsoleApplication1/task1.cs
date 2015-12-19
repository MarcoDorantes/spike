using System;
using System.Threading.Tasks;

interface IA
{
	Task f();
	Task f0();
	Task f2();
	Task<int> f3();
}

class A:IA
{
	public Task f()
	{
		return new Task(()=>Console.WriteLine($"[{Environment.CurrentManagedThreadId}] task"));
	}
	public Task f0()
	{
		return Task.Run(()=>Console.WriteLine($"[{Environment.CurrentManagedThreadId}] task0"));
	}
	public async Task f2()
	{
		await f0();
	}
	public async Task<int> _f3() => await Task.Run(()=>12*12);
	public async Task<int> f3()
	{
		return await _f3();
	}
}

class exe
{
	static void g(IA a)
	{
		var t=a.f();
		t.Start();
	}
	static async void h(IA a)
	{
		await a.f2();
	}
	static async void i(IA a)
	{
		Console.WriteLine($"[{Environment.CurrentManagedThreadId}] i-begin");
		int r = await a.f3();
		Console.WriteLine($"[{Environment.CurrentManagedThreadId}] i-result: {r}");
	}
	static void _Main()
	{
	try{
		Console.WriteLine($"[{Environment.CurrentManagedThreadId}] Begin");
		var a=new A();
		g(a);
		h(a);
		i(a);

		Console.WriteLine($"[{Environment.CurrentManagedThreadId}] Done");
		Console.ReadLine();
	}catch(Exception ex){Console.WriteLine($"{ex.GetType().FullName}: {ex.Message}");}
	}
}