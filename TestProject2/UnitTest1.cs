using System;
using System.Linq;
using RecordModel = RESTDanmarksRadio.Models.Record;
using RESTDanmarksRadio.Repos;
using Xunit;

namespace TestProject2
{
    public class RecordRepoTests
    {
        [Fact]
        public void AddRecord_Null_ThrowsArgumentNullException()
        {
            var repo = new RecordRepo(false);
            Assert.Throws<ArgumentNullException>(() => repo.AddRecord(null!));
        }

        [Fact]
        public void AddRecord_AssignsIdsAndAdds()
        {
            var repo = new RecordRepo(false);
            var r1 = repo.AddRecord(new RecordModel { Artist = "A", Title = "T1", Duration = 100, PublicationYear = 2000 });
            var r2 = repo.AddRecord(new RecordModel { Artist = "B", Title = "T2", Duration = 200, PublicationYear = 2001 });

            Assert.Equal(1, r1.Id);
            Assert.Equal(2, r2.Id);
            var all = repo.GetAllRecords().ToList();
            Assert.Equal(2, all.Count);
            Assert.Contains(all, r => r.Id == 1 && r.Title == "T1");
            Assert.Contains(all, r => r.Id == 2 && r.Title == "T2");
        }

        [Fact]
        public void Constructor_WithIncludeData_PopulatesThreeRecords()
        {
            var repo = new RecordRepo(true);
            var all = repo.GetAllRecords().ToList();
            Assert.Equal(3, all.Count);
            Assert.Contains(all, r => r.Artist.Contains("Kim Larsen"));
            Assert.Contains(all, r => r.Artist.Contains("Nephew"));
            Assert.Contains(all, r => r.Artist.Contains("Lukas Graham"));
        }

        [Fact]
        public void DeleteRecord_RemovesAndReturns_WhenExists()
        {
            var repo = new RecordRepo(false);
            var added = repo.AddRecord(new RecordModel { Artist = "Del", Title = "ToDelete", Duration = 50, PublicationYear = 1990 });
            var deleted = repo.DeleteRecord(added.Id);
            Assert.NotNull(deleted);
            Assert.Equal(added.Id, deleted!.Id);
            Assert.Empty(repo.GetAllRecords());
        }

        [Fact]
        public void DeleteRecord_ReturnsNull_WhenNotFound()
        {
            var repo = new RecordRepo(false);
            var result = repo.DeleteRecord(999);
            Assert.Null(result);
        }

        [Fact]
        public void GetRecordById_ReturnsCorrectRecord()
        {
            var repo = new RecordRepo(false);
            var a = repo.AddRecord(new RecordModel { Artist = "X", Title = "One", Duration = 10, PublicationYear = 2000 });
            var b = repo.AddRecord(new RecordModel { Artist = "Y", Title = "Two", Duration = 20, PublicationYear = 2001 });

            var gotA = repo.GetRecordById(a.Id);
            var gotB = repo.GetRecordById(b.Id);

            Assert.Equal(a.Title, gotA?.Title);
            Assert.Equal(b.Title, gotB?.Title);
        }

        [Fact]
        public void UpdateRecord_UpdatesPropertiesAndReturns_WhenExists()
        {
            var repo = new RecordRepo(false);
            var added = repo.AddRecord(new RecordModel { Artist = "Old", Title = "OldT", Duration = 30, PublicationYear = 1980 });

            var updated = repo.UpdateRecord(added.Id, new RecordModel { Artist = "New", Title = "NewT", Duration = 99, PublicationYear = 2020 });

            Assert.NotNull(updated);
            Assert.Equal(added.Id, updated!.Id);
            Assert.Equal("New", updated.Artist);
            Assert.Equal("NewT", updated.Title);
            Assert.Equal(99, updated.Duration);
            Assert.Equal(2020, updated.PublicationYear);
        }

        [Fact]
        public void UpdateRecord_ReturnsNull_WhenNotFound()
        {
            var repo = new RecordRepo(false);
            var result = repo.UpdateRecord(999, new RecordModel { Artist = "N", Title = "T", Duration = 1, PublicationYear = 1 });
            Assert.Null(result);
        }
    }
}
