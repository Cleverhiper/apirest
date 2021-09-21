using System.Linq;
using apirest.Data;
using apirest.Models;
using Microsoft.AspNetCore.Mvc;

namespace apirest.Controllers
{
    [Route ("/api/[Controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly ApplicationDbContext database;

        public ProdutosController(ApplicationDbContext database) {
            this.database = database;
        }

        [HttpGet]
        public IActionResult PegarProdutos(){  
            var produtos = database.Produtos.ToList();
            return Ok(produtos); //retorna 200 com dados
        }

        [HttpGet("{id}")]
         public IActionResult PegarProdutos(int id){
            try
            {
                var produtos = database.Produtos.First(p => p.Id == id);
                return Ok(produtos); //retorna 200 com dados
            }
            catch (System.Exception e)
            {
                var mensagem = e.Message.ToString();
                return BadRequest(new {msg = mensagem});
            }
        }

        [HttpPost]
        public IActionResult SalvarProduto([FromBody] ProdutoTemp pTemp){
            Produto p = new Produto();
            p.Nome = pTemp.Nome;
            p.Preco = pTemp.Preco;
            database.Produtos.Add(p);
            database.SaveChanges();

            return Ok(new {info = "Produto criado com sucesso!!"});
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id) {
            try
            {
                 Produto  produtos = database.Produtos.First(p => p.Id == id);
                 database.Produtos.Remove(produtos);
                 database.SaveChanges();
                 return Ok();
            }
            catch (System.Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("");
            }
            
        }

        public class ProdutoTemp{
            public string Nome {get; set;}
            public float Preco {get; set;}
        }
    }
}