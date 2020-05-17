using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using AutoFixture.NUnit3;
using Moq;
using System;
using WebRtcVadSharp.WebRtc;

namespace WebRtcVadSharp.Tests
{
    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute() : base(CustomizeFixture) { }

        static IFixture CustomizeFixture()
        {
            var fixture = new Fixture() { RepeatCount = 1000 };
            fixture.Customize(new AutoMoqCustomization());
            
            fixture.Customize<WebRtcVad>(c => c.FromFactory(new MethodInvoker(new GreedyConstructorQuery())));
            fixture.Customize<IntPtr>(c => c.FromFactory((int ptr) => new IntPtr(ptr)));

            var defaultPointer = fixture.Freeze<IntPtr>();
            fixture.Customize<Mock<IWebRtcVadDll>>(c => c.Do(m => m.Setup(l => l.Create()).Returns(defaultPointer)));
            
            return fixture;
        }
    }
}
