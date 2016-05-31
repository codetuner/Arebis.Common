using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Xsl;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Arebis.Caching
{
    /// <summary>
    /// A cache for XSLT document files.
    /// </summary>
    public class XsltFileCache : FileCache<XslCompiledTransform>
    {
        /// <summary>
        /// Instantiates a file cache to hold compiled XSLT documents.
        /// </summary>
        public XsltFileCache()
        { }

        /// <summary>
        /// Instantiates a file cache to hold compiled XSLT documents.
        /// </summary>
        public XsltFileCache(string appSettingsBaseKey)
            :base(appSettingsBaseKey)
        { }

        /// <summary>
        /// Instantiates a file cache to hold compiled XSLT documents up to the given number or
        /// file size rules.
        /// </summary>
        public XsltFileCache(string appSettingsBaseKey, int defaultMaxFileCount, long defaultMaxFileLengthToCache, long defaultMaxFileLengthSum)
            : base(appSettingsBaseKey, defaultMaxFileCount, defaultMaxFileLengthToCache, defaultMaxFileLengthSum)
        { }

        /// <summary>
        /// Instantiates a file cache to hold compiled XSLT documents up to the given number or
        /// file size rules.
        /// </summary>
        public XsltFileCache(int maxFileCount, long maxFileLengthToCache, long maxFileLengthSum)
            : base(maxFileCount, maxFileLengthToCache, maxFileLengthSum)
        { }

        protected override XslCompiledTransform Load(string filePath)
        {
            XslCompiledTransform document = new XslCompiledTransform();
            document.Load(filePath);

            return document;
        }
    }

    
    /// <summary>
    /// A cache for XSLT document files.
    /// </summary>
    public class XsltCache2
    {
        private const int _lockTimeout = 10000;
        private Dictionary<string, XsltCacheSlot> _xsltCache = new Dictionary<string, XsltCacheSlot>();
        private ReaderWriterLock _lock = new ReaderWriterLock();

        /// <summary>
        /// Returns the requested XSLT document.
        /// </summary>
        /// <param name="filePath">The filename of the XSLT document.</param>
        /// <returns>The requested XSLT document from cache.</returns>
        public XslCompiledTransform Get(string filePath)
        {
            // Validate arguments:
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("The specified key cannot be an empty string or null");
            }

            // Acquire a reader lock:
            _lock.AcquireReaderLock(_lockTimeout);
            try
            {
                // Retrieve actual file timestamp:
                var actualTicks = File.GetLastWriteTime(filePath).Ticks; // If files does not exist, returns 01/01/1601 01:00:00, so anyway, does not throw an exception...

                // Try to retrieve the document from cache:
                XsltCacheSlot slot;
                if (_xsltCache.TryGetValue(filePath, out slot))
                {
                    // If found, verify it's timestamp, and if current, return the document:
                    if (slot.FileLastWriteTimeTicks == actualTicks)
                    {
                        return slot.Document;
                    }
                }

                // Upgrade to a writer lock:
                _lock.UpgradeToWriterLock(_lockTimeout);

                // Create a new cache slot by loading the document from disk:
                slot = _xsltCache[filePath] = new XsltCacheSlot() {
                    FilePath = filePath,
                    Document = this.Load(filePath),
                    FileLastWriteTimeTicks = actualTicks
                };

                // Return the document:
                return slot.Document;
            }
            finally
            {
                // Release reader and/or writer lock:
                _lock.ReleaseLock();
            }
        }

        /// <summary>
        /// Removes a xlscompileTransform from the cache
        /// </summary>
        /// <param name="filePath">The filename of the XSLT template.</param>
        public void Invalidate(string filePath)
        {
            try
            {
                _lock.AcquireWriterLock(_lockTimeout);
                _xsltCache[filePath] = null;
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Loads an XSLT document and returns the document
        /// as compiled XSLT.
        /// </summary>
        /// <param name="filePath">The filename of the XSLT template.</param>
        private XslCompiledTransform Load(string filePath)
        {
            XslCompiledTransform myTransformer = new XslCompiledTransform();
            myTransformer.Load(filePath);

            return myTransformer;
        }

        class XsltCacheSlot {

            public string FilePath { get; set; }

            public long FileLastWriteTimeTicks { get; set; }

            public XslCompiledTransform Document { get; set; }

            public override bool Equals(object obj)
            {
                var other = obj as XsltCacheSlot;
                if (other == null)
                    return false;
                else
                    return String.Equals(this.FilePath, other.FilePath);
            }

            public override int GetHashCode()
            {
                return this.FilePath.GetHashCode();
            }
        }
    }
}
