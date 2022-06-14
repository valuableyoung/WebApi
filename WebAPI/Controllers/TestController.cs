using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {

    

        

        [HttpGet("test/first")]
        
        public string firsttest()
        {
            return "firsttest";
        }
        //[HttpGet("test/second")]
        public string secondtest()
        {
            return "secondtest";
        }
    }
}
