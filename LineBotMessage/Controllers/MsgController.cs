using LineBotMessage.Hub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace LineBotMessage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MsgController : ControllerBase
    {
        private IHubContext<MessageHub, IMessageHub> messageHub;
        public MsgController(IHubContext<MessageHub, IMessageHub> _messageHub)
        {
            messageHub = _messageHub;
        }
        [HttpPost]
        [Route("toAll")]
        public string ToAll()
        {
            string msgs = "Don't forget, the deadline for submitting your expense reports is this Friday.+/r/n" +
                "Friendly reminder, please refrain from using the conference room for personal calls or meetings without prior approval.";
              // messageHub.Clients.All.SendAsync("message",msgs);
            return "Msg sent successfully to all users!";
        }
    }
}
