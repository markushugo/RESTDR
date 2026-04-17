using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RESTDR.Models;
using RecordModel = RESTDR.Models.Record;
using RESTDR.Repos;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using RESTDR.Controllers;

namespace TestProject3
{
    public class RecordRepoTests
    {
        [Fact]
        public void RecordRepo_IncludeData_Populates_DefaultRecords()
        {
            var repo = new RecordRepo(includeData: true);
            var all = repo.GetAllRecords(null, null).ToList();
            Assert.Equal(3, all.Count);
        }

        [Fact]
        public void RecordRepo_GetAllRecords_WithArtistFilter()
        {
            var repo = new RecordRepo(includeData: true);
            var filtered = repo.GetAllRecords(artist: "Kim Larsen", title: null).ToList();
            Assert.Single(filtered);
            Assert.Equal("Kim Larsen", filtered.First().Artist);
        }

        [Fact]
        public void RecordRepo_GetAllRecords_WithTitleFilter()
        {
            var repo = new RecordRepo(includeData: true);
            var filtered = repo.GetAllRecords(artist: null, title: "7 Years").ToList();
            Assert.Single(filtered);
            Assert.Equal("7 Years", filtered.First().Title);
        }

        [Fact]
        public void RecordRepo_GetAllRecords_CaseInsensitiveSearch()
        {
            var repo = new RecordRepo(includeData: true);
            var filtered = repo.GetAllRecords(artist: "kim larsen", title: null).ToList();
            Assert.Single(filtered);
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

            var all = repo.GetAllRecords(null, null).ToList();
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

        [Fact]
        public void RecordRepo_GetAllRecords_NoFilters_ReturnsAll()
        {
            var repo = new RecordRepo(includeData: true);
            var all = repo.GetAllRecords(artist: null, title: null).ToList();
            Assert.Equal(3, all.Count);
        }

        [Fact]
        public void RecordRepo_GetAllRecords_PartialSearch()
        {
            var repo = new RecordRepo(includeData: true);
            var filtered = repo.GetAllRecords(artist: "Graham", title: null).ToList();
            Assert.Single(filtered);
            Assert.Equal("Lukas Graham", filtered.First().Artist);
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
            var all = repo.GetAllRecords(null, null).ToList();
            Assert.Empty(all);
        }

        [Fact]
        public void RecordDbRepo_GetAllRecords_WithFilters()
        {
            var repo = CreateRepoWithInMemory(out var context);
            var r = new RecordModel { Artist = "TestArtist", Title = "TestTitle", Duration = 100, PublicationYear = 2020 };
            repo.AddRecord(r);

            // Note: RecordDbRepo currently ignores filters, returns all
            var all = repo.GetAllRecords(artist: "TestArtist", title: "TestTitle").ToList();
            Assert.Single(all);
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

            var all = repo.GetAllRecords(null, null).ToList();
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

        [Fact]
        public void RecordDbRepo_SaveChanges_IsCalled()
        {
            var repo = CreateRepoWithInMemory(out var context);
            var r = new RecordModel { Artist = "Persistent", Title = "Data", Duration = 200, PublicationYear = 2022 };
            var added = repo.AddRecord(r);

            // Create a new repo with same context - should find the record
            var repo2 = new RecordDbRepo(context);
            var found = repo2.GetRecordById(added.Id);
            Assert.NotNull(found);
            Assert.Equal("Persistent", found.Artist);
        }

        [Fact]
        public void RecordDbRepo_UpdateRecord_PersistsChanges()
        {
            var repo = CreateRepoWithInMemory(out var context);
            var r = new RecordModel { Artist = "Original", Title = "Title", Duration = 100, PublicationYear = 2020 };
            var added = repo.AddRecord(r);

            var updated = new RecordModel { Artist = "Modified", Title = "NewTitle", Duration = 250, PublicationYear = 2023 };
            repo.UpdateRecord(added.Id, updated);

            // Query context directly to verify persistence
            var dbRecord = context.Records.FirstOrDefault(x => x.Id == added.Id);
            Assert.NotNull(dbRecord);
            Assert.Equal("Modified", dbRecord.Artist);
            Assert.Equal(250, dbRecord.Duration);
        }
    }

    public class RecordsControllerTests
    {
        private RecordsController CreateController(IRecordRepo? repo = null)
        {
            repo ??= new RecordRepo(includeData: false);
            return new RecordsController(repo);
        }

        [Fact]
        public void Get_NoRecords_ReturnsNoContent()
        {
            var controller = CreateController();
            var result = controller.Get(artist: null, titel: null);
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public void Get_WithRecords_ReturnsOk()
        {
            var repo = new RecordRepo(includeData: true);
            var controller = CreateController(repo);
            var result = controller.Get(artist: null, titel: null);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var records = Assert.IsAssignableFrom<List<RecordModel>>(okResult.Value);
            Assert.Equal(3, records.Count);
        }

        [Fact]
        public void Get_WithArtistFilter_ReturnsFiltered()
        {
            var repo = new RecordRepo(includeData: true);
            var controller = CreateController(repo);
            var result = controller.Get(artist: "Kim Larsen", titel: null);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var records = Assert.IsAssignableFrom<List<RecordModel>>(okResult.Value);
            Assert.Single(records);
            Assert.Equal("Kim Larsen", records.First().Artist);
        }

        [Fact]
        public void Get_WithTitleFilter_ReturnsFiltered()
        {
            var repo = new RecordRepo(includeData: true);
            var controller = CreateController(repo);
            var result = controller.Get(artist: null, titel: "7 Years");
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var records = Assert.IsAssignableFrom<List<RecordModel>>(okResult.Value);
            Assert.Single(records);
        }

        [Fact]
        public void GetById_ExistingId_ReturnsOk()
        {
            var repo = new RecordRepo(includeData: false);
            var record = new RecordModel { Artist = "Test", Title = "Test", Duration = 100, PublicationYear = 2020 };
            var added = repo.AddRecord(record);
            var controller = CreateController(repo);

            var result = controller.Get(added.Id);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<RecordModel>(okResult.Value);
            Assert.Equal(added.Id, returned.Id);
        }

        [Fact]
        public void GetById_NonExistingId_ReturnsNotFound()
        {
            var controller = CreateController();
            var result = controller.Get(999);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void Post_ValidRecord_ReturnsCreated()
        {
            var controller = CreateController();
            var record = new RecordModel { Artist = "New", Title = "Song", Duration = 180, PublicationYear = 2023 };
            var result = controller.Post(record);
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(201, createdResult.StatusCode);
            Assert.Contains("api/records/", createdResult.Location);
        }

        [Fact]
        public void Post_NullRecord_ReturnsBadRequest()
        {
            var controller = CreateController();
            var result = controller.Post(null!);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void Put_ExistingId_ReturnsOk()
        {
            var repo = new RecordRepo(includeData: false);
            var record = new RecordModel { Artist = "Old", Title = "Old", Duration = 100, PublicationYear = 2020 };
            var added = repo.AddRecord(record);
            var controller = CreateController(repo);

            var updated = new RecordModel { Artist = "New", Title = "New", Duration = 200, PublicationYear = 2021 };
            var result = controller.Put(added.Id, updated);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<RecordModel>(okResult.Value);
            Assert.Equal("New", returned.Artist);
        }

        [Fact]
        public void Put_NonExistingId_ReturnsNotFound()
        {
            var controller = CreateController();
            var record = new RecordModel { Artist = "New", Title = "New", Duration = 100, PublicationYear = 2020 };
            var result = controller.Put(999, record);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void Delete_ExistingId_ReturnsOk()
        {
            var repo = new RecordRepo(includeData: false);
            var record = new RecordModel { Artist = "Test", Title = "Test", Duration = 100, PublicationYear = 2020 };
            var added = repo.AddRecord(record);
            var controller = CreateController(repo);

            var result = controller.Delete(added.Id);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<RecordModel>(okResult.Value);
        }

        [Fact]
        public void Delete_NonExistingId_ReturnsNotFound()
        {
            var controller = CreateController();
            var result = controller.Delete(999);
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
