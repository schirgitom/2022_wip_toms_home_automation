using Context.DAL;
using Context.DAL.Alarming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.Repositories.Concrete
{
    public interface IAlarmListRepository : IMongoRepository<AlarmListEntry>
    {

        Task<AlarmListEntry> IsAlarmActiveForDataPoint(String name);
        Task<AlarmListEntry> IsAlarmActiveForDataPointAndLevel(String name, AlarmType type);
        Task<List<AlarmListEntry>> GetAlarmListForDataPoint(string name);
        Task<List<AlarmListEntry>> GetActiveAlarms();
        Task<List<AlarmListEntry>> GetDeactiveAlarms();
        Task<List<AlarmListEntry>> GetUnacknowledgedAlarms();
        Task<List<AlarmListEntry>> GetAllAlarms();
        Task<List<AlarmListEntry>> AcknowledgeAlarm(string id, String comment, User usr);
    }
}
