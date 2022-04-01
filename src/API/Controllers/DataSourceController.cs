using Context;
using Context.DAL.Data;
using Context.DAL.Data.Sources;
using Context.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DataCollector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DataSourceController : ControllerBase
    {

        InfluxDBUnitOfWork InfluxUoW = MonitoringFacade.Instance.InfluxDB;
        MongoDBUnitOfWork MongoUoW = MonitoringFacade.Instance.MongoDB;

        
        [HttpGet("GetDataSources")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<DataSource>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<DataSource>>> GetDataSources()
        {

            List<DataSource> dp = await MongoUoW.DataSources.GetDataSources();
            if (dp != null)
            {
                return dp;
            }
            else
            {
                return NotFound();
            }
        }

 
        [HttpGet("GetDataSource")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataSource))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DataSource>> GetDataSource([FromQuery] String datasource)
        {

            DataSource dp = await MongoUoW.DataSources.GetDataSource(datasource);
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
        [HttpPatch("UpdateModbusDataSource")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModbusDatasource))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ModbusDatasource>> UpdateModbusDataSource([FromQuery][Required] String id, [FromBody][Required] ModbusDatasource datapoint)
        {



            DataSource pnt = await MongoUoW.DataSources.UpdateOneAsync(datapoint);


            if (pnt != null)
            {
                return (ModbusDatasource)pnt;
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize]
        [HttpPatch("UpdateMqttDataSource")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MQTTDatasource))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MQTTDatasource>> UpdateMqttDataSource([FromQuery][Required] String id, [FromBody][Required] MQTTDatasource datapoint)
        {



            DataSource pnt = await MongoUoW.DataSources.UpdateOneAsync(datapoint);


            if (pnt != null)
            {
                return (MQTTDatasource)pnt;
            }
            else
            {
                return NotFound();
            }
        }


        [Authorize]
        [HttpPost("AddDataSource")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DataSource))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DataSource>> AddDataSource([FromBody][Required] DataSource datapoint)
        {



            DataSource pnt = await MongoUoW.DataSources.InsertOneAsync(datapoint);


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
