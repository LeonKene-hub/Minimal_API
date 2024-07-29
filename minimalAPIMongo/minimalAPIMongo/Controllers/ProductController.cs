using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using minimalAPIMongo.Domains;
using minimalAPIMongo.Services;
using MongoDB.Driver;

namespace minimalAPIMongo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProductController : ControllerBase
    {
        /// <summary>
        /// Armazena os dados de acesso da collection
        /// </summary>
        private readonly IMongoCollection<Product>? _product;

        /// <summary>
        /// Construtor que recebe como dependencia o obj da classe MongoDbService
        /// </summary>
        /// <param name="mongoDbService">obj da classe MongoDbService</param>
        public ProductController(MongoDbService mongoDbService)
        {
            //obtem a collection "product"
            _product = mongoDbService.GetDatabase.GetCollection<Product>("product");
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> Get()
        {
            try
            {
                var products = await _product.Find(FilterDefinition<Product>.Empty).ToListAsync();
                return Ok(products);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(string id)
        {
            try
            {
                var productAchado = await _product.Find(p => p.Id == id).FirstOrDefaultAsync();
                return productAchado is not null ? Ok(productAchado) : NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post(Product Novoproduct)
        {
            try
            {
                await _product.InsertOneAsync(Novoproduct);
                return StatusCode(201);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Update(Product atualizado)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.Id, atualizado.Id);

                await _product.ReplaceOneAsync(filter, atualizado);
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
                await _product.FindOneAndDeleteAsync(p => p.Id == id);
                return StatusCode(204);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
