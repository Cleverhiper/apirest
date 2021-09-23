using apirest.Models;
using Microsoft.AspNetCore.Mvc;
using apirest.Data;
using System.Linq;
using System;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace apirest.Controllers
{
    [Route("api/v1/[Controller]")]
    [ApiController]
    public class UsuariosController: ControllerBase
    {
        private readonly ApplicationDbContext database;

        public UsuariosController(ApplicationDbContext database){
            this.database = database;
        }

        [HttpPost("registro")]
        public IActionResult Registro([FromBody] Usuario usuarios) {
            database.Add(usuarios);
            database.SaveChanges();
            return Ok (new{Migrations="Usuário cadastro com sucesso."});
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] Usuario credenciais) {

            try {

            
                //Buscar um usuário por E-mail
                //Verificar se a senha está correta
                //Gerar um token JWT e retornar esse token para o usuário
                Usuario usuario = database.Usuarios.First(user => user.Email.Equals(credenciais.Email));
                if (usuario != null){
                    //Achou usuario com cadastro valido
                    if (usuario.Senha.Equals(credenciais.Senha)){
                        string chaveDeSeguranca = "Chave Secreta do Projeto Api Rest";
                        var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeSeguranca));
                        var credenciaisDeAcesso = new SigningCredentials(chaveSimetrica,SecurityAlgorithms.HmacSha256Signature);
                        var jwt_token = new JwtSecurityToken(
                            issuer: "Projeto API Rest",   //Quem está fornecendo o JWT para o usuário
                            expires: DateTime.Now.AddHours(1), //Tempo de expiração do Token
                            audience: "Usuario Comum", 
                            signingCredentials: credenciaisDeAcesso
                        );

                        return Ok(new JwtSecurityTokenHandler().WriteToken(jwt_token));

                    } else {
                        //Senha inválida
                        Response.StatusCode = 401;
                        return new ObjectResult(new {msg = "Não Autorizado" });
                    }
                }else{
                    //Não existe nenhum usuário com este e-mail
                    Response.StatusCode = 401; // Não autorizado
                    return new ObjectResult(new {msg = "Não Autorizado" });
                }
            }
            catch (System.Exception e) {
                var mensagem = e.Message.ToString();
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = mensagem });
            }
            
            
        }

        
    }
}