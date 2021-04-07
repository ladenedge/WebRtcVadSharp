
WebRtcVadSharp
==

A .NET Standard adapter for the [WebRTC](https://webrtc.org/) voice activity
detection (VAD) component.  The WebRTC VAD uses a
[Gaussian Mixture Model](https://en.wikipedia.org/wiki/Mixture_model)
to detect speech, typically with better performance than the more common energy
threshold model.

See below for a brief overview, or visit the wiki for more
[in-depth documentation](../../wiki).

[![Build Status](https://travis-ci.org/ladenedge/WebRtcVadSharp.svg?branch=main)](https://travis-ci.org/ladenedge/WebRtcVadSharp)
[![Coverage Status](https://coveralls.io/repos/github/ladenedge/WebRtcVadSharp/badge.svg?branch=main)](https://coveralls.io/github/ladenedge/WebRtcVadSharp?branch=main)
[![NuGet Version](https://img.shields.io/nuget/v/WebRtcVadSharp)](https://www.nuget.org/packages/WebRtcVadSharp)

Installation
--

WebRtcVadSharp is available on [NuGet](https://www.nuget.org/packages/WebRtcVadSharp):

```
Install-Package WebRtcVadSharp
```

This will install the .NET Standard adapter (WebRtcVadSharp.dll) and an
unmanaged library (WebRtcVad.dll) containing the supporting WebRTC algorithms.

Usage
--

In the simplest case, you just need to instantiate a
[WebRtcVad](../../wiki/WebRtcVad) object and supply it with a `byte[]` of raw audio.

```csharp
bool DoesFrameContainSpeech(byte[] audioFrame)
{
  using var vad = new WebRtcVad();
  return vad.HasSpeech(audioFrame, SampleRate.Is8kHz, FrameLength.Is10ms);
}
```

ℹ️ Note that [WebRtcVad](../../wiki/WebRtcVad) implements `IDisposable`, so a `using` block is necessary.

ℹ️ This library (and WebRTC itself) only supports raw, 16-bit linear PCM audio, and will not work with WAV files or other container formats. For hints on converting your audio, see [issue #6](../../issues/6#issuecomment-812570219).

Configuration
--

The underlying VAD code can be configured along three axes:

* [**Frame length**](../../wiki/FrameLength-Enum): 10ms, 20ms and 30ms frames are supported.
* [**Sample rate**](../../wiki/SampleRate-Enum): 8kHz, 16kHz, 32kHz and 48kHz sample rates are supported.
* [**Operating mode**](../../wiki/OperatingMode-Enum): four levels of "aggressiveness" are supported.

These options may be set via properties on the [WebRtcVad](../../wiki/WebRtcVad) object.  More
documentation on each is available in the wiki.

License
--

The code in this repository dual licensed.  The .NET code and the DLL exports
are covered by the [MIT license](https://opensource.org/licenses/MIT), while
the WebRTC code &mdash; imported from the
[WebRTC repository](https://webrtc.googlesource.com/src/) &mdash; is licensed
under Google's [WebRTC license](https://webrtc.org/support/license).
 