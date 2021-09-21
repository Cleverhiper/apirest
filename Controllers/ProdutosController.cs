using Microsoft.AspNetCore.Mvc;

namespace apirest.Controllers
{
    [Route ("/api/[Controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        [HttpGet]
        public IActionResult PegarProdutos(){
            return Ok(new {empresa = "Wald Sistemas", nome = "Cleverson Silva"}); //retorna 200 com dados
        }

        [HttpPost]
        public IActionResult SalvarProduto(){
            return Ok("Salvando via Post");
        }
    }
}