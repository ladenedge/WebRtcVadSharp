using System;

namespace WebRtcVadSharp.WebRtc
{
    /// <summary>
    /// Interface for the underlying WebRTC DLL.
    /// </summary>
    public interface IWebRtcDll
    {
        /// <summary>
        /// Creates an empty VAD context.
        /// </summary>
        /// <returns>A handle to a new VAD context.</returns>
        IntPtr Create();

        /// <summary>
        /// Initializes a VAD context.
        /// </summary>
        /// <param name="handle">Handle to the fresh VAD context.</param>
        /// <returns>0 if the initialization is successful, otherwise -1.</returns>
        int Init(IntPtr handle);

        /// <summary>
        /// Sets the aggressiveness mode for the supplied context.
        /// </summary>
        /// <param name="self">Handle to the VAD context to update.</param>
        /// <param name="mode">New aggressiveness mode.</param>
        /// <returns>0 if the mode change is successful, otherwise -1.</returns>
        int SetMode(IntPtr self, int mode);

        /// <summary>
        /// Tests a sample rate/frame length combination for validity.
        /// </summary>
        /// <param name="rate">The sample rate to test.</param>
        /// <param name="frame_length">The frame length to test.</param>
        /// <returns>0 if the combination is valid, otherwise -1.</returns>
        int ValidRateAndFrameLength(int rate, long frame_length);

        /// <summary>
        /// Process a frame of audio for voice content.
        /// </summary>
        /// <param name="handle">Handle to an initialized VAD context.</param>
        /// <param name="fs">The sample rate of <paramref name="audio_frame"/>.</param>
        /// <param name="audio_frame">Frame of audio to test for speech.</param>
        /// <param name="frame_length">Length of the frame, in 16-bit samples.</param>
        /// <returns>1 if the sample contains speech, 0 if no speech is found, and -1 on error.</returns>
        int Process(IntPtr handle, int fs, byte[] audio_frame, long frame_length);

        /// <summary>
        /// Process a frame of audio for voice content.
        /// </summary>
        /// <param name="handle">Handle to an initialized VAD context.</param>
        /// <param name="fs">The sample rate of <paramref name="audio_frame"/>.</param>
        /// <param name="audio_frame">Frame of audio to test for speech.</param>
        /// <param name="frame_length">Length of the frame, in 16-bit samples.</param>
        /// <returns>1 if the sample contains speech, 0 if no speech is found, and -1 on error.</returns>
        int Process(IntPtr handle, int fs, short[] audio_frame, long frame_length);

        /// <summary>
        /// Process a frame of audio for voice content.
        /// </summary>
        /// <param name="handle">Handle to an initialized VAD context.</param>
        /// <param name="fs">The sample rate of <paramref name="audio_frame"/>.</param>
        /// <param name="audio_frame">Frame of audio to test for speech.</param>
        /// <param name="frame_length">Length of the frame, in 16-bit samples.</param>
        /// <returns>1 if the sample contains speech, 0 if no speech is found, and -1 on error.</returns>
        int Process(IntPtr handle, int fs, IntPtr audio_frame, long frame_length);

        /// <summary>
        /// Free the resources associated with the supplied handle.
        /// </summary>
        /// <param name="handle">Handle of the VAD context to free.</param>
        void Free(IntPtr handle);
    }
}
