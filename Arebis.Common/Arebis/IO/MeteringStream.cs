using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.IO
{
    /// <summary>
    /// A stream that keeps track of the number of BytesRead and BytesWritten.
    /// </summary>
    public class MeteringStream : Stream
    {
        /// <summary>
        /// A stream that wraps an innerStream and keeps track of the number of BytesRead and BytesWritten.
        /// </summary>
        public MeteringStream(Stream innerStream)
        {
            this.InnerStream = innerStream;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing) this.InnerStream.Dispose();
        }

        /// <summary>
        /// The inner stream to which bytes are read from or written to.
        /// </summary>
        public Stream InnerStream { get; private set; }

        /// <summary>
        /// The number of bytes read.
        /// </summary>
        public long BytesRead { get; private set; }
        
        /// <summary>
        /// The number of bytes written.
        /// </summary>
        public long BytesWritten { get; private set; }

        /// <summary>
        /// Resets the BytesRead and BytesWritten counters.
        /// </summary>
        public void Reset()
        {
            this.BytesRead = 0L;
            this.BytesWritten = 0L;
        }

        public override bool CanRead
        {
            get { return this.InnerStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return this.InnerStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return this.InnerStream.CanWrite; }
        }

        public override void Flush()
        {
            this.InnerStream.Flush();
        }

        public override long Length
        {
            get { return this.InnerStream.Length; }
        }

        public override long Position
        {
            get
            {
                return this.InnerStream.Position;
            }
            set
            {
                this.InnerStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var result = this.InnerStream.Read(buffer, offset, count);
            this.BytesRead += result;
            return result;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.InnerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.InnerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.InnerStream.Write(buffer, offset, count);
            this.BytesWritten += count;
        }
    }
}
