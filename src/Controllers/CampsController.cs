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
using Microsoft.AspNetCore.Routing;

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

    //This tells the project (among other things) to make body binding by post or put requests.
    [ApiController] 
    public class CampsController : ControllerBase
    {
        //Property of type ICampRepository.
        private readonly ICampRepository repository;

        //Property of type IMapper.
        private readonly IMapper mapper;

        //Property of type LinkGenerator.
        private readonly LinkGenerator linkGenerator;

        //Constructor.
        public CampsController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            //The in-the-constructor injected parameters are assigned to the local properties (of course types of both match).
            this.repository = repository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
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

        [HttpPost]
        public async Task<ActionResult<CampModel>> Post(CampModel campModel)
        {
            try
            {
                var existing = await this.repository.GetCampAsync(campModel.Moniker);

                if (existing != null)
                {
                    return BadRequest("Moniker in use");
                }

                var location = this.linkGenerator.GetPathByAction("Get", "Camps", new { moniker = campModel.Moniker });

                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not use current moniker");
                }

                //This is the reverse of what happens in the get methods. Here the campModel is mapped into a Camp.
                var newCamp = this.mapper.Map<Camp>(campModel);

                this.repository.Add(newCamp);

                if (await this.repository.SaveChangesAsync())
                {
                    //The mapper is used again to return an instance of a CampModel (that's always what we want the user to see).
                    return Created(location, this.mapper.Map<CampModel>(newCamp));
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
            return BadRequest("Failed to create the camp");
        }

        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel campModel)
        {
            try
            {
                var oldCamp = await this.repository.GetCampAsync(moniker);

                if (oldCamp == null)
                {
                    return NotFound($"Could not find camp with moniker {moniker}");
                }

                //"oldCamp" is of type Camp.
                this.mapper.Map(campModel, oldCamp);

                if (await this.repository.SaveChangesAsync())
                {
                    var editedCamp = this.mapper.Map<CampModel>(oldCamp);
                    return editedCamp;
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
            return BadRequest("Failed to update the camp");
        }

        [HttpDelete("{moniker}")]
        public async Task<ActionResult<CampModel>> Delete(string moniker)
        {
            try
            {
                var oldCamp = await this.repository.GetCampAsync(moniker);

                if (oldCamp == null)
                {
                    return NotFound($"Could not find camp with moniker {moniker}");
                }

                this.repository.Delete(oldCamp);

                if (await this.repository.SaveChangesAsync())
                {
                    return Ok("Camp was deleted");
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
            return BadRequest("Failed to delete the camp");
        }
    }
}

/*
 * VERY IMPORTANT!!!
 * 
 * In order to update the database run: "dotnet ef database update" in the path where the solution file is. It is also
 * important to mention that the solution cannot run while updating the database, otherwise the build will throw an error.
 * 
 */
