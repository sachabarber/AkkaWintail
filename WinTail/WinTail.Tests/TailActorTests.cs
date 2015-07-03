using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.TestKit.NUnit;
using FakeItEasy;
using NUnit.Framework;

namespace WinTail.Tests
{
    [TestFixture]
    public class TailActorTests : TestKit
    {
        [Test]
        public void Show_NotifyTailActor_When_Change_Is_Made_To_File()
        {
            string fullFilePath = @"c:\temp\AkkaTest.txt";
            FileObserver fileObserver = new FileObserver(TestActor, fullFilePath);
            fileObserver.Start();
            File.WriteAllText(fullFilePath, "A test message from TailActorTests" +
                DateTime.Now.ToString());

            ExpectMsg<TailActor.FileWrite>(x => fullFilePath.Contains(x.FileName));
        }
    }
}
