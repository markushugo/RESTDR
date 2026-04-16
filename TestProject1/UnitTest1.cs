using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ModelRecord = RESTDanmarksRadio.Models.Record;
using RESTDanmarksRadio.Repos;
using RESTDanmarksRadio.Controllers;
using Xunit;

namespace TestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void RecordRepo_Add_Get_Update_Delete_Workflow()
        {
            var repo = new RecordRepo(false);

            // initially empty
            Assert.Empty(repo.GetAllRecords());

            // add
            var rec = new ModelRecord { Artist = "A", Title = "T", Duration = 100, PublicationYear = 2000 };
            var added = repo.AddRecord(rec);
            Assert.Equal(1, added.Id);
            Assert.Contains(added, repo.GetAllRecords());

            // get by id
            var fetched = repo.GetRecordById(1);
            Assert.NotNull(fetched);
            Assert.Equal("A", fetched!.Artist);

            // update existing
            var updatedData = new ModelRecord { Artist = "B", Title = "T2", Duration = 200, PublicationYear = 2001 };
            var updated = repo.UpdateRecord(1, updatedData);
            Assert.NotNull(updated);
            Assert.Equal("B", updated!.Artist);

            // delete
            var deleted = repo.DeleteRecord(1);
            Assert.NotNull(deleted);
            Assert.Equal(1, deleted!.Id);
            Assert.Null(repo.GetRecordById(1));
        }

        [Fact]
        public void RecordRepo_AddNull_Throws()
        {
            var repo = new RecordRepo(false);
            Assert.Throws<ArgumentNullException>(() => repo.AddRecord(null!));
        }

        [Fact]
        public void RecordRepo_UpdateOrDelete_NonExisting_ReturnsNull()
        {
            var repo = new RecordRepo(false);
            var update = repo.UpdateRecord(999, new ModelRecord { Artist = "X", Title = "Y", Duration = 1, PublicationYear = 1999 });
            Assert.Null(update);
            var delete = repo.DeleteRecord(999);
            Assert.Null(delete);
        }

        [Fact]
        public void RecordRepo_Constructor_IncludeData_PopulatesData()
        {
            var repo = new RecordRepo(true);
            var all = repo.GetAllRecords().ToList();
            Assert.NotEmpty(all);
            // should assign ids starting at 1
            Assert.Equal(1, all.First().Id);
        }

        [Fact]
        public void RecordsController_Get_GetById_Post_Put_Delete_Options_Behavior()
        {
            var repo = new RecordRepo(false);
            var controller = new RecordsController(repo);

            // GET when empty -> NoContent
            var getEmpty = controller.Get();
            Assert.IsType<NoContentResult>(getEmpty.Result);

            // POST create
            var newRec = new ModelRecord { Artist = "Artist1", Title = "Title1", Duration = 120, PublicationYear = 2020 };
            var postResult = controller.Post(newRec);
            var created = Assert.IsType<CreatedResult>(postResult.Result);
            Assert.Equal($"api/records/{newRec.Id}", created.Location);

            // GET now returns Ok with records
            var getAll = controller.Get();
            var okAll = Assert.IsType<OkObjectResult>(getAll.Result);
            var list = Assert.IsAssignableFrom<IEnumerable<ModelRecord>>(okAll.Value);
            Assert.Single(list);

            // GET by id -> Ok
            var getById = controller.Get(newRec.Id);
            var okById = Assert.IsType<OkObjectResult>(getById.Result);
            var returned = Assert.IsType<ModelRecord>(okById.Value);
            Assert.Equal("Artist1", returned.Artist);

            // PUT update -> Ok
            var updatedData = new ModelRecord { Artist = "Artist2", Title = "Title2", Duration = 130, PublicationYear = 2021 };
            var putResult = controller.Put(newRec.Id, updatedData);
            var okPut = Assert.IsType<OkObjectResult>(putResult.Result);
            var afterPut = Assert.IsType<ModelRecord>(okPut.Value);
            Assert.Equal("Artist2", afterPut.Artist);

            // DELETE -> Ok
            var deleteResult = controller.Delete(newRec.Id);
            var okDelete = Assert.IsType<OkObjectResult>(deleteResult.Result);
            var delRecord = Assert.IsType<ModelRecord>(okDelete.Value);
            Assert.Equal(newRec.Id, delRecord.Id);

            // GET by id after delete -> NotFound
            var getNotFound = controller.Get(newRec.Id);
            Assert.IsType<NotFoundResult>(getNotFound.Result);

            // POST with null -> BadRequest (repo will throw and controller catches)
            var badPost = controller.Post(null!);
            Assert.IsType<BadRequestObjectResult>(badPost.Result);

            // PUT non-existing -> NotFound
            var putNotFound = controller.Put(999, new ModelRecord { Artist = "x", Title = "y", Duration = 1, PublicationYear = 1 });
            Assert.IsType<NotFoundResult>(putNotFound.Result);

            // DELETE non-existing -> NotFound
            var deleteNotFound = controller.Delete(999);
            Assert.IsType<NotFoundResult>(deleteNotFound.Result);

            // Options (void) -- should not throw
            controller.Options();
        }
    }
}
