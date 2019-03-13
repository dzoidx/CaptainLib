# Exception propagation
Idea for handling task exceptions in single place and for processing result of a task which completion are not awaited.
``` c#
static class ErrorHandler
{
	public static void Handle(Exception e)
	{
		Console.WriteLine(e);
	}
}


class Program
{
	static void Main(string[] args)
	{
		var t = Task.Run(() => throw null)
			.HandleError(ErrorHandler.Handle);
	}
}
```