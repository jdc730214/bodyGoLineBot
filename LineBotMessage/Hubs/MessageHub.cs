
using LineBotMessage.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Data.SqlTypes;
using System.Diagnostics;
using static LineBotMessage.Controllers.LineBotController;

namespace LineBotMessage.Hub
{
    public class MessageHub : Hub<IMessageHub> 
    {


        public string currentLocalId = "";

        public static List<string> ConnIDList = new List<string>();
        
           /// <summary>
           /// 連線事件
           /// </summary>
           /// <returns></returns>
           [Authorize]
  
        public override async Task OnConnectedAsync()
        {
            LineBotService.lineBotSendInfoEvent += LineBotService_lineBotSendInfoEvent;


            if (ConnIDList.Where(p => p == Context.ConnectionId).FirstOrDefault() == null)
            {
                ConnIDList.Add(Context.ConnectionId);
            }
            // 更新連線 ID 列表
            string jsonString = JsonConvert.SerializeObject(ConnIDList);
            await Clients.All.UpdListConnections( jsonString);

            // 更新個人 ID
            currentLocalId = Context.ConnectionId;
             await Clients.Client(Context.ConnectionId).UpdSelfIDConnections(Context.ConnectionId);

           // // 更新聊天內容
           // await Clients.All.sendToAllConnections("UpdContent"/*, "新連線 ID: " + Context.ConnectionId*/);
            
           // await base.OnConnectedAsync();
            
        }

        private async void LineBotService_lineBotSendInfoEvent(string token, string id)
        {
            if (token != "" && id != "" && currentLocalId != "")
            {
                await this.Clients.Client(currentLocalId).UpdLineBotInfoConnections(token, id);
               
            }
        }

        /// <summary>
        /// 離線事件
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            //string id = ConnIDList.Where(p => p == Context.ConnectionId).FirstOrDefault();
            //if (id != null)
            //{
            //    ConnIDList.Remove(id);
            //}
            //// 更新連線 ID 列表
            //string jsonString = JsonConvert.SerializeObject(ConnIDList);
            //await Clients.All.UpdListConnections("UpdList"/*,jsonString*/);

            //// 更新聊天內容
            //await Clients.All.UpdListConnections("UpdContent"/*, "已離線 ID: " + Context.ConnectionId*/);

            //await base.OnDisconnectedAsync(ex);
        }

        /// <summary>
        /// 傳遞訊息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <param name="id"></param>
        /// <returns></returns>
  

      
        public async Task UpdateLocalID(string localID)
        {
            currentLocalId = localID;
           // await Clients.All.UpdListConnections(nameof(SendMessage));

            // await Clients.Client(Context.ConnectionId).UpdSelfIDConnections("UpdSelfID"/*, Context.ConnectionId*/);
            Trace.WriteLine("msg="+ currentLocalId);


        
            
          
        }
      
     

    }
}
