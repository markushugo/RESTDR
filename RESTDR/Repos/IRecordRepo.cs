using RESTDR.Models;
namespace RESTDR.Repos
{
    public interface IRecordRepo
    {
        IEnumerable<Record> GetAllRecords(string? artist, string? titel);
        Record? GetRecordById(int id);
        Record AddRecord(Record record);
        Record? UpdateRecord(int id, Record updatedRecord);
        Record? DeleteRecord(int id);
    }
}
