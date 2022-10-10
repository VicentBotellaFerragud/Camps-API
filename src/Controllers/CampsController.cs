using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

/*
 * For each request ASP.NET Core tries to match the route of the request URL with any of the existing routes of the project.
 * When it finds a route that matches an existing route of the project, it looks at the action and the controller for that
 * route and executes it.
 * 
 * By "action" it is meant --> a method on a class. The controller is just a class that has methods (actions) on it. Once that
 * action is exectuted the response of that action can be returned.
 */
namespace CoreCodeCamp.Controllers
{
    //"[controller]" means --> route is whatever comes before the word "controller" ("camps" in this case).
    [Route("api/[controller]")]
    public class CampsController : ControllerBase
    {
        //Property to which the ICampRepository is assigned in the constructor.
        private readonly ICampRepository repository;

        //Property to which the IMapper is assigned in the constructor.
        private readonly IMapper mapper;

        //Constructor. Here is the ICampRepository and the IMapper injected.
        public CampsController(ICampRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        //Indicates the method/action.
        [HttpGet]
        public async Task<ActionResult<CampModel[]>> Get(bool includeTalks = false)
        {
            try
            {
                var results = await this.repository.GetAllCampsAsync(includeTalks);

                //Using the mapper the results data (of type Camp) are converted into CampsModel data.
                CampModel[] models = this.mapper.Map<CampModel[]>(results);

                //The Ok ControllerBase method returns the data with a 200 status code.
                return Ok(models);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }

        //"moniker" is the extension of the route. If "moniker" was an integer we should have: "{moniker:int}".
        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> Get(string moniker, bool includeTalks = false)
        {
            try
            {
                var result = await this.repository.GetCampAsync(moniker, includeTalks);

                if (result == null)
                {
                    return NotFound();
                }

                var model = this.mapper.Map<CampModel>(result);

                return Ok(model);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime date, bool includeTalks = false)
        {
            try
            {
                var results = await this.repository.GetAllCampsByEventDate(date, includeTalks);

                if (!results.Any())
                {
                    return NotFound();
                }

                var models = this.mapper.Map<CampModel[]>(results);

                return Ok(models);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }
    }
}

/*
 * VERY IMPORTANT!!!
 * 
 * In order to update the database run: "dotnet ef database update" in the path where the solution file is. It is also
 * important to mention that the solucion cannot run while updating the database, otherwise the build will throw an error.
 * 
 */
