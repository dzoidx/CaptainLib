using System;
#if NETSTANDARD1_0
using System.Threading.Tasks;
#endif

namespace CaptainLib.Threading
{
    static class Extensions
    {

#if NETSTANDARD1_0
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
