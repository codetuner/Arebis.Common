using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Xsl;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Configuration;

namespace Arebis.Caching
{
    /// <summary>
    /// A base implementation for a cache that contains file system files and invalidates cache
    /// data when file has changed on filesystem.
    /// </summary>
    public abstract class FileCache<TFile>
    where TFile : class
    {
        private const int lockTimeout = 10000;
        private Dictionary<string, FileCacheSlot> documentsCache = new Dictionary<string, FileCacheSlot>();
        private Queue<string> documentsQueue = new Queue<string>();
        private ReaderWriterLock cacheLock = new ReaderWriterLock();
        private long actualFileLengthSum = 0L;

        /// <summary>
        /// Instantiates a new, unlimited FileCache.
        /// </summary>
        public FileCache()
            : this(int.MaxValue, long.MaxValue, long.MaxValue)
        { }

        /// <summary>
        /// Instantiates a new FileCache.
        /// Takes settings for this instance by extending the given appSettingBaseKey with respectively ".MaxFileCount", ".MaxFileLengthToCache" and ".MaxFileLengthSum".
        /// Missing appSetting keys default to unlimited.
        /// </summary>
        /// <param name="appSettingsBaseKey">The base key name of appSettings.</param>
        public FileCache(string appSettingsBaseKey)
            : this(appSettingsBaseKey, Int32.MaxValue, Int64.MaxValue, Int64.MaxValue)
        { }

        /// <summary>
        /// Instantiates a new FileCache.
        /// Takes settings for this instance by extending the given appSettingBaseKey with respectively ".MaxFileCount", ".MaxFileLengthToCache" and ".MaxFileLengthSum".
        /// For each missing appSetting, the given default value is taken.
        /// </summary>
        /// <param name="appSettingsBaseKey">The base key name of appSettings.</param>
        /// <param name="defaultMaxFileCount">The value for MaxFileCount when no matching appSetting key is defined.</param>
        /// <param name="defaultMaxFileLengthToCache">The value for MaxFileLengthToCache when no matching appSetting key is defined.</param>
        /// <param name="defaultMaxFileLengthSum">The value for MaxFileLengthSum when no matching appSetting key is defined.</param>
        public FileCache(string appSettingsBaseKey, int defaultMaxFileCount, long defaultMaxFileLengthToCache, long defaultMaxFileLengthSum)
            : this(
                Int32.Parse(ConfigurationManager.AppSettings[appSettingsBaseKey + ".MaxFileCount"] ?? defaultMaxFileCount.ToString()),
                Int64.Parse(ConfigurationManager.AppSettings[appSettingsBaseKey + ".MaxFileLengthToCache"] ?? defaultMaxFileLengthToCache.ToString()),
                Int64.Parse(ConfigurationManager.AppSettings[appSettingsBaseKey + ".MaxFileLengthSum"] ?? defaultMaxFileLengthSum.ToString())
            )
        { }
       
        /// <summary>
        /// Instantiages a new FileCache with given count and size limitations.
        /// </summary>
        /// <param name="maxFileCount">The maximum numbers of files the cache should contain.</param>
        /// <param name="maxFileLengthToCache">The maximum a single file should have, to be allowed to the cache.</param>
        /// <param name="maxFileLengthSum">The maximum file size summed for all files in cache.</param>
        public FileCache(int maxFileCount, long maxFileLengthToCache, long maxFileLengthSum)
        {
            this.MaxFileCount = maxFileCount;
            this.MaxFileLengthSum = maxFileLengthSum;
        }

        /// <summary>
        /// The maximum numbers of files the cache should contain.
        /// </summary>
        public int MaxFileCount { get; set; }

        /// <summary>
        /// The maximum a single file should have, to be allowed to the cache.
        /// Too large files are simply not stored in the cache.
        /// </summary>
        public long MaxFileLengthToCache { get; set; }

        /// <summary>
        /// The maximum file size summed for all files in cache.
        /// If exceeded, files are flushed from the cache.
        /// </summary>
        public long MaxFileLengthSum { get; set; }

        /// <summary>
        /// Returns the requested document.
        /// </summary>
        /// <param name="filePath">The filename of the document.</param>
        /// <returns>The requested document from cache.</returns>
        public TFile Get(string filePath)
        {
            // Validate arguments:
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("The specified key cannot be an empty string or null");
            }

            // Acquire a reader lock:
            cacheLock.AcquireReaderLock(lockTimeout);
            try
            {
                // Retrieve actual file timestamp:
                var fileInfo = new FileInfo(filePath);
                var actualTicks = fileInfo.LastWriteTime.Ticks; // If files does not exist, returns 01/01/1601 01:00:00, so anyway, does not throw an exception...
                var fileLength = fileInfo.Length;

                // Try to retrieve the document from cache:
                FileCacheSlot slot;
                if (documentsCache.TryGetValue(filePath, out slot))
                {
                    // If found, verify it's timestamp, and if current, return the document:
                    if (slot.FileLastWriteTimeTicks == actualTicks)
                    {
                        return slot.Document;
                    }
                    else
                    {
                        // Invalidate the slot and decrease the actualFileLengthSum:
                        slot.FileLastWriteTimeTicks = DateTime.MinValue.Ticks;
                        slot.FileLength = 0L;
                        actualFileLengthSum -= slot.FileLength;
                    }
                }

                // Upgrade to a writer lock:
                cacheLock.UpgradeToWriterLock(lockTimeout);

                // Check if file is not too large to cache:
                if (fileLength <= MaxFileLengthToCache)
                {
                    // Create a new cache slot by loading the document from disk:
                    SkimCache(fileLength);
                    documentsQueue.Enqueue(filePath);
                    actualFileLengthSum += fileLength;
                    slot = documentsCache[filePath] = new FileCacheSlot()
                    {
                        FilePath = filePath,
                        Document = this.Load(filePath),
                        FileLastWriteTimeTicks = actualTicks,
                        FileLength = fileLength
                    };

                    // Return the document:
                    return slot.Document;
                }
                else
                {
                    // Load and return document without caching:
                    return this.Load(filePath);
                }
            }
            finally
            {
                // Release reader and/or writer lock:
                cacheLock.ReleaseLock();
            }
        }

        /// <summary>
        /// If needed, removes items from the cache to make room for a new one with given fileLength.
        /// </summary>
        /// <param name="fileLength">Length of a file that will be added.</param>
        private void SkimCache(long fileLength)
        {
            var maxAllowedLength = MaxFileLengthSum - fileLength;
            while ((documentsQueue.Count >= MaxFileCount) || (actualFileLengthSum > maxAllowedLength))
            {
                var path = documentsQueue.Dequeue();
                var cdoc = documentsCache[path];
                actualFileLengthSum -= cdoc.FileLength;
                documentsCache.Remove(path);
            }
        }

        /// <summary>
        /// Loads a document and returns the document as TFile object.
        /// </summary>
        /// <param name="filePath">The filename of the document.</param>
        protected abstract TFile Load(string filePath);

        /// <summary>
        /// Internal cache slot implementation class.
        /// </summary>
        private class FileCacheSlot
        {

            public string FilePath { get; set; }

            public long FileLastWriteTimeTicks { get; set; }

            public long FileLength { get; set; }

            public TFile Document { get; set; }

            public override bool Equals(object obj)
            {
                var other = obj as FileCacheSlot;
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
