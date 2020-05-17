
#ifndef EXPORTS_H_
#define EXPORTS_H_

/** Lib export/import defines (win32) */
#ifdef WEBRTC_WIN
#ifdef WEBRTCVAD_EXPORTS
#define WEBRTC_DECLARE(type)   __declspec(dllexport) type __stdcall
#else
#define WEBRTC_DECLARE(type)   __declspec(dllimport) type __stdcall
#endif
#else
#define WEBRTC_DECLARE(type) type
#endif

#include "webrtc/common_audio/vad/include/webrtc_vad.h"

WEBRTC_DECLARE(VadInst*) Vad_Create(void);
WEBRTC_DECLARE(int) Vad_Init(VadInst* handle);
WEBRTC_DECLARE(int) Vad_SetMode(VadInst* self, int mode);
WEBRTC_DECLARE(int) Vad_ValidRateAndFrameLength(int rate, size_t frame_length);
WEBRTC_DECLARE(int) Vad_Process(VadInst* handle, int fs, const int16_t* audio_frame, size_t frame_length);
WEBRTC_DECLARE(void) Vad_Free(VadInst* handle);

#endif  // EXPORTS_H_
