using System;
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
        unsafe public int Process(IntPtr handle, int fs, byte[] audio_frame, ulong frame_length)
        {
            var sizet_length = new UIntPtr(frame_length);
            fixed (byte* frame_ptr = audio_frame)
            {
                return NativeMethods.Vad_Process(handle, fs, (IntPtr)frame_ptr, sizet_length);
            }
        }

        /// <inheritdoc/>
        unsafe public int Process(IntPtr handle, int fs, short[] audio_frame, ulong frame_length)
        {
            var sizet_length = new UIntPtr(frame_length);
            fixed (short* frame_ptr = audio_frame)
            {
                return NativeMethods.Vad_Process(handle, fs, (IntPtr)frame_ptr, sizet_length);
            }
        }

        /// <inheritdoc/>
        public int Process(IntPtr handle, int fs, IntPtr audio_frame, ulong frame_length)
        {
            var sizet_length = new UIntPtr(frame_length);
            return NativeMethods.Vad_Process(handle, fs, audio_frame, sizet_length);
        }

        /// <inheritdoc/>
        public int SetMode(IntPtr self, int mode)
        {
            return NativeMethods.Vad_SetMode(self, mode);
        }

        /// <inheritdoc/>
        public int ValidRateAndFrameLength(int rate, ulong frame_length)
        {
            var sizet_length = new UIntPtr(frame_length);
            return NativeMethods.Vad_ValidRateAndFrameLength(rate, sizet_length);
        }

        /// <inheritdoc/>
        public void Free(IntPtr handle)
        {
            NativeMethods.Vad_Free(handle);
        }

        private class NativeMethods
        {
            [DllImport("webrtcvad")]
            public static extern IntPtr Vad_Create();

            [DllImport("webrtcvad")]
            public static extern int Vad_Init(IntPtr handle);

            [DllImport("webrtcvad")]
            public static extern int Vad_SetMode(IntPtr self, int mode);

            [DllImport("webrtcvad")]
            public static extern int Vad_ValidRateAndFrameLength(int rate, UIntPtr frame_length);

            [DllImport("webrtcvad")]
            public static extern int Vad_Process(IntPtr handle, int fs, IntPtr audio_frame, UIntPtr frame_length);

            [DllImport("webrtcvad")]
            public static extern void Vad_Free(IntPtr handle);
        }
    }
}
