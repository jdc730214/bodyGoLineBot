using LineBotMessage.Dtos;
using LineBotMessage.Domain;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using LineBotMessage.Enum;
using LineBotMessage.Providers;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text;


namespace LineBotMessage.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class LineBotController : ControllerBase
    {

        private readonly LineBotService _lineBotService;
        private readonly JsonProvider _jsonProvider;
        string linebotloginEvent = "";

        /* public IEnumerable<string> Get()
         {
             Trace.WriteLine("Get");
             return new string[] { "value1", "value2" };
         }*/
        // constructor
        public LineBotController()
        {
            _lineBotService = new LineBotService();
            _jsonProvider = new JsonProvider();
           
           
        }
        public class Message
        {
            public string username { get; set; }
            public string text { get; set; }
            public string dt { get; set; }
        }
        [HttpGet("eventtest")]
        public async Task<IActionResult> eventtest()
        {
           

            if (linebotloginEvent == "")
            {
                Console.WriteLine("Excel is busy");
                await Task.Delay(25);
                return Content("", "text/event-stream");
            }
            else
            {
                var stringBuilder = new StringBuilder();

                var serializedData = JsonSerializer.Serialize(
                       new { message = linebotloginEvent });
                stringBuilder.AppendFormat("data: {0}\n\n", serializedData);
                Trace.WriteLine(stringBuilder.ToString());
                linebotloginEvent = "";
                return Content(stringBuilder.ToString(), "text/event-stream");
            }
         //   var result = string.Empty;
            

        }
       
        [HttpGet("LoginEvent")]
        public void _lineBotService_bodyGoLoginTick(string msg)
        {
            linebotloginEvent = msg;
            Trace.WriteLine("bodyGoLoginTick:" + msg);


            //  throw new NotImplementedException();
        }
        [HttpPost("Webhook")]
        public IActionResult Webhook(WebhookRequestBodyDto body)
        {
            _lineBotService.ReceiveWebhook(body);
            return Ok();
        }
      

   
       
        [HttpPost("SendMessage/Broadcast")]
        public IActionResult Broadcast([Required] string messageType, object body)
        {
            _lineBotService.BroadcastMessageHandler(messageType, body);
            return Ok();
        }
    }
}