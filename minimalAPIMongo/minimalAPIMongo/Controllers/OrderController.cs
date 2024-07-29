using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using minimalAPIMongo.Domains;
using minimalAPIMongo.Services;
using MongoDB.Driver;

namespace minimalAPIMongo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class OrderController : ControllerBase
    {
        private readonly IMongoCollection<Order> _order;

        public OrderController(MongoDbService mongoDbService)
        {
            _order = mongoDbService.GetDatabase.GetCollection<Order>("order");
        }

        [HttpGet]
        public async Task<ActionResult<List<Order>>> Get()
        {
            try
            {
                var orders = await _order.Find(FilterDefinition<Order>.Empty).ToListAsync();
                return Ok(orders);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById(string id)
        {
            try
            {
                var orderFinded = await _order.Find(o => o.Id == id).FirstOrDefaultAsync();
                return orderFinded is not null ? Ok(orderFinded) : NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post(Order newOrder)
        {
            try
            {
                await _order.InsertOneAsync(newOrder);
                return StatusCode(204);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Update(Order atualizado)
        {
            try
            {
                var filter = Builders<Order>.Filter.Eq(o => o.Id, atualizado.Id);
                await _order.ReplaceOneAsync(filter, atualizado);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]  
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                await _order.FindOneAndDeleteAsync(o => o.Id == id);
                return StatusCode(204);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
