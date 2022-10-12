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

namespace CoreCodeCamp.Controllers
{
    [Route("api/camps/{moniker}/talks")]
    [ApiController]
    public class TalksController : Controller
    {
        private readonly ICampRepository repository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public TalksController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker)
        {
            try
            {
                var results = await this.repository.GetTalksByMonikerAsync(moniker);

                TalkModel[] models = this.mapper.Map<TalkModel[]>(results);

                return Ok(models);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TalkModel>> Get(string moniker, int id)
        {
            try
            {
                var result = await this.repository.GetTalkByMonikerAsync(moniker, id);

                TalkModel model = this.mapper.Map<TalkModel>(result);

                return Ok(model);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TalkModel>> Post(string moniker, TalkModel talkModel)
        {
            try
            {
                var camp = await this.repository.GetCampAsync(moniker);

                if (camp == null)
                {
                    return BadRequest("Camp does not exist");
                }

                var newTalk = this.mapper.Map<Talk>(talkModel);
                newTalk.Camp = camp;

                if (talkModel.Speaker == null)
                {
                    return BadRequest("Speaker Id is required");
                }

                var speaker = await this.repository.GetSpeakerAsync(talkModel.Speaker.SpeakerId);

                if (speaker == null)
                {
                    return BadRequest("Speaker couldn't be found");
                }

                newTalk.Speaker = speaker;

                this.repository.Add(newTalk);

                if (await this.repository.SaveChangesAsync())
                {
                    var url = this.linkGenerator.GetPathByAction(HttpContext, 
                        "Get", 
                        values : new { moniker, id = newTalk.TalkId });

                    return Created(url, this.mapper.Map<TalkModel>(newTalk));
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
            return BadRequest("Failed to create the talk");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TalkModel>> Put(string moniker, int id, TalkModel talkModel)
        {
            try
            {
                var oldTalk = await this.repository.GetTalkByMonikerAsync(moniker, id, true);

                if (oldTalk == null)
                {
                    return BadRequest("Couldn't find the talk");
                }

                this.mapper.Map(talkModel, oldTalk);

                if (talkModel.Speaker != null)
                {
                    var speaker = await this.repository.GetSpeakerAsync(talkModel.Speaker.SpeakerId);
                    if (speaker != null)
                    {
                        oldTalk.Speaker = speaker;
                    }
                }

                if (await this.repository.SaveChangesAsync())
                {
                    var editedTalk = this.mapper.Map<TalkModel>(oldTalk);
                    return editedTalk;
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
            return BadRequest("Failed to update the talk");
        }
    }
}
