
using System.IO;
using System.Threading;
using CabineParty.UnitCodeApp;
using NUnit.Framework;

namespace UnitCodeAppUnitTests
{
    [TestFixture]
    public class MainWindowsViewModelTest
    {
        [Test]
        public void WatchFileStatus_WhenSessionComplete_ShouldExecuteCallBack()
        {
            var filePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "FileStatus.txt");
            var viewModel = new MainWindowViewModel {PhotoboothStatusFilePath = filePath};

            var isCall = false;
            viewModel.MonitorEnOfSession(() => isCall = true);

            File.WriteAllText(filePath, @"PHOTO1");
            Assert.IsFalse(isCall);
            File.WriteAllText(filePath, @"PHOTO2");
            Assert.IsFalse(isCall);
            File.WriteAllText(filePath, @"PHOTO3");
            Assert.IsFalse(isCall);
            File.WriteAllText(filePath, @"PHOTO4");
            Assert.IsFalse(isCall);
            File.WriteAllText(filePath, @"PREVIEW");
            Assert.IsFalse(isCall);
            File.WriteAllText(filePath, @"SESSION_COMPLETE");
            Thread.Sleep(1000);
            Assert.IsTrue(isCall);
        }
    }
}
