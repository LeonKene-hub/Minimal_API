using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using minimalAPIMongo.Domains;
using minimalAPIMongo.Services;
using minimalAPIMongo.ViewModels;
using MongoDB.Driver;

namespace minimalAPIMongo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class OrderController : ControllerBase
    {
        private readonly IMongoCollection<Order> _order;
        private readonly IMongoCollection<Client> _client;
        private readonly IMongoCollection<Product> _product;

        public OrderController(MongoDbService mongoDbService)
        {
            _order = mongoDbService.GetDatabase.GetCollection<Order>("order");
            _client = mongoDbService.GetDatabase.GetCollection<Client>("client");
            _product = mongoDbService.GetDatabase.GetCollection<Product>("product");
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
        public async Task<ActionResult> Create(OrderViewModel newOrder)
        {
            try
            {
                Order order = new Order();
                order.Id = newOrder.Id;
                order.Date = newOrder.Date;
                order.Status = newOrder.Status;
                order.ProductId = newOrder.ProductId;
                order.ClientId = newOrder.ClientId;

                var clientOwner = _client.Find(c => c.Id == newOrder.ClientId).FirstOrDefaultAsync();

                if (clientOwner is not null)
                {
                    order.Client = await clientOwner;
                }
                else
                {
                    return NotFound("Cliente nao encontrado");
                }

                var lista = new List<Product>();

                foreach (var productId in newOrder.ProductId!)
                {
                    var item = _product.Find(p => p.Id == productId).FirstOrDefault();

                    if (item is not null)
                    {
                        lista.Add(item);
                    }
                }

                order.Products = lista;

                await _order.InsertOneAsync(order);
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
