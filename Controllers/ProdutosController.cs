using System.Linq;
using apirest.Data;
using apirest.Models;
using Microsoft.AspNetCore.Mvc;
using apirest.HATEOAS;

namespace apirest.Controllers
{
    [Route ("/api/[Controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        private HATEOAS.HATEOAS HATEOAS;
        public ProdutosController(ApplicationDbContext database) {
            this.database = database;
            HATEOAS = new HATEOAS.HATEOAS("localhost:5001/api/Produtos");
            HATEOAS.AddAction("DELETE_PRODUCT","DELETE");
            HATEOAS.AddAction("EDIT_PRODUCT","PATCH");
            
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
                Produto produto = database.Produtos.First(p => p.Id == id);

                ProdutoContainer produtoHATEOAS = new ProdutoContainer();
                produtoHATEOAS.produto = produto;
                produtoHATEOAS.links = HATEOAS.GetActions();

                return Ok(produtoHATEOAS); //retorna 200 com dados
            }
            catch (System.Exception e)
            {
                var mensagem = e.Message.ToString();
                return BadRequest(new {msg = mensagem});
            }
        }

        [HttpPost]
        public IActionResult SalvarProduto([FromBody] ProdutoTemp pTemp){

            if (pTemp.Preco <= 0) {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O preço do produto deve ser maior que zero!"});
            }

            if (pTemp.Nome.Trim().Length <=1)
            {
                Response.StatusCode = 400;
                return new ObjectResult (new {msg = "O nome do produto precisar ter mais que um caracter!"});
            }


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

         [HttpPatch]
         public IActionResult Editar([FromBody] Produto pTemp){
            try
            {
                var produtos = database.Produtos.First(p => p.Id == pTemp.Id);
                if (produtos != null)
                {
                    produtos.Nome = pTemp.Nome != null && pTemp.Nome.Trim().Length > 0 ? pTemp.Nome : produtos.Nome;
                    produtos.Preco = pTemp.Preco > 0 ? pTemp.Preco : produtos.Preco;
                    database.SaveChanges();
                    return Ok(); //retorna 200 com dados
                }
                else {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Produto não encontrado!"});
                }
            }
            catch (System.Exception e)
            {
                var mensagem = e.Message.ToString();
                return BadRequest(new {msg = mensagem});
            }
        }

        public class ProdutoTemp{
            public int Id {get;}
            public string Nome {get; set;}
            public float Preco {get; set;}
        }

        public class ProdutoContainer{
            public Produto produto;
            public Link[] links;
        }
    }
}