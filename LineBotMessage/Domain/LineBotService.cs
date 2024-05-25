using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using LineBotMessage.Dtos;
using LineBotMessage.Enum;
using LineBotMessage.Hub;
using LineBotMessage.Providers;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;


namespace LineBotMessage.Domain
{
    public class LineBotService
    {

        // 貼上 messaging api channel 中的 accessToken & secret
        private readonly string channelAccessToken = "2o0n+bT779iWQ/a01CeLJNltOKV+qCQ/EVnsmDHSeKVHPYHdLI8COtbeYfgCkGnqnfT1Y11M0F4RnajHo6ethQJKjuOQGJ95H5f4p5w0DKH9BgqS4FeNdcmrpb/8lgGjBNlooNVCefVRwGs7Mhe3ZQdB04t89/1O/w1cDnyilFU=";
        private readonly string channelSecret = "f2561a68ab08899e0ae48ab469efff40";


        private readonly string replyMessageUri = "https://api.line.me/v2/bot/message/reply";
        private readonly string broadcastMessageUri = "https://api.line.me/v2/bot/message/broadcast";


        private static HttpClient client = new HttpClient();
        private readonly JsonProvider _jsonProvider = new JsonProvider();


       

        public LineBotService()
        {
        }
        public delegate void lineSendHandler(string token ,string id);
        public static event lineSendHandler lineBotSendInfoEvent;
        public void ReceiveWebhook(WebhookRequestBodyDto requestBody)
        {
            dynamic replyMessage;
            foreach (var eventObject in requestBody.Events)
            {
                switch (eventObject.Type)
                {
                    case WebhookEventTypeEnum.Message:
                        if (eventObject.Message.Type == MessageTypeEnum.Text)
                        {

                            if (eventObject.Postback != null)
                            {
                                if (eventObject.Postback.Data.Contains("BodyGo_User_Info"))
                                {
                                    Trace.WriteLine($"使用者{eventObject.Source.UserId}觸發了postback事件:" + eventObject.Postback.Data + "contant:" + eventObject.Postback.Params.Date.ToString());

                                }
                            }
                            


                            ReceiveMessageWebhookEvent(eventObject);
                            Trace.WriteLine($"使用者{eventObject.Source.UserId}在聊天室發送訊息:" + eventObject.Message.Text);

                            if (lineBotSendInfoEvent != null)
                            {
                                if (eventObject.Message.Text == "報告")
                                {
                                    lineBotSendInfoEvent(channelAccessToken, eventObject.Source.UserId);

                                  

                                }
                            }

                            /*
                              var replyMessage = new ReplyMessageRequestDto<TextMessageDto>()
                              {
                                  ReplyToken = eventObject.ReplyToken,
                                  Messages = new List<TextMessageDto>
                                  {
                                      new TextMessageDto(){Text = $"您好，您傳送了\"{eventObject.Message.Text}\"!"}
                                  }
                              };
                              ReplyMessageHandler("text",replyMessage);
                              break;
                             */

                        }
                        break;
                    case WebhookEventTypeEnum.Unsend:
                        Trace.WriteLine($"使用者{eventObject.Source.UserId}在聊天室收回訊息！");
                        break;
                    case WebhookEventTypeEnum.Follow:
                        Trace.WriteLine($"使用者{eventObject.Source.UserId}將我們新增為好友！");
                        break;
                    case WebhookEventTypeEnum.Unfollow:
                        Trace.WriteLine($"使用者{eventObject.Source.UserId}封鎖了我們！");
                        break;
                    case WebhookEventTypeEnum.Join:
                        Trace.WriteLine("我們被邀請進入聊天室了！");
                        break;
                    case WebhookEventTypeEnum.Leave:
                        Trace.WriteLine("我們被聊天室踢出了");
                        break;
                    case WebhookEventTypeEnum.MemberJoined:
                        string joinedMemberIds = "";
                        foreach (var member in eventObject.Joined.Members)
                        {
                            joinedMemberIds += $"{member.UserId} ";
                        }
                        Trace.WriteLine($"使用者{joinedMemberIds}加入了群組！");
                        break;
                    case WebhookEventTypeEnum.MemberLeft:
                        string leftMemberIds = "";
                        foreach (var member in eventObject.Left.Members)
                        {
                            leftMemberIds += $"{member.UserId} ";
                        }
                        Trace.WriteLine($"使用者{leftMemberIds}離開了群組！");
                        break;
                    case WebhookEventTypeEnum.Postback:
                        if (eventObject.Postback.Params != null)
                        {
                            Trace.WriteLine($"使用者{eventObject.Source.UserId}觸發了postback事件:"+ eventObject.Postback.Data+"contant:" + eventObject.Postback.Params.Date.ToString());

                        }

                       

                        break;
                    case WebhookEventTypeEnum.VideoPlayComplete:
                        replyMessage = new ReplyMessageRequestDto<TextMessageDto>()
                        {
                            ReplyToken = eventObject.ReplyToken,
                            Messages = new List<TextMessageDto>
                            {
                                new TextMessageDto(){Text = $"使用者您好，謝謝您收看我們的宣傳影片，祝您身體健康萬事如意 !"}
                            }
                        };
                        ReplyMessageHandler(replyMessage);
                        break;
                }
            }
        }

        public class bodyGoInfo
        { 
          public string userID { get; set; }
          public string name { get; set; }
          public string birthDay { get; set; }
          public string height { get; set; }
          public string weight { get; set; }
        }

        public List<bodyGoInfo> usersBodyGoInfo=new List<bodyGoInfo>();
        private static bodyGoInfo currentBodyGoInfo;
        private void ReceiveMessageWebhookEvent(WebhookEventDto eventDto)
        {
            dynamic replyMessage = new ReplyMessageRequestDto<BaseMessageDto>();

            switch (eventDto.Message.Type)
            {
                // 收到文字訊息
                case MessageTypeEnum.Text:
                    // 訊息內容等於 "測試" 時

                    /* if (eventDto.Message.Text == "我要測試體試能評估")
                     {
                         replyMessage = new ReplyMessageRequestDto<TextMessageDto>
                         {
                             ReplyToken = eventDto.ReplyToken,
                             Messages = new List<TextMessageDto>
                             {
                                  new TextMessageDto
                                 {
                                       Text = "個人資料登入",
                                     QuickReply = new QuickReplyItemDto
                                     {
                                       Items = new List<QuickReplyButtonDto>
                                       {
                                        new QuickReplyButtonDto {
                                                 Action = new ActionDto {
                                                     Type = ActionTypeEnum.Postback,
                                                     Label = "受測者名稱" ,
                                                     Text = "我要測試體試能評估"
                                                 }
                                             },
                                       }
                                     }
                                 }
                             }
                         };
                     }
                    */
                    if (eventDto.Message.Text.Contains("身高:"))
                    {
                        if (currentBodyGoInfo != null)
                        {
                            if (currentBodyGoInfo.userID == eventDto.Source.UserId)
                            {
                                string[]s= eventDto.Message.Text.Split(":");
                                currentBodyGoInfo.height=s[1].Replace("\n體重", "");
                                currentBodyGoInfo.weight = s[2];
                            }
                        }

                        // 回覆文字訊息並挾帶 quick reply button
                        replyMessage = new ReplyMessageRequestDto<TextMessageDto>
                        {
                            ReplyToken = eventDto.ReplyToken,
                            Messages = new List<TextMessageDto>
                            {
                                new TextMessageDto
                                {
                                    Text = "BodyGo-進行個人資料填寫",
                                    QuickReply = new QuickReplyItemDto
                                    {
                                        Items = new List<QuickReplyButtonDto>
                                        {

                                           new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                Type = ActionTypeEnum.DatetimePicker,
                                                Label = "日期時間選擇",
                                                    Data = "BodyGo_User_Info_BirthDay",
                                                    Mode = DatetimePickerModeEnum.Date,
                                                    /*Initial = "1966-09-30T19:00",
                                                    Max = "1920-12-31T23:59",
                                                    Min = "2018-01-01T00:00"*/
                                                }
                                            },

                                        }
                                    }
                                }
                            }
                        };
                    }
                    if (eventDto.Message.Text.Contains("名稱:"))
                    {
                        if (currentBodyGoInfo != null)
                        {
                            if (currentBodyGoInfo.userID == eventDto.Source.UserId)
                            {
                                currentBodyGoInfo.name = eventDto.Message.Text.Replace("名稱:","") ;
                            }
                        }
                       
                        // 回覆文字訊息並挾帶 quick reply button
                        replyMessage = new ReplyMessageRequestDto<TextMessageDto>
                        {
                            ReplyToken = eventDto.ReplyToken,
                            Messages = new List<TextMessageDto>
                            {
                                new TextMessageDto
                                {
                                    Text = "BodyGo-進行個人資料填寫",
                                    QuickReply = new QuickReplyItemDto
                                    {
                                        Items = new List<QuickReplyButtonDto>
                                        {
                                          
                                           new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Postback,
                                                    Label = "受測者身體訊" ,
                                                    Data = "BodyGo_User_Info_BodyIndo" ,
                                                    DisplayText = "請填寫您的身高體重",
                                                    InputOption = PostbackInputOptionEnum.OpenKeyboard,
                                                    FillInText ="身高:\n體重:"
                                                }
                                            },
                                           

                                        }
                                    }
                                }
                            }
                        };
                    }



                    if (eventDto.Message.Text == "進行個人資料填寫" )
                    {
                        currentBodyGoInfo = new bodyGoInfo();
                        currentBodyGoInfo.userID = eventDto.Source.UserId;
                        // 回覆文字訊息並挾帶 quick reply button
                        replyMessage = new ReplyMessageRequestDto<TextMessageDto>
                        {
                            
                            ReplyToken = eventDto.ReplyToken,
                            Messages = new List<TextMessageDto>
                            {
                                new TextMessageDto
                                {
                                    Text = "BodyGo-進行個人資料填寫",
                                    QuickReply = new QuickReplyItemDto
                                    {
                                        Items = new List<QuickReplyButtonDto>
                                        {
                                            // message action
                                             new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Postback,
                                                    Label = "受測者名稱" ,
                                                    Data = "BodyGo_User_Info_Name" ,
                                                    DisplayText = "請填寫您在系統要顯系的名稱",
                                                    InputOption = PostbackInputOptionEnum.OpenKeyboard,
                                                    FillInText = "名稱:"
                                                  
                                                }
                                            },
                                            // uri action




                                        }
                                    }
                                }
                            }
                        };
                    }

                    if (eventDto.Message.Text.ToLower() == "hello")
                    {
                        // 回覆文字訊息並挾帶 quick reply button
                        replyMessage = new ReplyMessageRequestDto<TextMessageDto>
                        {
                            ReplyToken = eventDto.ReplyToken,
                            Messages = new List<TextMessageDto>
                            {
                                new TextMessageDto
                                {
                                    Text = "BodyGo-體適能選單",
                                    QuickReply = new QuickReplyItemDto
                                    {
                                        Items = new List<QuickReplyButtonDto>
                                        {
                                            // message action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Message,
                                                    Label = "個人資料填寫" ,
                                                    Text = "進行個人資料填寫"
                                                }
                                            },
                                            // uri action
                                           new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Message,
                                                    Label = "檢測報告" ,
                                                    Text = "進行檢測報告"
                                                }
                                            }
                                            
                                          
                                           
                                           
                                        }
                                    }
                                }
                            }
                        };
                    }
                    if (eventDto.Message.Text == "測試")
                    {
                        // 回覆文字訊息並挾帶 quick reply button
                        replyMessage = new ReplyMessageRequestDto<TextMessageDto>
                        {
                            ReplyToken = eventDto.ReplyToken,
                            Messages = new List<TextMessageDto>
                            {
                                new TextMessageDto
                                {
                                    Text = "QuickReply 測試訊息",
                                    QuickReply = new QuickReplyItemDto
                                    {
                                        Items = new List<QuickReplyButtonDto>
                                        {
                                            // message action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Message,
                                                    Label = "message 測試" ,
                                                    Text = "我要測試體試能評估"
                                                }
                                            },
                                            // uri action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Uri,
                                                    Label = "uri 測試" ,
                                                    Uri = "https://www.appx.com.tw"
                                                }
                                            },
                                             // 使用 uri schema 分享 Line Bot 資訊
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Uri,
                                                    Label = "LIFF測試" ,
                                                    Uri = "https://liff.line.me/2005380189-ngy0lroK"
                                                }
                                            },
                                            // postback action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Postback,
                                                    Label = "postback 測試" ,
                                                    Data = "quick reply postback action" ,
                                                    DisplayText = "使用者傳送 displayTex，但不會有 Webhook event 產生。",
                                                    InputOption = PostbackInputOptionEnum.OpenKeyboard,
                                                    FillInText = "第一行\n第二行"
                                                }
                                            },
                                            // datetime picker action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                Type = ActionTypeEnum.DatetimePicker,
                                                Label = "日期時間選擇",
                                                    Data = "quick reply datetime picker action",
                                                    Mode = DatetimePickerModeEnum.Date,
                                                    /*Initial = "1966-09-30T19:00",
                                                    Max = "1920-12-31T23:59",
                                                    Min = "2018-01-01T00:00"*/
                                                }
                                            },
                                            // camera action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Camera,
                                                    Label = "開啟相機"
                                                }
                                            },
                                            // camera roll action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.CameraRoll,
                                                    Label = "開啟相簿"
                                                }
                                            },
                                            // location action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Location,
                                                    Label = "開啟位置"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        };
                    }
                    if (eventDto.Message.Text == "Sender")
                    {
                        replyMessage = new ReplyMessageRequestDto<TextMessageDto>
                        {
                            ReplyToken = eventDto.ReplyToken,
                            Messages = new List<TextMessageDto>
                            {
                                new TextMessageDto
                                {
                                    Text = "你好，我是客服人員 1號",
                                    Sender = new SenderDto
                                    {
                                        Name = "客服人員 1號",
                                        IconUrl = "https://b489-61-63-154-173.jp.ngrok.io/UploadFiles/man.png"
                                    }
                                },
                                new TextMessageDto
                                {
                                    Text = "你好，我是客服人員 2號",
                                    Sender = new SenderDto
                                    {
                                        Name = "客服人員 2號",
                                        IconUrl = "https://b489-61-63-154-173.jp.ngrok.io/UploadFiles/gamer.png"
                                    }
                                }
                            }
                        };
                    }
                    break;
            }

            ReplyMessageHandler(replyMessage);
        }
        /// <summary>
        /// 接收到廣播請求時，在將請求傳至 Line 前多一層處理，依據收到的 messageType 將 messages 轉換成正確的型別，這樣 Json 轉換時才能正確轉換。
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="requestBody"></param>
        public void BroadcastMessageHandler(string messageType, object requestBody)
        {
            string strBody = requestBody.ToString();
            dynamic messageRequest = new BroadcastMessageRequestDto<BaseMessageDto>();
            switch (messageType)
            {
                case MessageTypeEnum.Text:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<TextMessageDto>>(strBody);
                    break;

                case MessageTypeEnum.Sticker:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<StickerMessageDto>>(strBody);
                    break;

                case MessageTypeEnum.Image:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<ImageMessageDto>>(strBody);
                    break;

                case MessageTypeEnum.Video:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<VideoMessageDto>>(strBody);
                    break;

                case MessageTypeEnum.Audio:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<AudioMessageDto>>(strBody);
                    break;

                case MessageTypeEnum.Location:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<LocationMessageDto>>(strBody);
                    break;

                case MessageTypeEnum.Imagemap:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<ImagemapMessageDto>>(strBody);
                    break;
            }
            BroadcastMessage(messageRequest);

        }

        /// <summary>
        /// 將廣播訊息請求送到 Line
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        public async void BroadcastMessage<T>(BroadcastMessageRequestDto<T> request)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken); //帶入 channel access token
            var json = _jsonProvider.Serialize(request);
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(broadcastMessageUri),
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            Trace.WriteLine("發送廣播:"+json.ToString());
            var response = await client.SendAsync(requestMessage);
         

            var result = await response.Content.ReadAsStringAsync();
            //Trace.WriteLine(result);

        }

        /// <summary>
        /// 接收到回覆請求時，在將請求傳至 Line 前多一層處理(目前為預留)
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="requestBody"></param>
        public void ReplyMessageHandler<T>(ReplyMessageRequestDto<T> requestBody)
        {
            ReplyMessage(requestBody);
        }

        /// <summary>
        /// 將回覆訊息請求送到 Line
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        public async void ReplyMessage<T>(ReplyMessageRequestDto<T> request)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken); //帶入 channel access token
            var json = _jsonProvider.Serialize(request);
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(replyMessageUri),
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(requestMessage);
            Trace.WriteLine(await response.Content.ReadAsStringAsync());
        }



    }
}
