using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RESTDanmarksRadio.Models;
using RecordModel = RESTDanmarksRadio.Models.Record;
using RESTDanmarksRadio.Repos;
using Xunit;

namespace TestProject3
{
    public class RecordRepoTests
    {
        [Fact]
        public void RecordRepo_IncludeData_Populates_DefaultRecords()
        {
            var repo = new RecordRepo(includeData: true);
            var all = repo.GetAllRecords().ToList();
            Assert.Equal(3, all.Count);
        }

        [Fact]
        public void RecordRepo_AddRecord_Null_Throws()
        {
            var repo = new RecordRepo(includeData: false);
            Assert.Throws<ArgumentNullException>(() => repo.AddRecord(null!));
        }

        [Fact]
        public void RecordRepo_AddGetUpdateDelete_Workflow()
        {
            var repo = new RecordRepo(includeData: false);

            var r = new RecordModel { Artist = "A", Title = "T", Duration = 100, PublicationYear = 2000 };
            var added = repo.AddRecord(r);
            Assert.Equal(1, added.Id);

            var all = repo.GetAllRecords().ToList();
            Assert.Single(all);

            var fetched = repo.GetRecordById(added.Id);
            Assert.NotNull(fetched);
            Assert.Equal("A", fetched.Artist);

            var updated = new RecordModel { Artist = "B", Title = "T2", Duration = 150, PublicationYear = 2001 };
            var result = repo.UpdateRecord(added.Id, updated);
            Assert.NotNull(result);
            Assert.Equal("B", result.Artist);

            var notFoundUpdate = repo.UpdateRecord(999, updated);
            Assert.Null(notFoundUpdate);

            var deleted = repo.DeleteRecord(added.Id);
            Assert.NotNull(deleted);

            var deletedAgain = repo.DeleteRecord(added.Id);
            Assert.Null(deletedAgain);
        }
    }

    public class RecordDbRepoTests
    {
        private static RecordDbRepo CreateRepoWithInMemory(out RecordDbContext context)
        {
            var options = new DbContextOptionsBuilder<RecordDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            context = new RecordDbContext(options);
            return new RecordDbRepo(context);
        }

        [Fact]
        public void RecordDbRepo_GetAllRecords_Empty_ReturnsNone()
        {
            var repo = CreateRepoWithInMemory(out var context);
            var all = repo.GetAllRecords().ToList();
            Assert.Empty(all);
        }

        [Fact]
        public void RecordDbRepo_AddRecord_Null_Throws()
        {
            var repo = CreateRepoWithInMemory(out _);
            Assert.Throws<ArgumentNullException>(() => repo.AddRecord(null!));
        }

        [Fact]
        public void RecordDbRepo_AddGetUpdateDelete_Workflow()
        {
            var repo = CreateRepoWithInMemory(out var context);

            var r = new RecordModel { Artist = "X", Title = "Y", Duration = 120, PublicationYear = 2010 };
            var added = repo.AddRecord(r);
            Assert.True(added.Id > 0);

            var all = repo.GetAllRecords().ToList();
            Assert.Single(all);

            var fetched = repo.GetRecordById(added.Id);
            Assert.NotNull(fetched);
            Assert.Equal("X", fetched.Artist);

            var updated = new RecordModel { Artist = "Z", Title = "New", Duration = 130, PublicationYear = 2011 };
            var result = repo.UpdateRecord(added.Id, updated);
            Assert.NotNull(result);
            Assert.Equal("Z", result.Artist);

            var notFoundUpdate = repo.UpdateRecord(9999, updated);
            Assert.Null(notFoundUpdate);

            var deleted = repo.DeleteRecord(added.Id);
            Assert.NotNull(deleted);

            var deletedAgain = repo.DeleteRecord(added.Id);
            Assert.Null(deletedAgain);
        }
    }
}
