# CaptainLib
Some useful C# stuff you copy from one project to another
- Common
  - ```ScopedObject<T>``` - object which can delete himself when it's out from the scope (like RAII in C++). [Usage...](#scopedobject-usage)
- Collections
  - ```LimitedList<T>, LimitedQueue<T>``` - collections with size threshold. [Usage...](#limitedlist-usage)
  - ```MemCache<Key, Value>``` - create or get something in/from cache. [Usage...](#memcache-usage)
  - ```ObjectPool<T>``` - hmm..it's an object pool and an object's array pool. [Usage...](#objectpool-usage)
  - ```IEnumerable<T>.Randomize()``` - extension for shuffling elements in the sequence. [Usage...](#randomization-of-enumerable)
- Numbers
  - ```BcdNumber``` - binary coded decimal (BCD) implementation. Big floating point numbers
  - ```Range<T>``` - if you want to define subset of numbers
  - ```SafeRandom``` - thread-safe random numbers. [See a definition of the problem](https://blogs.msdn.microsoft.com/pfxteam/2009/02/19/getting-random-numbers-in-a-thread-safe-way/)
- Threading
  - ```ScopeWorker``` - Thread which stops on Dispose(). [Usage...](#scopeworker-usage)
  
 See project [Sandbox](https://github.com/dzoidx/CaptainLib/tree/master/Sandbox/Showcases) for usage examples
# ScopedObject usage
``` c#
Func<int> createFunc = () => { Console.WriteLine("Object created"); return 0; };
Action<int> removeFunc = (o) => Console.WriteLine($"Object removed. Value '{o}'");

Console.WriteLine("Entering the object scope");
using (new Common.ScopedObject<int>(createFunc, removeFunc))
{
    Console.WriteLine("Inside an object scope");
}
Console.WriteLine("Out of an object scope");
```
output:
```
Entering the object scope
Object created
Inside an object scope
Object removed. Value '0'
Out of an object scope
```
# LimitedList usage
``` c#
var list = new Collections.LimitedList<int>(10);
foreach (var n in Enumerable.Range(0, 50))
{
    list.Add(n);
}

Console.Write("Limited list content: ");
foreach (var n in list)
{
    Console.Write($"{n} ");
}
```
output:
```
Limited list content: 40 41 42 43 44 45 46 47 48 49
```
# Randomization of enumerable
``` c#
var list = Enumerable.Range(0, 10);

Console.Write("Original list: ");
foreach (var n in list)
{
    Console.Write($"{n} ");
}
Console.WriteLine();

var shuffledList = list.Randomize();

Console.Write("Shuffled list: ");
foreach (var n in shuffledList)
{
    Console.Write($"{n} ");
}
Console.WriteLine();
```
output:
```
Original list: 0 1 2 3 4 5 6 7 8 9
Shuffled list: 9 1 4 7 6 3 2 8 0 5
```
# ScopeWorker usage
Define a class:
``` c#
class SomeWork : ScopeWorker
{
    private int _iteration;
    
    protected override void Update(float dt, CancellationToken cancellation)
    {
        Console.WriteLine($"{_iteration++}: {dt}");
    }
}
```
when test it like so:
``` c#
Console.WriteLine("Entering the scope");
using (new SomeWork())
{
    Thread.Sleep(5);
}
Console.WriteLine("Out of a scope");
```
output:
```
Entering the scope
0: 1,462852E-06
1: 0,0003130502
2: 0,0001208315
3: 4,82741E-05
4: 2,340562E-05
5: 1,755422E-05
6: 1,696908E-05
7: 1,609137E-05
8: 3,335302E-05
9: 3,861928E-05
10: 3,832671E-05
11: 3,920442E-05
12: 2,340562E-05
13: 1,901707E-05
14: 1,960221E-05
15: 3,364559E-05
16: 3,393815E-05
17: 2,428333E-05
18: 2,311305E-05
19: 3,218273E-05
20: 1,813936E-05
21: 2,867189E-05
22: 0,0003159759
23: 0,0002346414
24: 0,0002109432
Out of a scope
```
# ObjectPool usage
Define a class:
``` c#
class PoolMe
{
    public PoolMe()
    {
        Console.WriteLine("New instance of PoolMe");
    }
}
```
when test it like so:
``` c#
// 10 is max capacity no relevance to this test
var pool = new ObjectPool<PoolMe>(10, () => new PoolMe());
Console.WriteLine("Getting 1 from a pool");
using (var obj1 = pool.Alloc())
{
    Console.WriteLine("Getting 2 from a pool");
    using (var obj2 = pool.Alloc())
    {
    }
    Console.WriteLine("Returning 2 to a pool");

    Console.WriteLine("Getting 3 from a pool");
    using (var obj3 = pool.Alloc())
    {
    }
    Console.WriteLine("Returning 3 to a pool");
}
Console.WriteLine("Returning 1 to a pool");
```
output:
```
Getting 1 from a pool
New instance of PoolMe
Getting 2 from a pool
New instance of PoolMe
Returning 2 to a pool
Getting 3 from a pool
Returning 3 to a pool
Returning 1 to a pool
```
# MemCache usage
Define a class:
``` c#
class CacheMe : IDisposable
{
    private readonly int _key;

    public CacheMe(int key)
    {
        _key = key;
        Console.WriteLine($"New instance of CacheMe. Key {_key}");
    }

    public void Dispose()
    {
        Console.WriteLine($"CacheMe disposed. Key {_key}");
    }
}
```
when test it like so:
``` c#
var cache = new MemCache<int, CacheMe>();

Console.WriteLine("Getting non-cached object two times");
cache.GetOrCreate(0, () => new CacheMe(0), TimeSpan.Zero);
cache.GetOrCreate(0, () => new CacheMe(0), TimeSpan.Zero);
Console.WriteLine("Creating cached object");
cache.GetOrCreate(1, () => new CacheMe(1), TimeSpan.FromMilliseconds(100));
Console.WriteLine("Getting cached object two times");
cache.GetOrCreate(1, () => new CacheMe(1), TimeSpan.FromMilliseconds(100));
cache.GetOrCreate(1, () => new CacheMe(1), TimeSpan.FromMilliseconds(100));
Console.WriteLine("Wait cache to expire");
Thread.Sleep(100);
cache.GetOrCreate(1, () => new CacheMe(1), TimeSpan.FromMilliseconds(100));
```
output:
```
Getting non-cached object two times
New instance of CacheMe. Key 0
CacheMe disposed. Key 0
New instance of CacheMe. Key 0
Creating cached object
CacheMe disposed. Key 0
New instance of CacheMe. Key 1
Getting cached object two times
Wait cache to expire
CacheMe disposed. Key 1
New instance of CacheMe. Key 1
```
