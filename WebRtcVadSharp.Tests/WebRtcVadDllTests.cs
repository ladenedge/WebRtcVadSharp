using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

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
        public void MemoryUsage()
        {
            var allBytes = ReadTestFile("leak-test.raw");
            var buffer = new byte[8 * 2 * 10];

            var before = FindMemoryUsage();

            for (int i = 0; i < 1000; i++)
                ReadAllFrames(allBytes, buffer);

            var after = FindMemoryUsage();
            
            Assert.That(after, Is.EqualTo(before).Within(5).Percent);
        }

        IEnumerable<bool> DetectAllFrames(WebRtcVad vad, string filename)
        {
            var frameSize = (int)vad.SampleRate / 1000 * 2 * (int)vad.FrameLength;
            var buffer = new byte[frameSize];
            using var audio = OpenTestFile(filename);
            for (int i = 0; i < audio.Length - frameSize; i += frameSize)
            {
                audio.Read(buffer, 0, buffer.Length);
                yield return vad.HasSpeech(buffer);
            }
        }

        void ReadAllFrames(byte[] allBytes, byte[] buffer)
        {
            using var vad = new WebRtcVad() { OperatingMode = OperatingMode.VeryAggressive };
            for (int i = 0; i < allBytes.Length - buffer.Length; i += buffer.Length)
            {
                Array.Copy(allBytes, i, buffer, 0, buffer.Length);
                vad.HasSpeech(buffer);
            }
        }

        float FindMemoryUsage()
        {
            using var pc = new PerformanceCounter()
            {
                CategoryName = "Process",
                CounterName = "Working Set - Private",
                InstanceName = Process.GetCurrentProcess().ProcessName,
            };
            return pc.NextValue();
        }

        Stream OpenTestFile(string filename)
        {
            var fullPath = Path.Combine(TestDirectory, filename);
            return File.OpenRead(fullPath);
        }

        byte[] ReadTestFile(string filename)
        {
            var fullPath = Path.Combine(TestDirectory, filename);
            return File.ReadAllBytes(fullPath);
        }

        string TestDirectory => Path.Combine(AssemblyDirectory, "TestData");

        static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
