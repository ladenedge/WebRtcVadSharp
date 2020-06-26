
using System;

namespace WebRtcVadSharp
{
    /// <summary>
    /// Voice activity detector interface to ease unit testing.
    /// </summary>
    public interface IWebRtcVad
    {
        /// <summary>
        /// Gets or sets the sample rate when using the audio-only <see cref="HasSpeech(byte[])"/> overload.
        /// </summary>
        SampleRate SampleRate { get; set; }

        /// <summary>
        /// Gets or sets the frame length when using the audio-only <see cref="HasSpeech(byte[])"/> overload.
        /// </summary>
        FrameLength FrameLength { get; set; }

        /// <summary>
        /// Gets or sets the aggressiveness of the detection.
        /// </summary>
        /// <seealso cref="OperatingMode"/>
        OperatingMode OperatingMode { get; set; }

        /// <summary>
        /// Test whether the supplied frame contains speech.
        /// </summary>
        /// <param name="audioFrame">Single frame of audio.</param>
        /// <remarks>
        /// The supplied frame must be encoded according to <see cref="SampleRate"/> and <see cref="FrameLength"/>.
        /// To test a frame at other rates or length, either re-set the appropriate properties, or use the
        /// standalone overload <see cref="HasSpeech(byte[], SampleRate, FrameLength)"/>.
        /// </remarks>
        /// <returns><b>true</b> if the provided frame contains speech, otherwise <b>false</b>.</returns>
        bool HasSpeech(byte[] audioFrame);

        /// <summary>
        /// Test whether the supplied frame contains speech.
        /// </summary>
        /// <param name="audioFrame">Single frame of audio.</param>
        /// <param name="sampleRate">The sample rate used to encode <paramref name="audioFrame"/>.</param>
        /// <param name="frameLength">The length of the frame in <paramref name="audioFrame"/>.</param>
        /// <remarks>
        /// This overload ignores the <see cref="SampleRate"/> and <see cref="FrameLength"/> properties.
        /// To avoid passing the rate and length on each call, use the <see cref="HasSpeech(byte[])"/>
        /// overload instead.
        /// </remarks>
        /// <returns><b>true</b> if the provided frame contains speech, otherwise <b>false</b>.</returns>
        bool HasSpeech(byte[] audioFrame, SampleRate sampleRate, FrameLength frameLength);

        /// <summary>
        /// Test whether the supplied frame contains speech.
        /// </summary>
        /// <param name="audioFrame">Single frame of audio.</param>
        /// <remarks>
        /// The supplied frame must be encoded according to <see cref="SampleRate"/> and <see cref="FrameLength"/>.
        /// To test a frame at other rates or length, either re-set the appropriate properties, or use the
        /// standalone overload <see cref="HasSpeech(short[], SampleRate, FrameLength)"/>.
        /// </remarks>
        /// <returns><b>true</b> if the provided frame contains speech, otherwise <b>false</b>.</returns>
        bool HasSpeech(short[] audioFrame);

        /// <summary>
        /// Test whether the supplied frame contains speech.
        /// </summary>
        /// <param name="audioFrame">Single frame of audio.</param>
        /// <param name="sampleRate">The sample rate used to encode <paramref name="audioFrame"/>.</param>
        /// <param name="frameLength">The length of the frame in <paramref name="audioFrame"/>.</param>
        /// <remarks>
        /// This overload ignores the <see cref="SampleRate"/> and <see cref="FrameLength"/> properties.
        /// To avoid passing the rate and length on each call, use the <see cref="HasSpeech(short[])"/>
        /// overload instead.
        /// </remarks>
        /// <returns><b>true</b> if the provided frame contains speech, otherwise <b>false</b>.</returns>
        bool HasSpeech(short[] audioFrame, SampleRate sampleRate, FrameLength frameLength);

        /// <summary>
        /// Test whether the supplied frame contains speech.
        /// </summary>
        /// <param name="audioFrame">Single frame of audio.</param>
        /// <remarks>
        /// The supplied frame must be encoded according to <see cref="SampleRate"/> and <see cref="FrameLength"/>.
        /// To test a frame at other rates or length, either re-set the appropriate properties, or use the
        /// standalone overload <see cref="HasSpeech(IntPtr, SampleRate, FrameLength)"/>.
        /// </remarks>
        /// <returns><b>true</b> if the provided frame contains speech, otherwise <b>false</b>.</returns>
        bool HasSpeech(IntPtr audioFrame);

        /// <summary>
        /// Test whether the supplied frame contains speech.
        /// </summary>
        /// <param name="audioFrame">Single frame of audio.</param>
        /// <param name="sampleRate">The sample rate used to encode <paramref name="audioFrame"/>.</param>
        /// <param name="frameLength">The length of the frame in <paramref name="audioFrame"/>.</param>
        /// <remarks>
        /// This overload ignores the <see cref="SampleRate"/> and <see cref="FrameLength"/> properties.
        /// To avoid passing the rate and length on each call, use the <see cref="HasSpeech(IntPtr)"/>
        /// overload instead.
        /// </remarks>
        /// <returns><b>true</b> if the provided frame contains speech, otherwise <b>false</b>.</returns>
        bool HasSpeech(IntPtr audioFrame, SampleRate sampleRate, FrameLength frameLength);
    }
}
