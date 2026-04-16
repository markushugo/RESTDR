using Xunit;
using RESTDanmarksRadio.Models;
using RESTDanmarksRadio.Repos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestProject1
{
    public class RecordRepoTests
    {
        [Fact]
        public void Constructor_WithoutData_InitializesEmpty()
        {
            var repo = new RecordRepo(includeData: false);
            var records = repo.GetAllRecords().ToList();
            Assert.Empty(records);
        }

        [Fact]
        public void Constructor_WithData_LoadsDefaultRecords()
        {
            var repo = new RecordRepo(includeData: true);
            var records = repo.GetAllRecords().ToList();
            Assert.Equal(3, records.Count);
        }

        [Fact]
        public void GetAllRecords_ReturnsAllRecords()
        {
            var repo = new RecordRepo(includeData: false);
            var record1 = new Record { Artist = "Artist1", Title = "Title1", Duration = 180, PublicationYear = 2020 };
            var record2 = new Record { Artist = "Artist2", Title = "Title2", Duration = 200, PublicationYear = 2021 };
            repo.AddRecord(record1);
            repo.AddRecord(record2);

            var result = repo.GetAllRecords().ToList();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetAllRecords_ReturnsEmptyList_WhenNoRecords()
        {
            var repo = new RecordRepo(includeData: false);
            var result = repo.GetAllRecords().ToList();
            Assert.Empty(result);
        }

        [Fact]
        public void GetRecordById_ReturnsRecord_WhenIdExists()
        {
            var repo = new RecordRepo(includeData: false);
            var record = new Record { Artist = "TestArtist", Title = "TestTitle", Duration = 180, PublicationYear = 2020 };
            var addedRecord = repo.AddRecord(record);

            var result = repo.GetRecordById(addedRecord.Id);

            Assert.NotNull(result);
            Assert.Equal("TestArtist", result.Artist);
            Assert.Equal("TestTitle", result.Title);
        }

        [Fact]
        public void GetRecordById_ReturnsNull_WhenIdNotFound()
        {
            var repo = new RecordRepo(includeData: false);
            var result = repo.GetRecordById(999);
            Assert.Null(result);
        }

        [Fact]
        public void AddRecord_AddsRecordAndAssignsId()
        {
            var repo = new RecordRepo(includeData: false);
            var newRecord = new Record { Artist = "NewArtist", Title = "NewTitle", Duration = 220, PublicationYear = 2022 };

            var result = repo.AddRecord(newRecord);

            Assert.NotNull(result);
            Assert.NotEqual(0, result.Id);
            Assert.Single(repo.GetAllRecords());
        }

        [Fact]
        public void AddRecord_IncrementsIdForEachRecord()
        {
            var repo = new RecordRepo(includeData: false);
            var record1 = new Record { Artist = "Artist1", Title = "Title1", Duration = 180, PublicationYear = 2020 };
            var record2 = new Record { Artist = "Artist2", Title = "Title2", Duration = 200, PublicationYear = 2021 };

            var addedRecord1 = repo.AddRecord(record1);
            var addedRecord2 = repo.AddRecord(record2);

            Assert.Equal(1, addedRecord1.Id);
            Assert.Equal(2, addedRecord2.Id);
            Assert.NotEqual(addedRecord1.Id, addedRecord2.Id);
        }

        [Fact]
        public void AddRecord_ThrowsException_WhenRecordIsNull()
        {
            var repo = new RecordRepo(includeData: false);
            Assert.Throws<ArgumentNullException>(() => repo.AddRecord(null));
        }

        [Fact]
        public void UpdateRecord_UpdatesExistingRecord()
        {
            var repo = new RecordRepo(includeData: false);
            var record = new Record { Artist = "OldArtist", Title = "OldTitle", Duration = 180, PublicationYear = 2020 };
            var addedRecord = repo.AddRecord(record);

            var updatedRecord = new Record { Artist = "NewArtist", Title = "NewTitle", Duration = 200, PublicationYear = 2021 };
            var result = repo.UpdateRecord(addedRecord.Id, updatedRecord);

            Assert.NotNull(result);
            Assert.Equal("NewArtist", result.Artist);
            Assert.Equal("NewTitle", result.Title);
            Assert.Equal(200, result.Duration);
            Assert.Equal(2021, result.PublicationYear);
        }

        [Fact]
        public void UpdateRecord_PreservesId_WhenUpdating()
        {
            var repo = new RecordRepo(includeData: false);
            var record = new Record { Artist = "OldArtist", Title = "OldTitle", Duration = 180, PublicationYear = 2020 };
            var addedRecord = repo.AddRecord(record);
            var originalId = addedRecord.Id;

            var updatedRecord = new Record { Artist = "NewArtist", Title = "NewTitle", Duration = 200, PublicationYear = 2021 };
            var result = repo.UpdateRecord(originalId, updatedRecord);

            Assert.Equal(originalId, result.Id);
        }

        [Fact]
        public void UpdateRecord_ReturnsNull_WhenRecordNotFound()
        {
            var repo = new RecordRepo(includeData: false);
            var updatedRecord = new Record { Artist = "NewArtist", Title = "NewTitle", Duration = 200, PublicationYear = 2021 };

            var result = repo.UpdateRecord(999, updatedRecord);

            Assert.Null(result);
        }

        [Fact]
        public void DeleteRecord_DeletesExistingRecord()
        {
            var repo = new RecordRepo(includeData: false);
            var record = new Record { Artist = "TestArtist", Title = "TestTitle", Duration = 180, PublicationYear = 2020 };
            var addedRecord = repo.AddRecord(record);

            var result = repo.DeleteRecord(addedRecord.Id);

            Assert.NotNull(result);
            Assert.Empty(repo.GetAllRecords());
        }

        [Fact]
        public void DeleteRecord_ReturnsDeletedRecord()
        {
            var repo = new RecordRepo(includeData: false);
            var record = new Record { Artist = "TestArtist", Title = "TestTitle", Duration = 180, PublicationYear = 2020 };
            var addedRecord = repo.AddRecord(record);

            var result = repo.DeleteRecord(addedRecord.Id);

            Assert.NotNull(result);
            Assert.Equal("TestArtist", result.Artist);
            Assert.Equal(addedRecord.Id, result.Id);
        }

        [Fact]
        public void DeleteRecord_ReturnsNull_WhenRecordNotFound()
        {
            var repo = new RecordRepo(includeData: false);
            var result = repo.DeleteRecord(999);
            Assert.Null(result);
        }

        [Fact]
        public void DefaultRecords_AreLoadedCorrectly()
        {
            var repo = new RecordRepo(includeData: true);
            var records = repo.GetAllRecords().ToList();

            var kimLarsen = records.FirstOrDefault(r => r.Artist == "Kim Larsen");
            var nephew = records.FirstOrDefault(r => r.Artist == "Nephew");
            var lukasGraham = records.FirstOrDefault(r => r.Artist == "Lukas Graham");

            Assert.NotNull(kimLarsen);
            Assert.NotNull(nephew);
            Assert.NotNull(lukasGraham);

            Assert.Equal("Midt Om Natten", kimLarsen.Title);
            Assert.Equal("007 Is Also Gonna Die", nephew.Title);
            Assert.Equal("7 Years", lukasGraham.Title);
        }

        [Fact]
        public void GetAllRecords_ReturnsReadOnlyCollection()
        {
            var repo = new RecordRepo(includeData: false);
            var record = new Record { Artist = "Artist1", Title = "Title1", Duration = 180, PublicationYear = 2020 };
            repo.AddRecord(record);

            var result = repo.GetAllRecords();

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<Record>>(result);
        }

        [Fact]
        public void MultipleOperations_WorkCorrectly()
        {
            var repo = new RecordRepo(includeData: false);

            var record1 = new Record { Artist = "Artist1", Title = "Title1", Duration = 180, PublicationYear = 2020 };
            var record2 = new Record { Artist = "Artist2", Title = "Title2", Duration = 200, PublicationYear = 2021 };
            var record3 = new Record { Artist = "Artist3", Title = "Title3", Duration = 190, PublicationYear = 2019 };

            var added1 = repo.AddRecord(record1);
            var added2 = repo.AddRecord(record2);
            var added3 = repo.AddRecord(record3);

            Assert.Equal(3, repo.GetAllRecords().Count());

            repo.DeleteRecord(added2.Id);
            Assert.Equal(2, repo.GetAllRecords().Count());

            var updated = repo.UpdateRecord(added1.Id, new Record { Artist = "UpdatedArtist", Title = "UpdatedTitle", Duration = 250, PublicationYear = 2023 });
            Assert.Equal("UpdatedArtist", updated.Artist);
            Assert.Equal(2, repo.GetAllRecords().Count());
        }
    }

    public class RecordModelTests
    {
        [Fact]
        public void CanBeCreated()
        {
            var record = new Record
            {
                Id = 1,
                Artist = "TestArtist",
                Title = "TestTitle",
                Duration = 180,
                PublicationYear = 2020
            };
            Assert.Equal(1, record.Id);
            Assert.Equal("TestArtist", record.Artist);
            Assert.Equal("TestTitle", record.Title);
            Assert.Equal(180, record.Duration);
            Assert.Equal(2020, record.PublicationYear);
        }

        [Fact]
        public void PropertiesCanBeModified()
        {
            var record = new Record { Artist = "OldArtist", Title = "OldTitle" };
            record.Artist = "NewArtist";
            record.Title = "NewTitle";
            Assert.Equal("NewArtist", record.Artist);
            Assert.Equal("NewTitle", record.Title);
        }

        [Fact]
        public void AllPropertiesCanBeSet()
        {
            var record = new Record();
            record.Id = 5;
            record.Artist = "TestArtist";
            record.Title = "TestTitle";
            record.Duration = 300;
            record.PublicationYear = 2023;
            Assert.Equal(5, record.Id);
            Assert.Equal("TestArtist", record.Artist);
            Assert.Equal("TestTitle", record.Title);
            Assert.Equal(300, record.Duration);
            Assert.Equal(2023, record.PublicationYear);
        }
    }
}
