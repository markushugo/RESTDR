using RESTDanmarksRadio.Models;
namespace RESTDanmarksRadio.Repos
{
    public interface IRecordRepo
    {
        IEnumerable<Record> GetAllRecords();
        Record? GetRecordById(int id);
        Record AddRecord(Record record);
        Record? UpdateRecord(int id, Record updatedRecord);
        Record? DeleteRecord(int id);
    }
}
