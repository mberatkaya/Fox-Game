using System.IO;
using NUnit.Framework;

namespace TilkiOyunu.Foundation.PlayModeTests
{
    // Existing Bootstrap tests use the default SaveService. Preserve its bytes across the suite.
    [SetUpFixture]
    public sealed class ProductionSaveProtection
    {
        private string path;
        private string backup;
        private bool existed;

        [OneTimeSetUp]
        public void Preserve()
        {
            path = new SaveService().SavePath;
            backup = path + ".playmode-backup";
            Assert.That(File.Exists(backup), Is.False,
                "A previous test run was interrupted. Restore the .playmode-backup before running tests.");
            existed = File.Exists(path);
            if (existed) File.Copy(path, backup);
        }

        [OneTimeTearDown]
        public void Restore()
        {
            if (existed && File.Exists(backup))
            {
                File.Copy(backup, path, true);
                File.Delete(backup);
            }
            else if (!existed && path != null && !File.Exists(backup) && File.Exists(path)) File.Delete(path);
        }
    }
}
