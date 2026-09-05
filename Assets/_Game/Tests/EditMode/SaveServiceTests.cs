using System;
using System.IO;
using NUnit.Framework;
using TilkiOyunu.Foundation;

namespace TilkiOyunu.Foundation.Tests
{
    public sealed class SaveServiceTests
    {
        private string tempDirectory;
        private string savePath;
        private SaveService service;

        [SetUp]
        public void SetUp()
        {
            tempDirectory = Path.Combine(Path.GetTempPath(), "TilkiOyunuTests", Guid.NewGuid().ToString("N"));
            savePath = Path.Combine(tempDirectory, SaveService.DefaultFileName);
            service = new SaveService(savePath);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Test]
        public void NewSaveSerializesToDisk()
        {
            SaveGameData data = service.CreateNewSave();

            bool saved = service.Save(data);

            Assert.That(saved, Is.True);
            Assert.That(File.Exists(savePath), Is.True);
            Assert.That(File.ReadAllText(savePath), Does.Contain("\"saveVersion\""));
        }

        [Test]
        public void SavedDataLoadsBack()
        {
            SaveGameData data = service.CreateNewSave();
            data.gameState = GameState.FinalAvailable;
            data.collectedMemoryIds.Add("memory_01");

            service.Save(data);
            SaveGameData loaded = service.Load();

            Assert.That(loaded.gameState, Is.EqualTo(GameState.FinalAvailable));
            Assert.That(loaded.collectedMemoryIds, Does.Contain("memory_01"));
        }

        [Test]
        public void MissingSaveReturnsFreshData()
        {
            SaveGameData loaded = service.Load();

            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded.saveVersion, Is.EqualTo(SaveService.CurrentSaveVersion));
            Assert.That(loaded.gameState, Is.EqualTo(GameState.NotStarted));
        }

        [Test]
        public void DeleteSaveRemovesFile()
        {
            service.Save(service.CreateNewSave());

            bool deleted = service.DeleteSave();

            Assert.That(deleted, Is.True);
            Assert.That(File.Exists(savePath), Is.False);
        }

        [Test]
        public void CorruptSaveReturnsFreshData()
        {
            Directory.CreateDirectory(tempDirectory);
            File.WriteAllText(savePath, "{not valid json");

            SaveGameData loaded = service.Load();

            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded.saveVersion, Is.EqualTo(SaveService.CurrentSaveVersion));
        }
    }
}
