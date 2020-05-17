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
        /// <returns><b>true</b> if the provided frame contains speech, otherwise <b>false</b>.</returns>
        bool HasSpeech(byte[] audioFrame);

        /// <summary>
        /// Test whether the supplied frame contains speech.
        /// </summary>
        /// <param name="audioFrame">Single frame of audio.</param>
        /// <param name="sampleRate">The sample rate used to encode <paramref name="audioFrame"/>.</param>
        /// <param name="frameLength">The length of the frame in <paramref name="audioFrame"/>.</param>
        /// <returns><b>true</b> if the provided frame contains speech, otherwise <b>false</b>.</returns>
        bool HasSpeech(byte[] audioFrame, SampleRate sampleRate, FrameLength frameLength);
    }
}