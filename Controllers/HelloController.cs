 using Microsoft.AspNetCore.Mvc;

 namespace Synoptis.API.controllers
 {
    
    // transforme ce contrôleur en API REST
    [ApiController]
    //  l’URL sera /hello (nom du contrôleur sans "Controller")
    [Route("api/[controller]")]
    public class HelloController : ControllerBase
    {
        // Mettre le type de route entre crochet
        [HttpGet]
        // En gros quand on vas faire un get sur /api/hello on vas appeler cette fonction Get()
        public IActionResult Get()
        {
            // OK() c'est equivalent à une reponse 200 http OK
            return Ok("Bienvenue sur Synoptis !");
        }
    }
 }  