using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arebis.Collections
{
    public sealed class DisposablesCollection : IDisposable
    {
        private Stack<IDisposable> disposables = new Stack<IDisposable>();

        public T Add<T>(T resource) where T : IDisposable
        {
            this.disposables.Push(resource);
            return resource;
        }

        public void Dispose()
        {
            while (this.disposables.Count > 0)
            {
                this.disposables.Pop().Dispose();
            }
        }
    }
}
