using System;
#if NETSTANDARD1_3 || NET45
using System.Threading.Tasks;
#endif

namespace CaptainLib.Threading
{
    public static class Extensions
    {

#if NETSTANDARD1_3 || NET45
        public static Task HandleError(this Task task, Action<Exception> callback)
        {
            return 
                task.ContinueWith(_ =>
                {
                    if (_.IsFaulted)
                        callback(_.Exception.Flatten());
                });
        }
#endif
    }
}
