
#include "exports.h"

WEBRTC_DECLARE(VadInst*) Vad_Create(void)
{
	return WebRtcVad_Create();
}

WEBRTC_DECLARE(int) Vad_Init(VadInst* handle)
{
	return WebRtcVad_Init(handle);
}

WEBRTC_DECLARE(int) Vad_SetMode(VadInst* self, int mode)
{
	return WebRtcVad_set_mode(self, mode);
}

WEBRTC_DECLARE(int) Vad_ValidRateAndFrameLength(int rate, size_t frame_length)
{
	return WebRtcVad_ValidRateAndFrameLength(rate, frame_length);
}

WEBRTC_DECLARE(int) Vad_Process(VadInst* handle, int fs, const int16_t* audio_frame, size_t frame_length)
{
	return WebRtcVad_Process(handle, fs, audio_frame, frame_length);
}

WEBRTC_DECLARE(void) Vad_Free(VadInst* handle)
{
	WebRtcVad_Free(handle);
}
