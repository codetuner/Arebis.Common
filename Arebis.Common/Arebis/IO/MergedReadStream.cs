using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.IO
{
    // Alternative implementations on:
    // http://stackoverflow.com/questions/3879152/how-do-i-concatenate-two-system-io-stream-instances-into-one


    /// <summary>
    /// A read-only stream that reads from several streams in sequnce.
    /// </summary>
    public class MergedReadStream : Stream
    {
        private List<Stream> sourceStreams = new List<Stream>();

        private int currentStreamIndex = 0;

        private long currentPosition = 0L;

        /// <summary>
        /// Constructs a read-only stream that can read from several streams in sequence.
        /// </summary>
        public MergedReadStream()
        {
            this.CloseStreamsAtEnd = true;
        }

        /// <summary>
        /// Constructs a read-only stream that can read from several streams in sequence.
        /// </summary>
        public MergedReadStream(params Stream[] streams)
            : this()
        {
            this.AddStreams(streams);
        }

        /// <summary>
        /// Add a stream to be read in sequence.
        /// </summary>
        public MergedReadStream AddStream(Stream stream)
        {
            this.sourceStreams.Add(stream);
            return this;
        }

        /// <summary>
        /// Adds streams to be read in sequence.
        /// </summary>
        public MergedReadStream AddStreams(IEnumerable<Stream> streams)
        {
            this.sourceStreams.AddRange(streams);
            return this;
        }

        /// <summary>
        /// Whether to automatically close the streams as soon as their end is encountered.
        /// </summary>
        public bool CloseStreamsAtEnd { get; set; }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        { }

        /// <summary>
        /// Not supported.
        /// </summary>
        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Current stream position. Only read is supported.
        /// </summary>
        public override long Position
        {
            get
            {
                return currentPosition;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Reads a sequence of bytes from the streams.
        /// </summary>
        public override int Read(byte[] buffer, int offset, int count)
        {
            // If past last stream, reached end:
            if (currentStreamIndex >= sourceStreams.Count) return 0;

            // Read from current stream:
            int bytesRead = sourceStreams[currentStreamIndex].Read(buffer, offset, count);
            currentPosition += bytesRead;

            // If not enough bytes read, move to next stream and extend read recursively:
            if (bytesRead < count)
            {
                // Close current stream:
                if (CloseStreamsAtEnd) sourceStreams[currentStreamIndex].Close();
                // Move to next stream:
                currentStreamIndex++;
                // Complete buffer with recursive call to read:
                return bytesRead + Read(buffer, offset + bytesRead, count - bytesRead);
            }
            else
            {
                // Otherwise done:
                return bytesRead;
            }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
