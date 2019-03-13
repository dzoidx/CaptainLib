# Remove delegate for collections
Notify on remove
``` c#
var l = new LimitedList<int>(1, i => Console.WriteLine($"{i} removed"));
l.Add(1);
l.Add(2);
l.Add(3);
```
output:
```
1 removed
2 removed
```
Dispose on remove
``` c#
class Dummy : IDisposable
{
	Guid id = Guid.NewGuid();
	public Dummy()
	{
		Console.WriteLine($"{id} ctor.");
	}

	public void Dispose()
	{
		Console.WriteLine($"{id} dispose.");
	}
}

class Program
{
	static void Main(string[] args)
	{
		var l = new LimitedQueue<Dummy>(1, d => d.Dispose());
		l.Enqueue(new Dummy());
		l.Enqueue(new Dummy());
		l.Enqueue(new Dummy());
	}
}

```
output:
```
dddda738-3fad-4f00-9f74-b979a87d6326 ctor.
481460e5-190f-4a2e-ac21-7aef89e212b3 ctor.
dddda738-3fad-4f00-9f74-b979a87d6326 dispose.
aba2bc92-2022-4c45-9ad7-4f5b27fa04ce ctor.
481460e5-190f-4a2e-ac21-7aef89e212b3 dispose.
```