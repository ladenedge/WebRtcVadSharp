namespace WebRtcVadSharp
{
    /// <summary>
    /// The VAD operating mode.  The default is <see cref="HighQuality"/>.
    /// </summary>
    /// <remarks>
    /// A more aggressive (higher mode) VAD is more restrictive in reporting speech.
    /// 
    /// In other words: the probability of "true" results detecting actual speech
    /// increases with increasing mode. However, as aggressiveness goes up, so too
    /// does the missed detection rate.
    /// 
    /// In still other words:
    ///  - False positives (non-speech) are more common at mode 0.
    ///  - False negatives (missed speech) are more common at mode 3.
    ///  
    /// Note: these names come from the WebRTC source (vad_core.c).
    /// </remarks>
    public enum OperatingMode
    {
        /// <summary>
        /// Least aggressive speech detection for high-quality audio.
        /// </summary>
        HighQuality = 0,

        /// <summary>
        /// More aggressive speech detection for low bitrate audio.
        /// </summary>
        LowBitrate = 1,

        /// <summary>
        /// Aggressive speech detection for moderately noisy audio.
        /// </summary>
        Aggressive = 2,

        /// <summary>
        /// Highly aggressive speech detection for very noisy audio.
        /// </summary>
        VeryAggressive = 3,
    }
}
