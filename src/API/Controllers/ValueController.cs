using Context;
using Context.DAL.Alarming;
using Context.DAL.Data;
using Context.DAL.InfluxDB;
using Context.DAL.Visuals;
using Context.UnitOfWork;
using DataCollector.ReturnModels;
using DataCollector.ReturnModels.Visuals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DataCollector.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ValueController : ControllerBase
    {

        InfluxDBUnitOfWork InfluxUoW = MonitoringFacade.Instance.InfluxDB;
        MongoDBUnitOfWork MongoUoW = MonitoringFacade.Instance.MongoDB;
        public ValueController()
        {
        }


        private VisualsReturnModel CreateVisuals(DataPoint dp, Sample ms)
        {

            if (dp.Visual != null)
            {

                if (dp.DataType == Context.DAL.Data.DataType.Boolean)
                {
                    VisualsBinaryReturnModel returnval = new VisualsBinaryReturnModel();
                    returnval.Icon = dp.Visual.Icon;

                    if (dp.Visual.GetType() == typeof(BinaryDataPointVisuals))
                    {
                        BinaryDataPointVisuals bbp = (BinaryDataPointVisuals)dp.Visual;
                        if (bbp.ValueMapping != null && bbp.ValueMapping.Count > 0)
                        {
                            if (ms.GetType() == typeof(BinarySample))
                            {
                                var bin = (BinarySample)ms;

                                var map = bbp.ValueMapping.Where(x => Utilities.Converter.ConvertToBoolean(x.Value) == bin.AsBoolean()).FirstOrDefault();

                                if (map != null)
                                {
                                    returnval.FinalText = map.Text;
                                }
                            }
                        }
                        
                    }

                    return returnval;
                }
                else
                {
                    VisualsNumericReturnModel returnval = new VisualsNumericReturnModel();
                    returnval.Icon = dp.Visual.Icon;

                    if (dp.Visual.GetType() == typeof(NumericDataPointVisuals))
                    {
                        NumericDataPointVisuals bbp = (NumericDataPointVisuals)dp.Visual;

                        returnval.MinValue = bbp.MinValue;
                        returnval.MaxValue = bbp.MaxValue;
                        returnval.Unit = bbp.Unit;
                       
                      
                        if(dp.AlarmThresholds != null && dp.AlarmThresholds.Count > 0)
                        {
                            foreach(KeyValuePair<String, AlarmThreshold> ths in dp.AlarmThresholds)
                            {
                                AlarmThreshold th = ths.Value;
                                VisualsThresholdModel modl = new VisualsThresholdModel();
                                modl.AlarmType = th.AlarmType;
                                modl.Value = th.Threashold;
                                modl.AlarmCheckType = th.AlarmCheckType;
                                modl.AlarmType = th.AlarmType;

                                returnval.Thresholds.Add(ths.Key, modl);
                            }


                        }

                        return returnval;
                           
                    }
                }
            }
           

            return null;
        }

        [Authorize]
        [HttpGet("GetLastValue")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ValueReturnModelSingle))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ValueReturnModelSingle>> GetLastValue([FromQuery][Required] String datapoint)
        {

            DataPoint dp = await MongoUoW.DataSources.GetDatapoint(datapoint);
            Sample meas = await InfluxUoW.Repository.GetLast(dp);

            if(dp != null)
            {
                ValueReturnModelSingle returnmodel = new ValueReturnModelSingle();
                returnmodel.DataPoint = dp;
                returnmodel.Sample = meas;
                returnmodel.Visuals = CreateVisuals(dp, meas);

                return returnmodel;
            }
            else
            {
                return NotFound();
            }
        }


      


        [Authorize]
        [HttpGet("GetLastValues")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ValueReturnModelSingle>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<ValueReturnModelSingle>>> GetLastValues()
        {

            List<DataPoint> dps = await MongoUoW.DataSources.GetDatapoints();
            List<ValueReturnModelSingle> returnval = new List<ValueReturnModelSingle>();
            if (dps != null)
            {
                foreach(DataPoint dp in dps)
                {
                    Sample meas = await InfluxUoW.Repository.GetLast(dp);

                    ValueReturnModelSingle returnmodel = new ValueReturnModelSingle();
                    returnmodel.DataPoint = dp;
                    returnmodel.Sample = meas;
                    returnmodel.Visuals = CreateVisuals(dp, meas);

                    returnval.Add(returnmodel);
                }


            }

            return returnval;
            
        }



        [Authorize]
        [HttpGet("GetValuesInRange")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ValueReturnModelMultiple))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ValueReturnModelMultiple>> GetNumericValuesInRange([FromQuery][Required] String datapoint, [FromQuery][Required] DateTime from, [FromQuery][Required] DateTime to)
        {
            DataPoint dp = await MongoUoW.DataSources.GetDatapoint(datapoint);
            List<Sample> meas = await InfluxUoW.Repository.GetInTimeRange(dp, from, to);

            if (meas != null)
            {
                ValueReturnModelMultiple returnmodel = new ValueReturnModelMultiple();
                returnmodel.DataPoint = dp;
                returnmodel.Samples = meas;

                return returnmodel;
            }
            else
            {
                return NotFound();
            }
        }



    }
}
