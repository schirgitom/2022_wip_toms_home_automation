using Context;
using Context.DAL.Data;
using Context.DAL.Visuals;
using Context.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Home_Automation_Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class VisualsController : Controller
    {

        MongoDBUnitOfWork MongoUoW = MonitoringFacade.Instance.MongoDB;





        [Authorize]
        [HttpGet("GetVisuals")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<DataPointVisual>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<DataPointVisual>>> GetVisuals()
        {

            List<DataPointVisual> dp = MongoUoW.DataPointVisuals.FilterBy(x => true).ToList();

            if (dp != null)
            {
              
                return dp;
            }
            else
            {
                return NotFound();
            }
        }


    }
}
