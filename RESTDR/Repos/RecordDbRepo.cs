using RESTDanmarksRadio.Models;

namespace RESTDanmarksRadio.Repos
{
    public class RecordDbRepo : IRecordRepo
    {
        private readonly RecordDbContext _context;


        public RecordDbRepo(RecordDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Record> GetAllRecords()
        {
            return _context.Records.ToList();
        }

        public Record? GetRecordById(int id)
        {
            return _context.Records.FirstOrDefault(r => r.Id == id);
        }

        public Record AddRecord(Record record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            _context.Records.Add(record);
            _context.SaveChanges();
            return record;
        }

        public Record? UpdateRecord(int id, Record updatedRecord)
        {
            var record = GetRecordById(id);
            if (record == null)
            {
                return null;
            }

            record.Artist = updatedRecord.Artist;
            record.Title = updatedRecord.Title;
            record.Duration = updatedRecord.Duration;
            record.PublicationYear = updatedRecord.PublicationYear;

            _context.SaveChanges();
            return record;
        }

        public Record? DeleteRecord(int id)
        {
            var record = GetRecordById(id);
            if (record == null)
            {
                return null;
            }

            _context.Records.Remove(record);
            _context.SaveChanges();
            return record;
        }
    }
}