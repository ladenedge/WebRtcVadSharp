using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using System;
using WebRtcVadSharp.WebRtc;

namespace WebRtcVadSharp.Tests
{
    [TestFixture]
    public class WebRtcVadTests
    {
        [Test]
        public void Constructor_ThrowsWithNullLibrary()
        {
            Assert.That(() => new WebRtcVad(null), Throws.ArgumentNullException);
        }

        [Test, AutoMoqData]
        public void Constructor_SetsOperatingModeDefault(WebRtcVad vad)
        {
            Assert.That(vad.OperatingMode, Is.EqualTo(OperatingMode.HighQuality));
        }

        [Test, AutoMoqData]
        public void Constructor_SetsSampleRateDefault(WebRtcVad vad)
        {
            Assert.That(vad.SampleRate, Is.EqualTo(SampleRate.Is8kHz));
        }

        [Test, AutoMoqData]
        public void Constructor_SetsFrameLengthDefault(WebRtcVad vad)
        {
            Assert.That(vad.FrameLength, Is.EqualTo(FrameLength.Is10ms));
        }

        [Test, AutoMoqData]
        public void Constructor_ThrowsOnInitFailure([Frozen] Mock<IWebRtcDll> libraryMock)
        {
            libraryMock.Setup(l => l.Init(It.IsAny<IntPtr>())).Returns(-1);
            Assert.That(() => new WebRtcVad(libraryMock.Object), Throws.InvalidOperationException);
        }

        [Test, AutoMoqData]
        public void Constructor_ThrowsOnInvalidHandle([Frozen] Mock<IWebRtcDll> libraryMock)
        {
            libraryMock.Setup(l => l.Create()).Returns(IntPtr.Zero);
            libraryMock.Setup(l => l.Init(It.IsAny<IntPtr>())).Returns(-1);
            Assert.That(() => new WebRtcVad(libraryMock.Object), Throws.InstanceOf<ObjectDisposedException>());
        }

        [Test, AutoMoqData]
        public void Constructor_InitializesCreatedHandle([Frozen] Mock<IWebRtcDll> libraryMock, [Frozen] IntPtr handle, WebRtcVad _)
        {
            libraryMock.Verify(l => l.Init(handle));
        }

        [Test, AutoMoqData]
        public void SampleRate_CanSetNewValue(SampleRate rate, WebRtcVad vad)
        {
            vad.SampleRate = rate;
            Assert.That(vad.SampleRate, Is.EqualTo(rate));
        }

        [Test, AutoMoqData]
        public void SampleRate_ValidatesNewValue([Frozen] Mock<IWebRtcDll> libraryMock, WebRtcVad vad, SampleRate rate)
        {
            vad.SampleRate = rate;
            var expectedLength = ExpectedFrameLength(rate, vad.FrameLength);
            libraryMock.Verify(l => l.ValidRateAndFrameLength((int)rate, expectedLength));
        }

        [Test, AutoMoqData]
        public void SampleRate_ThrowsOnInvalidRate([Frozen] Mock<IWebRtcDll> libraryMock, WebRtcVad vad, int rate)
        {
            libraryMock.Setup(l => l.ValidRateAndFrameLength(It.IsAny<int>(), It.IsAny<long>())).Returns(-1);
            Assert.That(() => vad.SampleRate = (SampleRate)rate, Throws.ArgumentException.With.Message.Contains(nameof(SampleRate)));
        }

        [Test, AutoMoqData]
        public void FrameLength_CanSetNewValue(FrameLength length, WebRtcVad vad)
        {
            vad.FrameLength = length;
            Assert.That(vad.FrameLength, Is.EqualTo(length));
        }

        [Test, AutoMoqData]
        public void FrameLength_ValidatesNewValue([Frozen] Mock<IWebRtcDll> libraryMock, WebRtcVad vad, FrameLength length)
        {
            vad.FrameLength = length;
            var expectedLength = ExpectedFrameLength(vad.SampleRate, length);
            libraryMock.Verify(l => l.ValidRateAndFrameLength((int)vad.SampleRate, expectedLength));
        }

        [Test, AutoMoqData]
        public void FrameLength_ThrowsOnInvalidLength([Frozen] Mock<IWebRtcDll> libraryMock, WebRtcVad vad, int length)
        {
            libraryMock.Setup(l => l.ValidRateAndFrameLength(It.IsAny<int>(), It.IsAny<long>())).Returns(-1);
            Assert.That(() => vad.FrameLength = (FrameLength)length, Throws.ArgumentException.With.Message.Contains(nameof(FrameLength)));
        }

        [Test, AutoMoqData]
        public void OperatingMode_CanSetNewValue(OperatingMode mode, WebRtcVad vad)
        {
            vad.OperatingMode = mode;
            Assert.That(vad.OperatingMode, Is.EqualTo(mode));
        }

        [Test, AutoMoqData]
        public void OperatingMode_PassesAllParametersToSetMode([Frozen] Mock<IWebRtcDll> libraryMock, [Frozen] IntPtr handle, WebRtcVad vad, OperatingMode mode)
        {
            vad.OperatingMode = mode;
            libraryMock.Verify(l => l.SetMode(handle, (int)mode));
        }

        [Test, AutoMoqData]
        public void OperatingMode_ThrowsOnInvalidHandle([Frozen] Mock<IWebRtcDll> libraryMock, int mode)
        {
            libraryMock.Setup(l => l.Create()).Returns(IntPtr.Zero);
            libraryMock.Setup(l => l.SetMode(It.IsAny<IntPtr>(), It.IsAny<int>())).Returns(-1);
            var vad = new WebRtcVad(libraryMock.Object);
            Assert.That(() => vad.OperatingMode = (OperatingMode)mode, Throws.InstanceOf<ObjectDisposedException>());
        }

        [Test, AutoMoqData]
        public void OperatingMode_ThrowsOnInvalidMode([Frozen] Mock<IWebRtcDll> libraryMock, WebRtcVad vad)
        {
            var invalidMode = 42;
            libraryMock.Setup(l => l.SetMode(It.IsAny<IntPtr>(), It.IsAny<int>())).Returns(-1);
            Assert.That(() => vad.OperatingMode = (OperatingMode)invalidMode, Throws.ArgumentException.With.Message.Contains(nameof(OperatingMode)));
        }

        [Test, AutoMoqData]
        public void OperatingMode_ThrowsOnOtherError([Frozen] Mock<IWebRtcDll> libraryMock, WebRtcVad vad, OperatingMode mode)
        {
            libraryMock.Setup(l => l.SetMode(It.IsAny<IntPtr>(), It.IsAny<int>())).Returns(-1);
            Assert.That(() => vad.OperatingMode = mode, Throws.InvalidOperationException);
        }

        [Test, AutoMoqData]
        public void HasSpeech1_ThrowsOnInvalidHandle([Frozen] Mock<IWebRtcDll> libraryMock, byte[] audio)
        {
            libraryMock.Setup(l => l.Create()).Returns(IntPtr.Zero);
            libraryMock.Setup(l => l.Process(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<long>())).Returns(-1);
            var vad = new WebRtcVad(libraryMock.Object);
            Assert.That(() => vad.HasSpeech(audio), Throws.InstanceOf<ObjectDisposedException>());
        }

        [Test, AutoMoqData]
        public void HasSpeech2_ThrowsOnInvalidHandle([Frozen] Mock<IWebRtcDll> libraryMock, byte[] audio, SampleRate rate, FrameLength length)
        {
            libraryMock.Setup(l => l.Create()).Returns(IntPtr.Zero);
            libraryMock.Setup(l => l.Process(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<long>())).Returns(-1);
            var vad = new WebRtcVad(libraryMock.Object);
            Assert.That(() => vad.HasSpeech(audio, rate, length), Throws.InstanceOf<ObjectDisposedException>());
        }

        [Test, AutoMoqData]
        public void HasSpeech1_ThrowsOnNullAudio(WebRtcVad vad)
        {
            Assert.That(() => vad.HasSpeech(null), Throws.ArgumentNullException);
        }

        [Test, AutoMoqData]
        public void HasSpeech2_ThrowsOnNullAudio(WebRtcVad vad, SampleRate rate, FrameLength length)
        {
            Assert.That(() => vad.HasSpeech(null, rate, length), Throws.ArgumentNullException);
        }

        [Test, AutoMoqData]
        public void HasSpeech1_ThrowsOnTooLittleAudio(WebRtcVad vad)
        {
            var expectedLength = ExpectedFrameLength(vad.SampleRate, vad.FrameLength);
            Assert.That(() => vad.HasSpeech(new byte[expectedLength - 1]), Throws.ArgumentException);
        }

        [Test, AutoMoqData]
        public void HasSpeech2_ThrowsOnTooLittleAudio(WebRtcVad vad, SampleRate rate, FrameLength length)
        {
            var expectedLength = ExpectedFrameLength(vad.SampleRate, vad.FrameLength);
            Assert.That(() => vad.HasSpeech(new byte[expectedLength - 1], rate, length), Throws.ArgumentException);
        }

        [Test, AutoMoqData]
        public void HasSpeech1_ThrowsOnInvalidSampleRate([Frozen] Mock<IWebRtcDll> libraryMock, WebRtcVad vad, byte[] audio)
        {
            vad.SampleRate = (SampleRate)42;
            libraryMock.Setup(l => l.Process(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<long>())).Returns(-1);
            Assert.That(() => vad.HasSpeech(audio), Throws.ArgumentException.With.Message.Contains(nameof(SampleRate)));
        }

        [Test, AutoMoqData]
        public void HasSpeech2_ThrowsOnInvalidSampleRate([Frozen] Mock<IWebRtcDll> libraryMock, WebRtcVad vad, byte[] audio, FrameLength length)
        {
            libraryMock.Setup(l => l.Process(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<long>())).Returns(-1);
            Assert.That(() => vad.HasSpeech(audio, (SampleRate)42, length), Throws.ArgumentException.With.Message.Contains(nameof(SampleRate)));
        }

        [Test, AutoMoqData]
        public void HasSpeech1_ThrowsOnInvalidFrameLength([Frozen] Mock<IWebRtcDll> libraryMock, WebRtcVad vad, byte[] audio)
        {
            vad.FrameLength = (FrameLength)42;
            libraryMock.Setup(l => l.Process(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<long>())).Returns(-1);
            Assert.That(() => vad.HasSpeech(audio), Throws.ArgumentException.With.Message.Contains(nameof(FrameLength)));
        }

        [Test, AutoMoqData]
        public void HasSpeech2_ThrowsOnInvalidFrameLength([Frozen] Mock<IWebRtcDll> libraryMock, WebRtcVad vad, byte[] audio, SampleRate rate)
        {
            libraryMock.Setup(l => l.Process(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<long>())).Returns(-1);
            Assert.That(() => vad.HasSpeech(audio, rate, (FrameLength)12), Throws.ArgumentException.With.Message.Contains(nameof(FrameLength)));
        }

        [Test, AutoMoqData]
        public void HasSpeech1_ThrowsOnOtherError([Frozen] Mock<IWebRtcDll> libraryMock, WebRtcVad vad, byte[] audio)
        {
            libraryMock.Setup(l => l.Process(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<long>())).Returns(-1);
            Assert.That(() => vad.HasSpeech(audio), Throws.InvalidOperationException);
        }

        [Test, AutoMoqData]
        public void HasSpeech2_ThrowsOnOtherError([Frozen] Mock<IWebRtcDll> libraryMock, WebRtcVad vad, byte[] audio, SampleRate rate, FrameLength length)
        {
            libraryMock.Setup(l => l.Process(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<long>())).Returns(-1);
            Assert.That(() => vad.HasSpeech(audio, rate, length), Throws.InvalidOperationException);
        }

        [Test, AutoMoqData]
        public void HasSpeech1_ReturnsResult([Frozen] Mock<IWebRtcDll> libraryMock, WebRtcVad vad, byte[] audio, bool result)
        {
            var returnCode = Convert.ToInt32(result);
            libraryMock.Setup(l => l.Process(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<long>())).Returns(returnCode);
            var hasSpeech = vad.HasSpeech(audio);
            Assert.That(hasSpeech, Is.EqualTo(result));
        }

        [Test, AutoMoqData]
        public void HasSpeech2_ReturnsResult([Frozen] Mock<IWebRtcDll> libraryMock, WebRtcVad vad, byte[] audio, SampleRate rate, FrameLength length, bool result)
        {
            var returnCode = Convert.ToInt32(result);
            libraryMock.Setup(l => l.Process(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<long>())).Returns(returnCode);
            var hasSpeech = vad.HasSpeech(audio, rate, length);
            Assert.That(hasSpeech, Is.EqualTo(result));
        }

        [Test, AutoMoqData]
        public void HasSpeech1_PassesAllParametersToProcess([Frozen] Mock<IWebRtcDll> libraryMock, [Frozen] IntPtr handle, WebRtcVad vad, byte[] audio)
        {
            vad.HasSpeech(audio);
            var expectedLength = ExpectedFrameLength(vad.SampleRate, vad.FrameLength);
            libraryMock.Verify(l => l.Process(handle, (int)vad.SampleRate, audio, expectedLength));
        }

        [Test, AutoMoqData]
        public void HasSpeech2_PassesAllParametersToProcess([Frozen] Mock<IWebRtcDll> libraryMock, [Frozen] IntPtr handle, WebRtcVad vad, byte[] audio, SampleRate rate, FrameLength length)
        {
            vad.HasSpeech(audio, rate, length);
            var expectedLength = ExpectedFrameLength(rate, length);
            libraryMock.Verify(l => l.Process(handle, (int)rate, audio, expectedLength));
        }

        [Test, AutoMoqData]
        public void Dispose_PassesHandleToFree([Frozen] Mock<IWebRtcDll> libraryMock, [Frozen] IntPtr handle, WebRtcVad vad)
        {
            vad.Dispose();
            libraryMock.Verify(l => l.Free(handle));
        }

        [Test, AutoMoqData]
        public void Dispose_OnlyCallsFreeOnce([Frozen] Mock<IWebRtcDll> libraryMock, WebRtcVad vad)
        {
            vad.Dispose();
            vad.Dispose();
            libraryMock.Verify(l => l.Free(It.IsAny<IntPtr>()), Times.Once);
        }

        [Test, AutoMoqData]
        public void Dispose_InvalidatesHandle([Frozen] Mock<IWebRtcDll> libraryMock, WebRtcVad vad, byte[] audio)
        {
            Assume.That(() => vad.HasSpeech(audio), Throws.Nothing);
            vad.Dispose();
            libraryMock.Setup(l => l.Process(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<long>())).Returns(-1);
            Assert.That(() => vad.HasSpeech(audio), Throws.InstanceOf<ObjectDisposedException>());
        }

        private long ExpectedFrameLength(SampleRate rate, FrameLength length)
        {
            // calculate a number of 16-bit samples
            return (int)rate / 1000 * (long)length;
        }
    }
}
