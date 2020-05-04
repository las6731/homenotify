using System.Collections.Generic;
using System.Threading.Tasks;
using HomeNotify.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeNotify.API.Controllers
{
    [ApiController]
    [Route("topics")]
    public class TopicsController : ControllerBase
    {
        private readonly ITopicsService topicsService;

        public TopicsController(ITopicsService topicsService)
        {
            this.topicsService = topicsService;
        }

        [HttpGet]
        public async Task<IList<string>> GetTopics()
        {
            return await this.topicsService.getTopics();
        }

        [HttpPost]
        public async Task<bool> AddTopic([FromBody]string topic)
        {
            return await this.topicsService.ensureTopic(topic);
        }

        [HttpDelete]
        public async Task<bool> RemoveTopic([FromBody]string topic)
        {
            return await this.topicsService.removeTopic(topic);
        }
    }
}