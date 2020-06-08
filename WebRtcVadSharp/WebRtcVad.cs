using System;
using System.Diagnostics;
using System.IO;
using WebRtcVadSharp.WebRtc;

namespace WebRtcVadSharp
{
    /// <summary>
    /// A .NET adapter for WebRTC's voice activity detection library.
    /// </summary>
    /// <remarks>
    /// Because the underlying WebRTC library has robust (and fast) error checking
    /// of its own, this class primarily validates input arguments <i>after</i> the raw
    /// WebRTC calls have failed to provide specific information about what went
    /// wrong. This approach avoids costly enum conversions/validations until a
    /// failure state obtains, at which point performance presumably doesn't matter.
    /// </remarks>
    public class WebRtcVad : IWebRtcVad, IDisposable
    {
        /// <summary>
        /// Creates and initializes a WebRTC voice activity detector.
        /// </summary>
        public WebRtcVad() : this(new WebRtcDll()) { }

        /// <summary>
        /// Creates and initializes a WebRTC voice activity detector.
        /// </summary>
        /// <remarks>
        /// This constructor injects a WebRTC library for unit testing, rather than
        /// depending on the P/Invoke implementation provided by the default constructor.
        /// </remarks>
        /// <param name="library">Interface for the underlying WebRTC library.</param>
        public WebRtcVad(IWebRtcDll library)
        {
            _webrtc = library ?? throw new ArgumentNullException(nameof(library));
            _mode = OperatingMode.HighQuality;
            _rate = SampleRate.Is8kHz;
            _length = FrameLength.Is10ms;

            try
            {
                _handle = _webrtc.Create();
                var result = _webrtc.Init(_handle);
                ValidateInitialize(result, _handle);
            }
            catch (DllNotFoundException e)
            {
                var currentDir = Directory.GetCurrentDirectory();
                throw new DllNotFoundException($"Unable to load DLL 'WebRtcVad.dll' or a dependency. Be sure it exists in '{currentDir}' or elsewhere in the DLL search path.", e);
            }
        }

        private readonly IWebRtcDll _webrtc;
        private IntPtr _handle;
        private SampleRate _rate;
        private FrameLength _length;
        private OperatingMode _mode;

        /// <summary>
        /// Gets or sets the sample rate when using the audio-only <see cref="HasSpeech(byte[])"/> overload.
        /// Defaults to <see cref="SampleRate.Is8kHz"/>.
        /// </summary>
        public SampleRate SampleRate
        {
            get => _rate;
            set
            {
                ValidateRateAndFrameLength(value, FrameLength);
                _rate = value;
            }
        }

        /// <summary>
        /// Gets or sets the frame length when using the audio-only <see cref="HasSpeech(byte[])"/> overload.
        /// Defaults to <see cref="FrameLength.Is10ms"/>.
        /// </summary>
        public FrameLength FrameLength
        {
            get => _length;
            set
            {
                ValidateRateAndFrameLength(SampleRate, value);
                _length = value;
            }
        }

        /// <summary>
        /// Gets or sets the aggressiveness of the detection.
        /// Defaults to <see cref="OperatingMode.HighQuality"/>.
        /// </summary>
        /// <seealso cref="WebRtcVadSharp.OperatingMode"/>
        public OperatingMode OperatingMode
        {
            get => _mode;
            set
            {
                var result = _webrtc.SetMode(_handle, (int)value);
                ValidateSetMode(result, _handle, value);
                _mode = value;
            }
        }

        /// <inheritdoc/>
        public bool HasSpeech(byte[] audioFrame)
        {
            return HasSpeech(audioFrame, _rate, _length);
        }

        /// <inheritdoc/>
        public bool HasSpeech(byte[] audioFrame, SampleRate sampleRate, FrameLength frameLength)
        {
            long frameSamples = CalculateSamples(sampleRate, frameLength);
            Debug.Assert(audioFrame != null, $"'{nameof(audioFrame)}' must not be null");
            Debug.Assert(audioFrame.Length >= frameSamples * 2, $"Audio must contain at least {frameSamples} 16-bit samples ({frameSamples * 2} bytes)");

            var result = _webrtc.Process(_handle, (int)sampleRate, audioFrame, frameSamples);
            return ValidateProcess(result, _handle, sampleRate, audioFrame, frameLength);
        }

        /// <inheritdoc/>
        public bool HasSpeech(short[] audioFrame)
        {
            return HasSpeech(audioFrame, _rate, _length);
        }

        /// <inheritdoc/>
        public bool HasSpeech(short[] audioFrame, SampleRate sampleRate, FrameLength frameLength)
        {
            long frameSamples = CalculateSamples(sampleRate, frameLength);
            Debug.Assert(audioFrame != null, $"'{nameof(audioFrame)}' must not be null");
            Debug.Assert(audioFrame.Length >= frameSamples, $"Audio must contain at least {frameSamples} 16-bit samples");

            var result = _webrtc.Process(_handle, (int)sampleRate, audioFrame, frameSamples);
            return ValidateProcess(result, _handle, sampleRate, audioFrame, frameLength);
        }

        /// <inheritdoc/>
        public bool HasSpeech(IntPtr audioFrame)
        {
            return HasSpeech(audioFrame, _rate, _length);
        }

        /// <inheritdoc/>
        public bool HasSpeech(IntPtr audioFrame, SampleRate sampleRate, FrameLength frameLength)
        {
            long frameSamples = CalculateSamples(sampleRate, frameLength);
            Debug.Assert(audioFrame != IntPtr.Zero, $"'{nameof(audioFrame)}' must not be a null pointer");

            var result = _webrtc.Process(_handle, (int)sampleRate, audioFrame, frameSamples);
            return ValidateProcess(result, _handle, sampleRate, audioFrame, frameLength);
        }

        #region Result/argument validation

        private long CalculateSamples(SampleRate rate, FrameLength length)
        {
            // calculate a number of 16-bit samples
            return (int)rate / 1000 * (long)length;
        }

        private void ValidateInitialize(int result, IntPtr handle)
        {
            switch (result)
            {
                case 0: return;
                default:
                    ValidateHandle(handle);
                    throw new InvalidOperationException($"Could not initialize WebRTC [Init({handle}) = {result}].");
            };
        }

        private void ValidateSetMode(int result, IntPtr handle, OperatingMode mode)
        {
            switch (result)
            {
                case 0: return;
                default:
                    ValidateHandle(handle);
                    ValidateEnum(mode);
                    throw new InvalidOperationException($"Could not set operating mode [SetMode({handle}, {mode}) = {result}].");
            };
        }

        private void ValidateRateAndFrameLength(SampleRate sampleRate, FrameLength frameLength)
        {
            var samples = CalculateSamples(sampleRate, frameLength);
            var result = _webrtc.ValidRateAndFrameLength((int)sampleRate, samples);
            switch (result)
            {
                case 0: return;
                default:
                    ValidateEnum(sampleRate);
                    ValidateEnum(frameLength);
                    throw new InvalidOperationException($"Could not validate rate/length [ValidRateAndFrameLength({(int)sampleRate}, {samples}) = {result}].");
            };
        }

        private bool ValidateProcess(int result, IntPtr handle, SampleRate sampleRate, byte[] audioFrame, FrameLength frameLength)
        {
            return ValidateProcess(result, handle, sampleRate, $"byte[{audioFrame.Length}]", frameLength);
        }

        private bool ValidateProcess(int result, IntPtr handle, SampleRate sampleRate, short[] audioFrame, FrameLength frameLength)
        {
            return ValidateProcess(result, handle, sampleRate, $"short[{audioFrame.Length}]", frameLength);
        }

        private bool ValidateProcess(int result, IntPtr handle, SampleRate sampleRate, IntPtr audioFrame, FrameLength frameLength)
        {
            return ValidateProcess(result, handle, sampleRate, "0x" + audioFrame.ToInt64().ToString("X8"), frameLength);
        }

        private bool ValidateProcess(int result, IntPtr handle, SampleRate sampleRate, string frameDescription, FrameLength frameLength)
        {
            switch (result)
            {
                case 0: return false;
                case 1: return true;
                default:
                    ValidateHandle(handle);
                    ValidateEnum(sampleRate);
                    ValidateEnum(frameLength);
                    throw new InvalidOperationException($"Could not process audio frame [Process({handle}, {sampleRate}, <{frameDescription}>, {frameLength}) = {result}].");
            };
        }

        private void ValidateHandle(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw new ObjectDisposedException($"Invalid WebRTC handle; object appears to have been disposed.");
        }

        private void ValidateEnum<T>(T val)
        {
            if (Enum.IsDefined(typeof(T), val))
                return;

            var validValues = string.Join(", ", Enum.GetValues(typeof(T)));
            throw new ArgumentException($"{val} was not a valid {typeof(T).Name}. Valid values are [{validValues}].");
        }
        
        #endregion

        #region IDisposable support

        private bool hasDisposed = false;

        /// <summary>
        /// Dispose the underlying WebRTC resources.
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (hasDisposed)
                return;

            if (disposing)
            {
                if (_webrtc != null)
                    _webrtc.Free(_handle);
                _handle = IntPtr.Zero;
            }

            hasDisposed = true;
        }

        /// <summary>
        /// Dispose the underlying WebRTC resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
