namespace WebRtcVadSharp
{
    /// <summary>
    /// Frame length, in ms, of input audio.
    /// </summary>
    public enum FrameLength : long
    {
        /// <summary>
        /// 10ms frame length.
        /// </summary>
        Is10ms = 10,

        /// <summary>
        /// 20ms frame length.
        /// </summary>
        Is20ms = 20,

        /// <summary>
        /// 30ms frame length.
        /// </summary>
        Is30ms = 30,
    }
}
