using PSPDFKitFoundation.Data;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace UWPSimpleExample
{
    /// <inheritdoc />
    /// <summary>
    /// Implements a <see cref="IDataSink"/> for a <see cref="IRandomAccessStream"/>
    /// </summary>
    public sealed class ExampleDataSink : IDataSink
    {
        /// <summary>
        /// Constructs a <see cref="ExampleDataSink"/> with a <see cref="DataSinkOption"/>.
        /// </summary>
        /// <param name="option">Whether data should be appended to or replaced.</param>
        public ExampleDataSink(DataSinkOption option)
        {
            DataSinkOption = option;
        }

        /// <summary>
        /// The <see cref="DataSinkOption"/> the data sink was constructed with.
        /// </summary>
        public DataSinkOption DataSinkOption { get; }

        /// <summary>
        /// Set to true if <see cref="ExampleDataSink.Finish"/> is called.
        /// </summary>
        public bool Finished { get; private set; } = false;

        /// <inheritdoc />
        public bool Finish()
        {
            Finished = true;
            return true;
        }

        /// <summary>
        /// The <see cref="IRandomAccessStream"/> to be written to.
        /// </summary>
        public IRandomAccessStream Stream { get; set; }

        private async Task<bool> WriteDataAsyncInternal(IBuffer data)
        {
            using (IOutputStream outputStream = Stream.GetOutputStreamAt(Stream.Size))
            {
                await outputStream.WriteAsync(data);
            }

            return true;
        }

        /// <inheritdoc />
        public IAsyncOperation<bool> WriteDataAsync(IBuffer data)
        {
            return WriteDataAsyncInternal(data).AsAsyncOperation();
        }
    }
}
