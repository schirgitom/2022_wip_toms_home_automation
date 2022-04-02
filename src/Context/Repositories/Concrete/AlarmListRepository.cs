    using Context.DAL;
    using Context.DAL.Alarming;
    using Context.DAL.Data;
    using Context.Repositories;
    using Context.Repositories.Concrete;
    using Context.Settings;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using MongoDB.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Context.Repository
    {
        public class AlarmListRepository : MongoRepository<AlarmListEntry>, IAlarmListRepository
        {
            public AlarmListRepository(MongoDBContext Context) : base(Context)
            {

            }

            public async Task<AlarmListEntry> IsAlarmActiveForDataPoint(string name)
            {
                AlarmListEntry alarmListEntry = await FindOneAsync(x => x.DataPoint != null && x.DataPoint.DatabaseName == name && x.AlarmStatus == AlarmStatus.Active && x.AcknowledgeStatus == AcknowledgeStatus.NotAcknowledged);

                return alarmListEntry;
            }

            public async Task<AlarmListEntry> IsAlarmActiveForDataPointAndLevel(string name, AlarmType type)
            {
                AlarmListEntry alarmListEntry = await FindOneAsync(x => x.DataPoint != null && x.DataPoint.DatabaseName == name && x.AlarmType == type && x.AlarmStatus == AlarmStatus.Active && x.AcknowledgeStatus == AcknowledgeStatus.NotAcknowledged);

                return alarmListEntry;
            }


            public async Task<List<AlarmListEntry>> GetAlarmListForDataPoint(string name)
            {
                List<AlarmListEntry> alarmListEntry = FilterBy(x => x.DataPoint != null && x.DataPoint.DatabaseName == name).ToList();

                return alarmListEntry;
            }

            public async Task<List<AlarmListEntry>> GetActiveAlarms()
            {
                List<AlarmListEntry> alarmListEntry = FilterBy(x => x.AlarmStatus == AlarmStatus.Active).ToList();

                return alarmListEntry;
            }

            public async Task<List<AlarmListEntry>> GetDeactiveAlarms()
            {
                List<AlarmListEntry> alarmListEntry = FilterBy(x => x.AlarmStatus == AlarmStatus.Deactive).ToList();

                return alarmListEntry;
            }

            public async Task<List<AlarmListEntry>> GetUnacknowledgedAlarms()
            {
                List<AlarmListEntry> alarmListEntry = FilterBy(x => x.AcknowledgeStatus == AcknowledgeStatus.NotAcknowledged).ToList();
                return alarmListEntry;
            }

            public async Task<List<AlarmListEntry>> GetAllAlarms()
            {
                List<AlarmListEntry> alarmListEntry = FilterBy(x => true).ToList();
                return alarmListEntry;
            }

            public async Task<List<AlarmListEntry>> AcknowledgeAlarm(string id, String comment, User usr)
            {
                AlarmListEntry alarmListEntry = await FindOneAsync(x => x.ID == id);

                if (alarmListEntry != null)
                {
                    alarmListEntry.AcknowledgeComment = comment;
                    alarmListEntry.AcknowledgeDate = DateTime.Now;
                    alarmListEntry.AcknowledgeStatus = AcknowledgeStatus.Acknowledged;
                    alarmListEntry.AcknowledgeUser = usr;

                    await UpdateOneAsync(alarmListEntry);

                }

                return await GetUnacknowledgedAlarms();

            }
        }
    }

