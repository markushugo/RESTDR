using RESTDR.Models;
using System.Globalization;

namespace RESTDR.Repos
{
    public class RecordRepo : IRecordRepo
    {
        private readonly List<Record> records = new();
        private int nextId = 1;

        public RecordRepo(bool includeData)
        {
            if (includeData)
            {
                AddRecord(new Record
                {
                    Artist = "Kim Larsen",
                    Title = "Midt Om Natten",
                    Duration = 230,
                    PublicationYear = 1983
                });

                AddRecord(new Record
                {
                    Artist = "Nephew",
                    Title = "007 Is Also Gonna Die",
                    Duration = 213,
                    PublicationYear = 2004
                });

                AddRecord(new Record
                {
                    Artist = "Lukas Graham",
                    Title = "7 Years",
                    Duration = 237,
                    PublicationYear = 2015
                });
            }
        }

        public Record AddRecord(Record record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }
            record.Id = nextId++;
            records.Add(record);
            return record;
        }

        public Record? DeleteRecord(int id)
        {
            var record = GetRecordById(id);
            if (record != null)
            {
                records.Remove(record);
                return record;
            }
            return null;
        }

        public IEnumerable<Record> GetAllRecords(string? artist, string? title)
        {
            IEnumerable<Record> result = records.AsReadOnly();
            if (artist != null)
            {
                result = result.Where(r => r.Artist.Contains(artist, StringComparison.OrdinalIgnoreCase));
            }
            if (title != null)
            {
                result = result.Where(r => r.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
            }
            return result;      
        }


        public Record? GetRecordById(int id)
        {
            return records.FirstOrDefault(r => r.Id == id);
        }

        public Record? UpdateRecord(int id, Record updatedRecord)
        {
            var existingRecord = GetRecordById(id);
            if (existingRecord != null)
            {
                existingRecord.Artist = updatedRecord.Artist;
                existingRecord.Title = updatedRecord.Title;
                existingRecord.Duration = updatedRecord.Duration;
                existingRecord.PublicationYear = updatedRecord.PublicationYear;
                return existingRecord;
            }
            return null;
        }



    }
}
