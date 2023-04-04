using PSPDFKitFoundation.Data;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;
using Buffer = Windows.Storage.Streams.Buffer;

namespace UWPSimpleExample
{
    public sealed class ExampleDataProvider : IDataProvider
    {
        /// <summary>
        /// Constructs a <see cref="ExampleDataProvider"/> from an <see cref="IRandomAccessStream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="IRandomAccessStream"/> that provides the data.</param>
        /// <param name="hashString">String representing the assigned file for creating a persistent HashCode. Ex: file names.</param>
        public ExampleDataProvider(IRandomAccessStream stream)
        {
            _data = stream;
            SupportsWriting = _data.CanWrite;
        }

        private readonly IRandomAccessStream _data;

        /// <inheritdoc />
        public ulong Size => _data.Size;

        /// <inheritdoc />
        public string Uid => GetHashCode().ToString();

        /// <inheritdoc />
        public bool SupportsWriting { get; private set; }

        /// <inheritdoc />
        public IDataSink CreateDataSink(DataSinkOption option)
        {
            if (option != DataSinkOption.NewFile)
            {
                throw new Exception("ExampleDataProvider cannot create a data sink that appends to the original source.");
            }
            return new ExampleDataSink(option) { Stream = new InMemoryRandomAccessStream() };
        }

        private async Task<IBuffer> ReadAsyncInternal(uint size, uint offset)
        {
            if (offset >= _data.Size)
            {
                throw new Exception("Attempt to read beyond the end of a random access stream.");
            }

            Buffer copy = new Buffer(size);
            _data.Seek(offset);
            return await _data.ReadAsync(copy, size, InputStreamOptions.ReadAhead);
        }

        /// <inheritdoc />
        public IAsyncOperation<IBuffer> ReadAsync(uint size, uint offset)
        {
            return ReadAsyncInternal(size, offset).AsAsyncOperation();
        }

        private async Task<bool> ReplaceWithDataSinkAsyncInternal(IDataSink dataSink)
        {
            if (!_data.CanWrite)
            {
                throw new Exception("This DataProvider is not writable and therefore you cannot replace it's content.");
            }

            if (dataSink is ExampleDataSink streamSink)
            {
                if (streamSink.DataSinkOption == DataSinkOption.NewFile)
                {
                    _data.Seek(0);
                    _data.Size = 0;
                }

                Stream sinkStream = streamSink.Stream.AsStream();
                sinkStream.Seek(0, SeekOrigin.Begin);
                Stream dataStream = _data.AsStream();
                await sinkStream.CopyToAsync(dataStream);
                await dataStream.FlushAsync();

                return true;
            }
            else
            {
                throw new NotImplementedException("Support for this kind of IDataSink is not supported.");
            }
        }

        /// <inheritdoc />
        public IAsyncOperation<bool> ReplaceWithDataSinkAsync(IDataSink dataSink)
        {
            return ReplaceWithDataSinkAsyncInternal(dataSink).AsAsyncOperation();
        }
    }
}
