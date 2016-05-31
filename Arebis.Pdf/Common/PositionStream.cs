using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Pdf.Common
{
    class PositionStream : System.IO.Stream
    {
        private long position = 0L;

        private long bufferSize = 0L;
        private long autoFlushTreshold = 16384L;
        private bool transparantFlush = false;

        public PositionStream(System.IO.Stream innerStream)
        {
            this.InnerStream = innerStream;
        }

        public System.IO.Stream InnerStream { get; private set; }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            if (this.transparantFlush)
                this.InnerStream.Flush();
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get
            {
                return position;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.InnerStream.Write(buffer, offset, count);
            this.position += count;

            this.bufferSize += count;
            if (this.bufferSize > this.autoFlushTreshold)
            {
                this.InnerStream.Flush();
                this.bufferSize = 0L;
            }
        }

        public override void Close()
        {
            this.InnerStream.Close();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing) InnerStream.Dispose();
        }
    }
}
