using System.Threading;
using System.Threading.Tasks;

namespace ArgbConverter.ProcessingClasses
{
    public static class GlobalProcessor
    {
        public static Task SleepTaskStart(int ms)
        {
            var t = new Task ( () => Thread.Sleep(ms) );
            t.Start();
            return t;
        }
    }
}
