
WebRtcVadSharp
==

A .NET Standard adapter for the [WebRTC](https://webrtc.org/) voice activity
detection (VAD) component.  The WebRTC VAD uses a
[Gaussian Mixture Model](https://en.wikipedia.org/wiki/Mixture_model)
to detect speech, typically more effectively than the more common energy
threshold model.

See below for a brief overview, or visit the wiki for more
[in-depth documentation](../../wiki).

Installation
--

WebRtcVadSharp is available on NuGet:

```
Install-Package WebRtcVadSharp
```

This will install the .NET Standard adapter (WebRtcVadSharp.dll) and an
unmanaged library (WebRtcVad.dll) containing the supporting WebRTC algorithms.

Usage
--

In the simplest case, you just need to instantiate a
[WebRtcVad](../../wiki/WebRtcVad) object and supply it with a `byte[]` of audio.

```csharp
bool DoesFrameContainSpeech(byte[] audioFrame)
{
  using var vad = new WebRtcVad();
  return vad.HasSpeech(audioFrame, SampleRate.Is8kHz, FrameLength.Is10ms);
}
```

Note that [WebRtcVad](../../wiki/WebRtcVad) implements `IDisposable`, so a `using` block is necessary.

Configuration
--

The underlying VAD code can be configured along three axes:

* [**Frame size**](../../wiki/FrameSize-Enum): 10ms, 20ms and 30ms frames are supported.
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
 