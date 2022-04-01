using Context;
using Context.DAL.Data;
using Context.DAL.Data.DataPoints;
using Context.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DataCollector.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class DataPointController : Controller
    {

        InfluxDBUnitOfWork InfluxUoW = MonitoringFacade.Instance.InfluxDB;
        MongoDBUnitOfWork MongoUoW = MonitoringFacade.Instance.MongoDB;
        public DataPointController()
        {
        }


        [Authorize]
        [HttpGet("GetDataPoint")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataPoint))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DataPoint>> GetDataPoint([FromQuery][Required] String datapoint)
        {

            DataPoint dp = await MongoUoW.DataPoints.FindByIdAsync(datapoint);
            if (dp != null)
            {
                return dp;
            }
            else
            {
                return NotFound();
            }
        }


        [Authorize]
        [HttpGet("GetDataPoints")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<DataPoint>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<DataPoint>>> GetDataPoints([FromQuery][Required] String datasource)
        {

            DataSource dp = await MongoUoW.DataSources.GetDataSource(datasource);
            if (dp != null)
            {
                return dp.DataPoints;
            }
            else
            {
                return NotFound();
            }
        }



        [Authorize]
        [HttpPost("AddModbusDataPoint")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModbusDataPoint))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DataPoint>> AddModbusDataPoint([FromQuery][Required] String datasource, [FromBody][Required] ModbusDataPoint datapoint)
        {

            DataSource src = await MongoUoW.DataSources.FindOneAsync(x => x.Name == datasource);

            DataPoint pnt = null;
            if (src != null)
            {
                pnt = await MongoUoW.DataPoints.InsertOneAsync(datapoint);

            }

            if (src != null && pnt != null)
            {
                await MongoUoW.DataSources.AddDatapointToDataSoure(src, pnt);
            }

            if (pnt != null)
            {
                return pnt;
            }
            else
            {
                return NotFound();
            }
        }


        [Authorize]
        [HttpPost("AddMQTTDataPoint")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModbusDataPoint))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DataPoint>> AddMQTTDataPoint([FromQuery][Required] String datasource, [FromBody][Required] MQTTDataPoint datapoint)
        {

            DataSource src = await MongoUoW.DataSources.FindOneAsync(x => x.Name == datasource);

            DataPoint pnt = null;
            if (src != null)
            {
                pnt = await MongoUoW.DataPoints.InsertOneAsync(datapoint);

            }

            if (src != null && pnt != null)
            {
                await MongoUoW.DataSources.AddDatapointToDataSoure(src, pnt);
            }

            if (pnt != null)
            {
                return pnt;
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize]
        [HttpPatch("UpdateModbusDataPoint")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataPoint))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DataPoint>> UpdateModbusDataPoint([FromQuery][Required] String id, [FromBody][Required] ModbusDataPoint datapoint)
        {



            DataPoint pnt = await MongoUoW.DataPoints.UpdateOneAsync(datapoint);


            if (pnt != null)
            {
                return pnt;
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize]
        [HttpPatch("UpdateMQTTDataPoint")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataPoint))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DataPoint>> UpdateMQTTDataPoint([FromQuery][Required] String id, [FromBody][Required] MQTTDataPoint datapoint)
        {



            DataPoint pnt = await MongoUoW.DataPoints.UpdateOneAsync(datapoint);


            if (pnt != null)
            {
                return pnt;
            }
            else
            {
                return NotFound();
            }
        }
    }
}
