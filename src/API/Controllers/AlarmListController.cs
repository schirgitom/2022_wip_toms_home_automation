using Context;
using Context.DAL;
using Context.DAL.Alarming;
using Context.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DataCollector.Controllers
{

    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AlarmListController : ControllerBase
    {

        InfluxDBUnitOfWork InfluxUoW = MonitoringFacade.Instance.InfluxDB;
        MongoDBUnitOfWork MongoUoW = MonitoringFacade.Instance.MongoDB;
        public AlarmListController()
        {
        }


        [Authorize]
        [HttpGet("GetActiveAlarmList")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AlarmListEntry>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<AlarmListEntry>>> GetActiveAlarmList()
        {
            List<AlarmListEntry> alarms = await MongoUoW.AlarmList.GetActiveAlarms();

            return alarms;
        }



        [Authorize]
        [HttpGet("GetDeactiveAlarmList")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AlarmListEntry>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<AlarmListEntry>>> GetDeactiveAlarmList()
        {
            List<AlarmListEntry> alarms = await MongoUoW.AlarmList.GetDeactiveAlarms();

            return alarms;
        }

        [Authorize]
        [HttpGet("GetUnacknowledgedAlarmList")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AlarmListEntry>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<AlarmListEntry>>> GetUnacknowledgedAlarmList()
        {
            List<AlarmListEntry> alarms = await MongoUoW.AlarmList.GetUnacknowledgedAlarms();

            return alarms;
        }

        [Authorize]
        [HttpGet("GetAllAlarms")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AlarmListEntry>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<AlarmListEntry>>> GetAllAlarms()
        {
            List<AlarmListEntry> alarms = await MongoUoW.AlarmList.GetAllAlarms();

            List<AlarmListEntry> ret =  alarms.OrderByDescending(x => x.ActiveDate).ToList();

            return ret;
        }



        [Authorize]
        [HttpGet("GetAlarm")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AlarmListEntry))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AlarmListEntry>> GetAlarm([FromQuery][Required] String id)
        {
            AlarmListEntry alarms = await MongoUoW.AlarmList.FindByIdAsync(id);

     

            return alarms;
        }





        [Authorize(Roles = "Admin")]
        [HttpPatch("AcknowledgeAlarm")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AlarmListEntry>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<AlarmListEntry>>> AcknowledgeAlarm([FromQuery] String AlarmID)
        {

            var token = Request.Headers["Authorization"];

            if(!String.IsNullOrEmpty(token))
            {
                String email = MonitoringFacade.Instance.Authentication.GetEmailByToken(token);

                if(!String.IsNullOrEmpty(email))
                {
                    User user = await MongoUoW.Users.FindOneAsync(x => x.UserName == email);

                    if(user != null)
                    {
                        await MongoUoW.AlarmList.AcknowledgeAlarm(AlarmID, "", user);
                    }
                }
            }

            List<AlarmListEntry> alarms = await MongoUoW.AlarmList.GetAllAlarms();

            return alarms;
        }


       


    }
}
