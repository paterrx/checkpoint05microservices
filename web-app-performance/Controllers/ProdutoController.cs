using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Newtonsoft.Json;
using StackExchange.Redis;
using web_app_domain;
using web_app_performance.Model;
using web_app_repository;


namespace web_app_performance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProdutoController : ControllerBase
    {

        private static ConnectionMultiplexer redis;
        private readonly IProdutoRepository _produtoRepository;

        public ProdutoController(IProdutoRepository produtoRepository) 
        {
            _produtoRepository = produtoRepository;
        }

        [HttpGet]

        public async Task<IActionResult> GetProduto()
        {
            //string key = "getproduto";
            //redis = ConnectionMultiplexer.Connect("localhost:6379");
            //IDatabase db = redis.GetDatabase();
            //await db.KeyExpireAsync(key, TimeSpan.FromMinutes(10));
            //string product = await db.StringGetAsync(key);

            //if (!string.IsNullOrEmpty(product))
            //{
            //    return Ok(product);
            //}

            var produtos = await _produtoRepository.ListarProduto();
            if(produtos == null)
            return NotFound();

            string produtosJson = JsonConvert.SerializeObject(produtos);
            //await db.StringSetAsync(key, produtosJson);

            return Ok(produtos);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Produto produto)
        {
            await _produtoRepository.SalvarProduto(produto);            

            //apagar o cachê
            //string key = "getproduto";
            //redis = ConnectionMultiplexer.Connect("localhost:6379");
            //IDatabase db = redis.GetDatabase();
            //await db.KeyDeleteAsync(key);

            return Ok(new { mensagem = "Produto criado com sucesso" });
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Produto produto)
        {
            await _produtoRepository.AtualizarProduto(produto);            

            //apagar o cachê
            string key = "getproduto";
            redis = ConnectionMultiplexer.Connect("localhost:6379");
            IDatabase db = redis.GetDatabase();
            await db.KeyDeleteAsync(key);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
           await _produtoRepository.RemoverProduto(id);           

            //apagar o cachê
            string key = "getproduto";
            redis = ConnectionMultiplexer.Connect("localhost:6379");
            IDatabase db = redis.GetDatabase();
            await db.KeyDeleteAsync(key);

            return Ok();
        }
    }
}
