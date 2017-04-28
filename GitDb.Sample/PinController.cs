using System;
using System.Threading.Tasks;
using System.Web.Http;
using GitDb.Core.Model;
using Newtonsoft.Json;

namespace GitDb.Sample.Controllers
{
    public class PinController : ApiController
    {
        readonly Author _author = new Author("kenneth", "truyers.kenneth@gmail.com");

        [Route("pins")]
        [HttpGet]
        public async Task<IHttpActionResult> Get([FromUri] string branch) =>
            Ok(await Git.Instance.GetFiles<Pin>(branch, "pins"));

        [Route("pins")]
        [HttpPost]
        public IHttpActionResult Post([FromUri] string branch, [FromBody] Pin pin)
        {
            pin.Id = Guid.NewGuid().ToString();
            Git.Instance.Save(branch, "Created pin " + pin.Name, new Document<Pin> {Key = "pins/" + pin.Id, Value = pin}, _author);
            return Created("/pins/" + pin.Id, pin);
        }

        [Route("pins")]
        [HttpPut]
        public IHttpActionResult Put([FromUri] string branch, [FromBody] Pin pin)
        {
            Git.Instance.Save(branch, "Created pin " + pin.Name, new Document<Pin> { Key = "pins/" + pin.Id, Value = pin }, _author);
            return Ok();
        }

        [Route("pins")]
        [HttpDelete]
        public IHttpActionResult Delete([FromUri] string branch, [FromUri] string id)
        {
            Git.Instance.Delete(branch, "pins/" + id, "Deleted pin with id " + id, _author);
            return Ok();
        }

        [Route("branch")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBranches() =>
            Ok(await Git.Instance.GetAllBranches());
    }

    public class Pin
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("lat")]
        public double Lat { get; set; }
        [JsonProperty("lng")]
        public double Lng { get; set; }
    }
}