using System;
using System.IO;
using System.Runtime.InteropServices;

namespace WebRtcVadSharp.WebRtc
{
    /// <summary>
    /// P/Invoke interface to the WebRTC DLL.
    /// </summary>
    public class WebRtcDll : IWebRtcDll
    {
        /// <inheritdoc/>
        public IntPtr Create()
        {
            return NativeMethods.Vad_Create();
        }

        /// <inheritdoc/>
        public int Init(IntPtr handle)
        {
            return NativeMethods.Vad_Init(handle);
        }

        /// <inheritdoc/>
        public int Process(IntPtr handle, int fs, byte[] audio_frame, long frame_length)
        {
            return NativeMethods.Vad_Process(handle, fs, audio_frame, frame_length);
        }

        /// <inheritdoc/>
        public int Process(IntPtr handle, int fs, short[] audio_frame, long frame_length)
        {
            return NativeMethods.Vad_Process(handle, fs, audio_frame, frame_length);
        }

        /// <inheritdoc/>
        public int Process(IntPtr handle, int fs, IntPtr audio_frame, long frame_length)
        {
            return NativeMethods.Vad_Process(handle, fs, audio_frame, frame_length);
        }

        /// <inheritdoc/>
        public int SetMode(IntPtr self, int mode)
        {
            return NativeMethods.Vad_SetMode(self, mode);
        }

        /// <inheritdoc/>
        public int ValidRateAndFrameLength(int rate, long frame_length)
        {
            return NativeMethods.Vad_ValidRateAndFrameLength(rate, frame_length);
        }

        /// <inheritdoc/>
        public void Free(IntPtr handle)
        {
            NativeMethods.Vad_Free(handle);
        }

        private class NativeMethods
        {
            [DllImport("WebRtcVad.dll")]
            public static extern IntPtr Vad_Create();

            [DllImport("WebRtcVad.dll")]
            public static extern int Vad_Init(IntPtr handle);

            [DllImport("WebRtcVad.dll")]
            public static extern int Vad_SetMode(IntPtr self, int mode);

            [DllImport("WebRtcVad.dll")]
            public static extern int Vad_ValidRateAndFrameLength(int rate, long frame_length);

            [DllImport("WebRtcVad.dll")]
            public static extern int Vad_Process(IntPtr handle, int fs, byte[] audio_frame, long frame_length);

            [DllImport("WebRtcVad.dll")]
            public static extern int Vad_Process(IntPtr handle, int fs, short[] audio_frame, long frame_length);

            [DllImport("WebRtcVad.dll")]
            public static extern int Vad_Process(IntPtr handle, int fs, IntPtr audio_frame, long frame_length);

            [DllImport("WebRtcVad.dll")]
            public static extern void Vad_Free(IntPtr handle);
        }
    }
}
