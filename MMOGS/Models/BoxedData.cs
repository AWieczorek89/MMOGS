using System;

namespace MMOGS.Models
{
    public class BoxedData : IDisposable
    {
        public object Data { get; set; } = null;
        public string Msg { get; set; } = "";

        ~BoxedData()
        {
            this.Data = null;
            this.Msg = null;
        }

        public void Dispose()
        {
            this.Data = null;
            this.Msg = null;
        }
    }
}
