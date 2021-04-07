
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace WebRtcVadSharp.Tests
{
    [TestFixture]
    public class WebRtcVadDllTests
    {
        [TestCase(OperatingMode.HighQuality, "011110111111111111111111111100")]
        [TestCase(OperatingMode.LowBitrate, "011110111111111111111111111100")]
        [TestCase(OperatingMode.Aggressive, "000000111111111111111111110000")]
        [TestCase(OperatingMode.VeryAggressive, "000000111111111111111100000000")]
        public void OperatingModes(OperatingMode mode, string expectedMap)
        {
            using var vad = new WebRtcVad()
            {
                FrameLength = FrameLength.Is30ms,
                OperatingMode = mode,
            };

            var results = DetectAllFrames(vad, "8k-speech.raw");
            var resultMap = results.Aggregate("", (map, r) => map += (r ? "1" : "0"));

            Assert.That(resultMap.ToString(), Is.EqualTo(expectedMap));
        }

        [TestCase(SampleRate.Is8kHz, "8k-speech.raw", "000000111111111111111111110000")]
        [TestCase(SampleRate.Is16kHz, "16k-speech.raw", "00000000011111111111111111100")]
        [TestCase(SampleRate.Is32kHz, "32k-speech.raw", "0000000111111111111111111111000")]
        [TestCase(SampleRate.Is48kHz, "48k-speech.raw", "00000000001111111111111111100000")]
        public void SampleRates(SampleRate rate, string filename, string expectedMap)
        {
            using var vad = new WebRtcVad()
            {
                OperatingMode = OperatingMode.Aggressive,
                FrameLength = FrameLength.Is30ms,
                SampleRate = rate,
            };

            var results = DetectAllFrames(vad, filename);
            var resultMap = results.Aggregate("", (map, r) => map += (r ? "1" : "0"));
            Console.WriteLine(filename + ": " + resultMap);

            Assert.That(resultMap.ToString(), Is.EqualTo(expectedMap));
        }

        [TestCase(FrameLength.Is10ms, "000000000000000000001111111111111111111111111111111111111111111111111111111111100000000000")]
        [TestCase(FrameLength.Is20ms, "000000000011111111111111111111111111111100000")]
        [TestCase(FrameLength.Is30ms, "000000111111111111111111110000")]
        public void FrameLengths(FrameLength length, string expectedMap)
        {
            using var vad = new WebRtcVad()
            {
                OperatingMode = OperatingMode.Aggressive,
                FrameLength = length
            };

            var results = DetectAllFrames(vad, "8k-speech.raw");
            var resultMap = results.Aggregate("", (map, r) => map += (r ? "1" : "0"));
            Console.WriteLine(resultMap);

            Assert.That(resultMap.ToString(), Is.EqualTo(expectedMap));
        }

        [TestCase(OperatingMode.HighQuality, "000000000000000000000000000000000")]
        [TestCase(OperatingMode.LowBitrate, "000000000000000000000000000000000")]
        [TestCase(OperatingMode.Aggressive, "000000000000000000000000000000000")]
        [TestCase(OperatingMode.VeryAggressive, "000000000000000000000000000000000")]
        public void Silence(OperatingMode mode, string expectedMap)
        {
            using var vad = new WebRtcVad()
            {
                OperatingMode = mode,
                FrameLength = FrameLength.Is30ms,
            };

            var results = DetectAllFrames(vad, "silence.raw");
            var resultMap = results.Aggregate("", (map, r) => map += (r ? "1" : "0"));

            Assert.That(resultMap.ToString(), Is.EqualTo(expectedMap));
        }

        [Test]
        public void FrameTypeShort()
        {
            using var vad = new WebRtcVad()
            {
                FrameLength = FrameLength.Is30ms,
            };

            var results = DetectAllFrames(vad, "8k-speech.raw", buf =>
            {
                short[] shortbuf = new short[buf.Length / 2];
                Buffer.BlockCopy(buf, 0, shortbuf, 0, buf.Length);
                return vad.HasSpeech(shortbuf);
            });

            var resultMap = results.Aggregate("", (map, r) => map += (r ? "1" : "0"));

            Assert.That(resultMap.ToString(), Is.EqualTo("011110111111111111111111111100"));
        }

        [Test]
        public void FrameTypeIntPtr()
        {
            using var vad = new WebRtcVad()
            {
                FrameLength = FrameLength.Is30ms,
            };

            var results = DetectAllFrames(vad, "8k-speech.raw", buf =>
            {
                IntPtr ptrbuf = Marshal.AllocHGlobal(buf.Length);
                Marshal.Copy(buf, 0, ptrbuf, buf.Length);
                var result = vad.HasSpeech(ptrbuf);
                Marshal.FreeHGlobal(ptrbuf);
                return result;
            });

            var resultMap = results.Aggregate("", (map, r) => map += (r ? "1" : "0"));

            Assert.That(resultMap.ToString(), Is.EqualTo("011110111111111111111111111100"));
        }

        static IEnumerable<bool> DetectAllFrames(WebRtcVad vad, string filename)
        {
            return DetectAllFrames(vad, filename, buf => vad.HasSpeech(buf));
        }

        static IEnumerable<bool> DetectAllFrames(WebRtcVad vad, string filename, Func<byte[], bool> hasSpeech)
        {
            var frameSize = (int)vad.SampleRate / 1000 * 2 * (int)vad.FrameLength;
            var buffer = new byte[frameSize];
            using var audio = OpenTestFile(filename);
            for (int i = 0; i < audio.Length - frameSize; i += frameSize)
            {
                audio.Read(buffer, 0, buffer.Length);
                yield return hasSpeech(buffer);
            }
        }

        static Stream OpenTestFile(string filename)
        {
            var fullPath = Path.Combine(TestDirectory, filename);
            return File.OpenRead(fullPath);
        }

        static byte[] ReadTestFile(string filename)
        {
            var fullPath = Path.Combine(TestDirectory, filename);
            return File.ReadAllBytes(fullPath);
        }

        static string TestDirectory => Path.Combine(AssemblyDirectory, "TestData");
        static string AssemblyDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}
