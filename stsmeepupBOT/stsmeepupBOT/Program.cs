using stsmeepupBOT.BDConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Timers;
using Timer = System.Threading.Timer;

namespace stsmeepupBOT
{
    internal class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient("6764016534:AAFaqr4u-pzFFa-Z_WgsmmL2Q2pmVcr5BGA");

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) 
        {
            
            try
            {
                if (update.Type == UpdateType.Message)
                {

                    if (update.Message.Chat.Type == ChatType.Private)
                    {

                        await HandleUpDateAdmin(botClient, update, cancellationToken);
                        return;

                    }
                    else if (update.Message.Chat.Type == ChatType.Group)
                    {
                        await HandleUpDateUser(botClient, update, cancellationToken);
                        return;

                    }


                }
                else if (update.Type == UpdateType.MyChatMember)
                {
                    var chatMember = update.MyChatMember;


                    if (chatMember.NewChatMember.Status == ChatMemberStatus.Member && chatMember.OldChatMember.Status != ChatMemberStatus.Member)
                    {



                        if (stsmeepupEntities.GetContext().chats.FirstOrDefault(x => x.chatId == chatMember.Chat.Id.ToString()) == null)
                        {
                            await botClient.SendTextMessageAsync(chatMember.Chat.Id, "Приветствую! \U0001F44B\nТеперь в этом чате будут приходить уведомления о предстоящих встреч. Что бы узнать о предстоящих встреч, напишите команду \"/meetings\"😲\n\nДо скорых встреч!✋", cancellationToken: cancellationToken);
                            chats newChat = new chats()
                            {
                                chatId = Convert.ToString(chatMember.Chat.Id),
                                nameChat = chatMember.Chat.Title,

                            };
                            stsmeepupEntities.GetContext().chats.Add(newChat);
                            stsmeepupEntities.GetContext().SaveChanges();
                            Console.WriteLine($"{DateTime.Now.TimeOfDay}| Группа {chatMember.Chat.Title} записана в БД");

                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(chatMember.Chat.Id, "Я вернулся. 🎉", cancellationToken: cancellationToken);
                            Console.WriteLine($"{DateTime.Now.TimeOfDay}| Бот добавлен в группу {chatMember.Chat.Title} {chatMember.Chat.Id}");


                        }


                        Console.WriteLine($"{DateTime.Now.TimeOfDay}| Бот добавлен в группу {chatMember.Chat.Title}");
                        return;

                    }
                    else if (chatMember.NewChatMember.Status == ChatMemberStatus.Kicked)
                    {
                        Console.WriteLine($"{DateTime.Now.TimeOfDay}| Бот кикнут из группы ");
                        return;
                    }


                }
                else if (update.Type == UpdateType.CallbackQuery)
                {
                    var callBack = update.CallbackQuery;
                    string[] callBackData = callBack.Data.Split(' ');
                    meetings meeting = stsmeepupEntities.GetContext().meetings.Find(int.Parse(callBackData[0]));
                    string typeCallBack = callBackData[1];
                    chats chat = stsmeepupEntities.GetContext().chats.FirstOrDefault(x => x.id_chat == meeting.id_chat);
                    admins admin = stsmeepupEntities.GetContext().admins.FirstOrDefault(x => x.username == update.CallbackQuery.From.Username);
                    if (stsmeepupEntities.GetContext().admins.Any(x => x.username == admin.username)
                        && stsmeepupEntities.GetContext().connectChatAdmin.Any(x => x.id_chat == chat.id_chat && x.id_admin == admin.id_admin))
                    {
                        if (typeCallBack == "editName")
                        {
                            admin.chatStatus = $"{meeting.id_meeting} editName";
                            await botClient.SendTextMessageAsync(callBack.Message.Chat.Id, "Отлично, введите новое назвние для этого события...", cancellationToken: cancellationToken);

                            return;
                        }
                        else if (typeCallBack == "editDesc")
                        {
                            admin.chatStatus = $"{meeting.id_meeting} editDesc";
                            await botClient.SendTextMessageAsync(callBack.Message.Chat.Id, "Хорошо, введите новое описание для этого события...", cancellationToken: cancellationToken);
                            return;
                        }
                        else if (typeCallBack == "editDate")
                        {
                            admin.chatStatus = $"{meeting.id_meeting} editDate";
                            await botClient.SendTextMessageAsync(callBack.Message.Chat.Id, "Замечательно, введите новую дату для этого события...\nВ формате \"dd.mm.yyyy hh:mm\"", cancellationToken: cancellationToken);
                            return;

                        }
                        else if (typeCallBack == "editNotify")
                        {
                            admin.chatStatus = $"{meeting.id_meeting} editNotify";
                            await botClient.SendTextMessageAsync(callBack.Message.Chat.Id, "Прекрасно, введите новое время за которое нужно сделать уведомление (в интервале от 30 до 5 минут)...", cancellationToken: cancellationToken);
                            return;
                        }
                        else if (typeCallBack == "cancel")
                        {
                            InlineKeyboardMarkup inlineKeyboardButton = new InlineKeyboardButton[][]
                           {
                                new InlineKeyboardButton[]
                                {
                                    InlineKeyboardButton.WithCallbackData("✅", callbackData: $"{meeting.id_meeting} yescancel"),
                                    InlineKeyboardButton.WithCallbackData("❌", callbackData: $"{meeting.id_meeting} nocancel")

                                }


                           };

                            await botClient.SendTextMessageAsync(callBack.Message.Chat.Id, text: "Вы уверенны что хотите отменить событие? Отредактировать или запустить заново больше не получиться!", replyMarkup: inlineKeyboardButton, cancellationToken: cancellationToken);
                            return;
                        }
                        else if (typeCallBack == "yescancel")
                        {
                            meeting.statusMeeting = 3;
                            await botClient.SendTextMessageAsync(callBack.Message.Chat.Id, $"Событие \"{meeting.titleMeeting}\" отменено ✅.");
                            await botClient.DeleteMessageAsync(callBack.Message.Chat.Id, callBack.Message.MessageId, cancellationToken: cancellationToken);
                            return;

                        }
                        else if (typeCallBack == "nocancel")
                        {
                            await botClient.SendTextMessageAsync(callBack.Message.Chat.Id, $"Отмена удаления.");
                            await botClient.DeleteMessageAsync(callBack.Message.Chat.Id, callBack.Message.MessageId, cancellationToken: cancellationToken);
                            return;

                        }
                        else if (typeCallBack == "editStatus")
                        {
                            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardButton[][]
                            {
                                new InlineKeyboardButton[]
                                {
                                    InlineKeyboardButton.WithCallbackData(text: "Начать событие✅", callbackData: $"{meeting.id_meeting} startMeeting"),
                                    InlineKeyboardButton.WithCallbackData(text: "Отменить❌", callbackData: $"{meeting.id_meeting} cancel")
                                },

                                new InlineKeyboardButton[]
                                {
                                    InlineKeyboardButton.WithCallbackData(text: "Завершить событие🛑", callbackData: $"{meeting.id_meeting} endMeeting")
                                }




                            };

                            await botClient.SendTextMessageAsync(callBack.Message.Chat.Id, "На какой статус поменять?", replyMarkup: inlineKeyboard, cancellationToken: cancellationToken);
                            return;

                        }
                        else if (typeCallBack == "startMeeting")
                        {
                            InlineKeyboardMarkup inlineKeyboardButton = new InlineKeyboardButton[][]
                            {
                                new InlineKeyboardButton[]
                                {
                                    InlineKeyboardButton.WithCallbackData("✅", callbackData: $"{meeting.id_meeting} yesstart"),
                                    InlineKeyboardButton.WithCallbackData("❌", callbackData: $"{meeting.id_meeting} nostart")

                                }


                            };

                            await botClient.SendTextMessageAsync(callBack.Message.Chat.Id, "Начать событие?", replyMarkup: inlineKeyboardButton, cancellationToken: cancellationToken);
                            return;

                        }
                        else if (typeCallBack == "yesstart")
                        {
                            try
                            {
                                chats chatMeeting = stsmeepupEntities.GetContext().chats.FirstOrDefault(x => x.id_chat == meeting.id_chat);
                                if (chatMeeting != null)
                                {
                                    if (meeting.statusMeeting == 1)
                                    {
                                        await botClient.DeleteMessageAsync(callBack.Message.Chat.Id, callBack.Message.MessageId, cancellationToken: cancellationToken);
                                        meeting.statusMeeting = 2;
                                        meeting.dateEventMeeting = DateTime.Now;



                                        await botClient.SendTextMessageAsync(chat.chatId, $"Началась встреча! ⏰\n\nНазвание: {meeting.titleMeeting}\nОписание: {meeting.descriptionMeeting}\nНачало встречи: {meeting.dateEventMeeting}", cancellationToken: cancellationToken);
                                        await botClient.SendTextMessageAsync(callBack.Message.Chat.Id, "Встреча запущена! ✅", cancellationToken: cancellationToken);

                                        stsmeepupEntities.GetContext().SaveChanges();
                                        return;

                                    }
                                    else
                                    {
                                        await botClient.SendTextMessageAsync(callBack.Message.Chat.Id, "Событие уже проходить, отменили или завершили. ", cancellationToken: cancellationToken);
                                        await botClient.DeleteMessageAsync(callBack.Message.Chat.Id, callBack.Message.MessageId, cancellationToken: cancellationToken);
                                        return;
                                    }


                                }
                                else
                                {
                                    await botClient.SendTextMessageAsync(callBack.Message.Chat.Id, "Ошибка. Чат не найден, попробуйте еще раз.", cancellationToken: cancellationToken);
                                    await botClient.DeleteMessageAsync(callBack.Message.Chat.Id, callBack.Message.MessageId, cancellationToken: cancellationToken);
                                    return;
                                }

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }


                        }
                        else if (typeCallBack == "nostart")
                        {
                            await botClient.SendTextMessageAsync(callBack.Message.Chat.Id, "Отлично. 🎉", cancellationToken: cancellationToken);
                            await botClient.DeleteMessageAsync(callBack.Message.Chat.Id, callBack.Message.MessageId, cancellationToken: cancellationToken);
                            return;
                        }
                        else if (typeCallBack == "endMeeting")
                        {
                            try 
                            {
                              InlineKeyboardMarkup inlineKeyboardButton = new InlineKeyboardButton[][]
                              {
                                new InlineKeyboardButton[]
                                {
                                    InlineKeyboardButton.WithCallbackData("✅", callbackData: $"{meeting.id_meeting} yesend"),
                                    InlineKeyboardButton.WithCallbackData("❌", callbackData: $"{meeting.id_meeting} noend")

                                }


                              };

                                await botClient.SendTextMessageAsync(callBack.Message.Chat.Id, text: "Вы уверенны что хотите завершить событие? Отредактировать или запустить заново больше не получиться!", replyMarkup: inlineKeyboardButton, cancellationToken: cancellationToken);
                                
                                return;

                            }
                            catch (Exception ex)
                            {

                                Console.WriteLine(ex);
                            }
                          

                           
                        }
                        else if (typeCallBack == "yesend")
                        {

                            meeting.statusMeeting = 4;
                            DateTime dateTimeStart = DateTime.Parse(meeting.dateEventMeeting.ToString("dd.MM.yyyy HH:mm"));
                            finishedMeetings finished = new finishedMeetings()
                            {
                                id_meeting = meeting.id_meeting,
                                duration = (int)Math.Round((DateTime.Now - dateTimeStart).TotalMinutes)

                            };

                            stsmeepupEntities.GetContext().finishedMeetings.Add(finished);
                            stsmeepupEntities.GetContext().SaveChanges();

                            InlineKeyboardMarkup inlineKeyboardMarkup = new InlineKeyboardButton[][]
                            {
                               new InlineKeyboardButton[]
                               {
                                  InlineKeyboardButton.WithCallbackData("Отметиться ✅", callbackData: $"{meeting.id_meeting} user")
                               }

                            };
                            chats meetingChat = stsmeepupEntities.GetContext().chats.FirstOrDefault(x => x.id_chat == meeting.id_chat);
                            await botClient.SendTextMessageAsync(meetingChat.chatId, text: "Были на встрече? Через пять минут, после окончания встречи, отметиться будет невозможно ⚠️", replyMarkup: inlineKeyboardMarkup, cancellationToken: cancellationToken);
                            await botClient.DeleteMessageAsync(callBack.Message.Chat.Id, callBack.Message.MessageId, cancellationToken: cancellationToken);
                            return;


                        }
                        else if (typeCallBack == "noend")
                        {
                            await botClient.SendTextMessageAsync(callBack.Message.Chat.Id, text: "Завершение отменено. ", cancellationToken: cancellationToken);
                            await botClient.DeleteMessageAsync(callBack.Message.Chat.Id, callBack.Message.MessageId, cancellationToken: cancellationToken);
                            return;

                        }
                        else if (typeCallBack == "user") 
                        {
                            DateTime dateTime = meeting.dateEventMeeting;
                            finishedMeetings finishedMeetings = stsmeepupEntities.GetContext().finishedMeetings.FirstOrDefault(x => x.id_meeting == meeting.id_meeting);
                            TimeSpan timeSpan = TimeSpan.FromMinutes(finishedMeetings.duration);
                            if (dateTime.AddMinutes(timeSpan.TotalMinutes).AddMinutes(5) <= DateTime.Now ) 
                            {
                                await botClient.DeleteMessageAsync(callBack.Message.Chat.Id,callBack.Message.MessageId, cancellationToken: cancellationToken);
                                return;
                            
                            }
                            var user = callBack.From;
                            if (stsmeepupEntities.GetContext().users.Any(x => x.userId == user.Id.ToString()))
                            {
                                users oldUser = stsmeepupEntities.GetContext().users.FirstOrDefault(x => x.userId == user.Id.ToString());
                                finishedMeetings fmeeting = stsmeepupEntities.GetContext().finishedMeetings.FirstOrDefault(x => x.id_meeting == meeting.id_meeting);
                                if (!stsmeepupEntities.GetContext().connectUsersFinishedMeetings.Any(x => x.id_user == oldUser.id_user && x.id_finishedMeeting == fmeeting.id_finishedMeeting))
                                {

                                    connectUsersFinishedMeetings connectUsers = new connectUsersFinishedMeetings()
                                    {
                                        id_finishedMeeting = fmeeting.id_finishedMeeting,
                                        id_user = oldUser.id_user

                                    };
                                    stsmeepupEntities.GetContext().connectUsersFinishedMeetings.Add(connectUsers);
                                    stsmeepupEntities.GetContext().SaveChanges();

                                    await botClient.AnswerCallbackQueryAsync(callBack.Id, "Вы отметились! ✅", showAlert: true, cancellationToken: cancellationToken);
                                    return;

                                }

                                await botClient.AnswerCallbackQueryAsync(callBack.Id, "Вы отметились! ✅", showAlert: true, cancellationToken: cancellationToken);
                                return;

                            }
                            else 
                            {
                                users newUser = new users()
                                {
                                    userId = user.Id.ToString(),
                                    username = user.Username,

                                };
                                stsmeepupEntities.GetContext().users.Add(newUser);
                                stsmeepupEntities.GetContext().SaveChanges();
                                finishedMeetings fmeeting = stsmeepupEntities.GetContext().finishedMeetings.FirstOrDefault(x => x.id_meeting == meeting.id_meeting);
                                connectUsersFinishedMeetings connectUsers = new connectUsersFinishedMeetings()
                                {
                                    id_finishedMeeting = fmeeting.id_finishedMeeting,
                                    id_user = newUser.id_user

                                };
                                stsmeepupEntities.GetContext().connectUsersFinishedMeetings.Add(connectUsers);
                                stsmeepupEntities.GetContext().SaveChanges();


                                await botClient.AnswerCallbackQueryAsync(callBack.Id, "Вы отметились! ✅", showAlert: true, cancellationToken: cancellationToken);
                                return;
                            }

                        }

                    }
                    else
                        return;
                }
                

            }
            catch { }
           
           

        
        }

        private static async Task HandleUpDateAdmin(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) 
        {
            var message = update.Message;
            Console.WriteLine($"{DateTime.Now.TimeOfDay}| New message, user: {message.From.Username}");

            if (!stsmeepupEntities.GetContext().admins.Any(x => x.chatid == message.Chat.Id.ToString()))
            {
                admins newAdmin = new admins()
                {
                    username = message.From.Username,
                    chatid = message.Chat.Id.ToString(),
                    chatStatus = "default"
                };

                stsmeepupEntities.GetContext().admins.Add(newAdmin);
                stsmeepupEntities.GetContext().SaveChanges();
                Console.WriteLine($"{DateTime.Now.TimeOfDay}| Администратор {message.From.Username} записан в БД");
            }

            admins thisAdmin = stsmeepupEntities.GetContext().admins.FirstOrDefault(x => x.chatid == message.Chat.Id.ToString());



            if (message.Text == "/start")
            {
                await botClient.SendTextMessageAsync(message.Chat, "Привет!✋\nВ данном чате можно администрировать встречи, если вас назначили быть администратором встречи.😪\nЧто бы узнать являетесь ли вы администратором какой нибудь встречи, напишите команду " +
                    "\"/adminstatus\".", cancellationToken: cancellationToken);
                string idChats = message.Chat.Id.ToString();
                admins chat = stsmeepupEntities.GetContext().admins.FirstOrDefault(x => x.chatid == idChats);
                chat.chatStatus = "default";
                return;

            }
            else if (message.Text == "/adminstatus")
            {
                string idChats = message.Chat.Id.ToString();
                admins chat = stsmeepupEntities.GetContext().admins.FirstOrDefault(x => x.chatid == idChats);
                chat.chatStatus = "default";
                stsmeepupEntities.GetContext().SaveChanges();
                try
                {

                    if (!stsmeepupEntities.GetContext().admins.Any(x => x.username == message.From.Username))
                        return;

                    admins admins = stsmeepupEntities.GetContext().admins.FirstOrDefault(x => x.username == message.From.Username);
                    var chatAdmins = stsmeepupEntities.GetContext().connectChatAdmin.Where(x => x.id_admin == admins.id_admin).Select(y => y.id_chat).ToList();
                    List<meetings> meetings = stsmeepupEntities.GetContext().meetings.Where(x => chatAdmins.Contains(x.id_chat) && x.statusMeeting != 3 && x.statusMeeting != 4).ToList();



                    if (meetings.Count == 0)
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "В данный момент вы не являетесь админстратором для какой либо встерчи. 🎉", cancellationToken: cancellationToken);
                        return;
                    }
                    else
                    {
                        string listgroup = "";
                        string listIdMeeting = "";
                        for (int i = 0; i < meetings.Count; i++)
                        {
                            int chatID = meetings[i].id_chat;
                            listgroup += $"{i + 1}.\t {meetings[i].titleMeeting}\nДата проведения: {meetings[i].dateEventMeeting}\nГруппа: {stsmeepupEntities.GetContext().chats.FirstOrDefault(x => x.id_chat == chatID)?.nameChat}\nОписание: {meetings[i].descriptionMeeting}\nУведомление перед началом: {meetings[i].notificationMeeting} мин.\n\n";
                            listIdMeeting += $" {meetings[i].id_meeting}";
                        }


                        await botClient.SendTextMessageAsync(message.Chat, $"Вы являетесь администратором следующих встреч:\n\n{listgroup}\n\nВы можете выбрать одну из встреч и отредактировать ее.", cancellationToken: cancellationToken);

                        admins.chatStatus = $"pickNumber{listIdMeeting}";
                        stsmeepupEntities.GetContext().SaveChanges();
                        return;
                    }
                    



                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }



            }
            else if (message.Text == "кто твой создатель?")
            {
                await botClient.SendTextMessageAsync(message.Chat, $"Привет, {message.From.FirstName}!✋\nМои создателем является - Никита Паймышев, он учится в КПОИиП на 3 курсе, он из группы И-32, у него рост 186 см., у него карие глаза.", cancellationToken: cancellationToken);
                return;

            }
            else if (thisAdmin.chatStatus.Split(' ')[1] == "editName")
            {
                int idMeeting = int.Parse(thisAdmin.chatStatus.Split(' ')[0]);
                meetings meeting = stsmeepupEntities.GetContext().meetings.FirstOrDefault(x => x.id_meeting == idMeeting);
                meeting.titleMeeting = message.Text;
                await botClient.SendTextMessageAsync(message.Chat.Id, "Название изменено ✅.", cancellationToken: cancellationToken);
                thisAdmin.chatStatus = "default";
                stsmeepupEntities.GetContext().SaveChanges();
                return;
            }
            else if (thisAdmin.chatStatus.Split(' ')[1] == "editDesc")
            {
                int idMeeting = int.Parse(thisAdmin.chatStatus.Split(' ')[0]);
                meetings meeting = stsmeepupEntities.GetContext().meetings.FirstOrDefault(x => x.id_meeting == idMeeting);
                meeting.descriptionMeeting = message.Text;
                await botClient.SendTextMessageAsync(message.Chat.Id, "Описание изменено ✅.", cancellationToken: cancellationToken);
                thisAdmin.chatStatus = "default";
                stsmeepupEntities.GetContext().SaveChanges();
                return;

            }
            else if (thisAdmin.chatStatus.Split(' ')[1] == "editDate")
            {
                string newDateTime = message.Text;
                if (DateTime.TryParse(newDateTime, out DateTime dateTime) && dateTime > DateTime.Now)
                {
                    int idMeeting = int.Parse(thisAdmin.chatStatus.Split(' ')[0]);
                    meetings meeting = stsmeepupEntities.GetContext().meetings.FirstOrDefault(x => x.id_meeting == idMeeting);
                    meeting.dateEventMeeting = dateTime;
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Дата изменена ✅.", cancellationToken: cancellationToken);
                    thisAdmin.chatStatus = "default";
                    stsmeepupEntities.GetContext().SaveChanges();
                    return;
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Неверная дата. Пропробуйте еще раз...", cancellationToken: cancellationToken);
                    return;
                }

            }
            else if (thisAdmin.chatStatus.Split(' ')[1] == "editNotify") 
            {
                string newNotify = message.Text;
                if (int.TryParse(newNotify, out int notify) && notify >= 5 && notify <= 30)
                {
                    int idMeeting = int.Parse(thisAdmin.chatStatus.Split(' ')[0]);
                    meetings meeting = stsmeepupEntities.GetContext().meetings.FirstOrDefault(x => x.id_meeting == idMeeting);
                    meeting.notificationMeeting = notify;
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Уведомление изменено ✅.", cancellationToken: cancellationToken);
                    thisAdmin.chatStatus = "default";
                    stsmeepupEntities.GetContext().SaveChanges();
                    return;
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Неверное число, введите чило интервале от 30 до 5 минут. Пропробуйте еще раз...", cancellationToken: cancellationToken);
                    return;
                }

            }
            else if (thisAdmin.chatStatus.Split(' ')[0] == "pickNumber")
            {
                try
                {
                    string[] listIdMeeting = new string[thisAdmin.chatStatus.Split(' ').Length - 1];
                    for (int i = 0; i < listIdMeeting.Length; i++)
                    {
                        listIdMeeting[i] = thisAdmin.chatStatus.Split(' ')[i + 1];
                    }


                    if (int.TryParse(message.Text, out int n) && n <= listIdMeeting.Length && n >= 0)
                    {
                        int idMeeting = int.Parse(listIdMeeting[n - 1]);
                        await EditMenuInAdmin(botClient, update, cancellationToken, stsmeepupEntities.GetContext().meetings.Find(idMeeting));
                    }
                    thisAdmin.chatStatus = "default";
                    stsmeepupEntities.GetContext().SaveChanges();
                    return;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }
           


        }
        private static async Task EditMenuInAdmin(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, meetings meeting )
        {
            try
            {
                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardButton[][]
                {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Поменять статус ✅", callbackData: $"{meeting.id_meeting} editStatus"),
                        InlineKeyboardButton.WithCallbackData(text: "Отменить ❌", callbackData: $"{meeting.id_meeting} cancel"),

                    },

                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Поменять название ✏️", callbackData: $"{meeting.id_meeting} editName")

                    },

                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Изм. описание 📄", callbackData: $"{meeting.id_meeting} editDesc"),
                        InlineKeyboardButton.WithCallbackData(text: "Изм. уведомление ⏰", callbackData: $"{meeting.id_meeting} editNotify"),
                        InlineKeyboardButton.WithCallbackData(text: "Изм. дату 🕘", callbackData: $"{meeting.id_meeting} editDate")

                    },
                     

                };

                Message message = await botClient.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: $"Редактирование события:\n\nНазвание: {meeting.titleMeeting},\nОписание: {meeting.descriptionMeeting},\nДата: {meeting.dateEventMeeting},\nСтатус: " +
                    $"{stsmeepupEntities.GetContext().typeStatusMeeting.FirstOrDefault(x => x.id_status == meeting.statusMeeting)?.nameStatus},\nУведомлять за {meeting.notificationMeeting} минут(-ы).",
                    replyMarkup: inlineKeyboard,
                    cancellationToken: cancellationToken);
                return;

            }
            catch
            {

            }

        }

        private static async Task HandleUpDateUser(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;
            Console.WriteLine($"{DateTime.Now.TimeOfDay}| New message, chat: {message.Chat.Id}");
            if (message.Text == "/meetings") 
            {
                string chatID = stsmeepupEntities.GetContext().chats.FirstOrDefault(x => x.chatId == message.Chat.Id.ToString())?.id_chat.ToString();
                string listMeeting = "";
                DateTime date = DateTime.Now;
                List<meetings> allMeetings = stsmeepupEntities.GetContext().meetings.Where(x => x.id_chat.ToString() == chatID && x.statusMeeting == 1).ToList();


                if (allMeetings.Count > 0)
                {
                    for (int i = 0; i < allMeetings.Count; i++)
                    {
                        listMeeting += $"{i + 1}. Наименование: {allMeetings[i].titleMeeting}\nОписание📄 : {allMeetings[i].descriptionMeeting}\n" +
                            $"Дата проведения🕘 : {allMeetings[i].dateEventMeeting}\nСтатус: {stsmeepupEntities.GetContext().typeStatusMeeting.Find(allMeetings[i].statusMeeting)?.nameStatus}\n\n";
                    }

                    string sendMessage = $"В группе {message.Chat.Title}, запланировано {allMeetings.Count} встреч(-и):\n\n{listMeeting}";

                    await botClient.SendTextMessageAsync(message.Chat.Id, sendMessage, cancellationToken: cancellationToken);
                    return;

                }
                else 
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "В ближайшее время, встреч не наблюдается. \U0001F389");
                    return;
                }

            }


        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception ex, CancellationToken cancellationToken) 
        {
            Console.WriteLine($"Произошла ошибка:\n{ex}");


        }

        static async Task ConsoleAdmin() 
        {
           

            string command = Console.ReadLine();
            string[] splitLine = command.Split(' ');

            var botClient = new TelegramBotClient("6764016534:AAFaqr4u-pzFFa-Z_WgsmmL2Q2pmVcr5BGA");

           
           

            switch (splitLine[0]) 
            {
                case "-help":
                    { Console.WriteLine("1. \"-adminadd\" - добавление нового администратора. Например -adminadd Username\n2. \"-deleteadmin\" - удаление администратора. Например -deleteadmin UserName" +
                        "\n3. \"-listadmin\" - выводить список достпных администраторов.\n4. \"-listgroup\" - выводить список доступных групп.\n5. \"-createconnect\" - создает администратора для группы. Например -createconnect UserName ChatID" +
                        "\n6. \"-listconnect\" - выводит назначенных администраторов.\n7. \"-deleteconnect\" - удаляет администратора группы. Например -deleteconnect IdConnect"); };
                    break;
                case "-adminadd": 
                    {

                        if (splitLine.Length != 2) 
                        {
                            Console.WriteLine("Введите username. Например: -adminadd username");
                            break;
                        }
                            
                        string adminId = splitLine[1];
                        

                        if (stsmeepupEntities.GetContext().admins.FirstOrDefault(x => x.username == adminId) != null)
                        {
                            Console.WriteLine($"Администратор {adminId} уже добавлен.");
                            break;
                        }
                        else 
                        {
                            admins newAdmin = new admins() 
                            { 
                                username = adminId,
 
                            };

                            stsmeepupEntities.GetContext().admins.Add(newAdmin);
                            stsmeepupEntities.GetContext().SaveChanges();
                            Console.WriteLine($"Администратор {adminId} добавлен.");
                            
                        }
                        
                    
                    };
                    break;
                case "-listadmin": 
                    {
                        var listAdmin = stsmeepupEntities.GetContext().admins.ToList();
                        Console.WriteLine("ID\tUser name\n");
                        foreach (admins item in listAdmin)
                        {
                            Console.WriteLine($"{item.id_admin}.\t@{item.username}");
                        }
                    
                    }
                    break;
                case "-deleteadmin": 
                    {
                        if (splitLine.Length != 2)
                        {
                            Console.WriteLine("Введите ID user. Например: -deleteadmin username");
                            break;
                        }

                        string adminId = splitLine[1];
                        if (stsmeepupEntities.GetContext().admins.FirstOrDefault(x => x.username == adminId)  == null)
                        {
                            Console.WriteLine($"Администратор {adminId} не найден.");
                            break;
                        }

                        admins admin = stsmeepupEntities.GetContext().admins.FirstOrDefault(x => x.username == adminId);
                        Console.WriteLine($"Если администратор {admin.username} является администратором какой либо группы,\nсвязь администратора с этой группой будет удалена. (Y/N)");
                        if (Console.ReadLine() == "Y")
                        {
                            List<connectChatAdmin> chatAdmins = stsmeepupEntities.GetContext().connectChatAdmin.Where(x => x.id_admin == admin.id_admin).ToList();
                            stsmeepupEntities.GetContext().connectChatAdmin.RemoveRange(chatAdmins);

                            stsmeepupEntities.GetContext().admins.Remove(admin);
                            stsmeepupEntities.GetContext().SaveChanges();
                            Console.WriteLine($"Администратор {adminId} удален.");

                        }
                        else
                            break;
                        

                    }
                    break;
                case "-createconnect": 
                    {
                        if (splitLine.Length != 3)
                        {
                            Console.WriteLine("Введите userID и chatID (что-бы узнать chatID, введите команду \"-listgroup\").\nНапример: -createconnect username chatID");
                            break;
                        }

                        string chatID = splitLine[2];
                        string adminID = splitLine[1];
                        if (stsmeepupEntities.GetContext().chats.FirstOrDefault(x => x.chatId == chatID) == null) 
                        {
                            Console.WriteLine($"CharID {chatID} не найден. Что-бы узнать chatID, введите команду \"-listgroup\"");
                            break;
                        }

                        if (stsmeepupEntities.GetContext().admins.FirstOrDefault(x => x.username == adminID) == null) 
                        {
                            Console.WriteLine($"Username {adminID} не найден. Что-бы узнать доступные usernames, введите команду \"-listadmin\"");
                            break;

                        }
                        chats chat = stsmeepupEntities.GetContext().chats.FirstOrDefault(x => x.chatId == chatID);
                        admins admin = stsmeepupEntities.GetContext().admins.FirstOrDefault(x => x.username == adminID);

                        if (stsmeepupEntities.GetContext().connectChatAdmin.FirstOrDefault(x => x.id_admin == admin.id_admin && x.id_chat == chat.id_chat) != null) 
                        {
                            Console.WriteLine($"Username {adminID} уже является администратором {chat.nameChat}.");
                            break;

                        }

                        connectChatAdmin connect = new connectChatAdmin() 
                        { 
                            id_admin = admin.id_admin,
                            id_chat= chat.id_chat

                        };

                        stsmeepupEntities.GetContext().connectChatAdmin.Add(connect);
                        stsmeepupEntities.GetContext().SaveChanges();
                        Console.WriteLine($"Администратор {admin.username} назначен администратором группы {chat.nameChat}.");

                    }
                    break;
                case "-listgroup": 
                    {
                        var listchats = stsmeepupEntities.GetContext().chats.ToList();
                        Console.WriteLine("chatID\t\tchatName\n");
                        foreach (chats item in listchats)
                        {
                            Console.WriteLine($"{item.chatId}\t@{item.nameChat}");
                        }

                    }
                    break;
                case "-listconnect": 
                    {
                        var listchats = stsmeepupEntities.GetContext().connectChatAdmin.ToList();
                        Console.WriteLine("IDConnect\tUserName\t\tNameChat + IDChat\n");
                        foreach (connectChatAdmin item in listchats)
                        {
                            Console.WriteLine($"{item.id_connect}.\t\t@{stsmeepupEntities.GetContext().admins.FirstOrDefault(x => x.id_admin == item.id_admin)?.username}" +
                                $"\t\t{stsmeepupEntities.GetContext().chats.FirstOrDefault(x => x.id_chat == item.id_chat)?.nameChat} {stsmeepupEntities.GetContext().chats.FirstOrDefault(x => x.id_chat == item.id_chat)?.chatId}");
                        }

                    }
                    break;
                case "-deleteconnect":
                    {
                        if (splitLine.Length != 2)
                        {
                            Console.WriteLine("Введите IDConnect (узнать нужный IDConnect \"-listconnect\"). Например: -deleteconnect IDConnect");
                            break;
                        }
                        int IDConnect = int.Parse(splitLine[1]);
                        connectChatAdmin connect = stsmeepupEntities.GetContext().connectChatAdmin.Find(IDConnect);
                        chats chat = stsmeepupEntities.GetContext().chats.FirstOrDefault(x => x.id_chat == connect.id_chat);
                        admins admin = stsmeepupEntities.GetContext().admins.FirstOrDefault(x => x.id_admin == connect.id_admin);
                        stsmeepupEntities.GetContext().connectChatAdmin.Remove(connect);
                        stsmeepupEntities.GetContext().SaveChanges();
                        Console.WriteLine($"Связь администратора {admin.username} с группой {chat.nameChat} удалена.");

                    }
                    break;
                case "-createmeeting": 
                    {
                        if (splitLine.Length != 2) 
                        {
                            Console.WriteLine($"Введите IDГруппы (узнать IDГруппы можно через команду \"-listgroup\"). Например -createmeeting IDГруппы.");
                            break;
                        }

                        
                        string idChat = splitLine[1];
                        chats chat = stsmeepupEntities.GetContext().chats.FirstOrDefault(x => x.chatId == idChat.ToString());

                        Console.Write("Name meeting: ");
                        string nameMeeting = Console.ReadLine();

                        Console.Write("Description meeting: ");
                        string descrMeeting = Console.ReadLine();

                        Console.Write("Notify in (Minutes) (в интервале от 30 до 5 минут):");
                        int notify = int.Parse(Console.ReadLine());
                        if(notify <= 5 && notify >= 30) 
                        {
                            Console.WriteLine("В интервале от 30 до 5 минут! Попробуйте еще раз");
                            await ConsoleAdmin();
                        }
                        
                        DateTime dateTime = DateTime.Now;
                        Console.Write("Date, time event (DD.MM.YYYY HH:MM):");
                        try
                        {
                            dateTime = DateTime.Parse(Console.ReadLine());
                        }
                        catch
                        {
                            Console.WriteLine("Не верный формат даты. Попробуйте еще раз.");
                            await ConsoleAdmin();
                        }
                        
                        if (dateTime.Date < DateTime.Now.Date)
                        {
                            Console.WriteLine("Ошибка. Дата проведения не может произойти в прошлом.\nПопробуйте еще раз.");
                            await ConsoleAdmin();
                        
                        }
                            

                        meetings meeting = new meetings() 
                        { 
                          id_chat = chat.id_chat,
                          titleMeeting = nameMeeting,
                          descriptionMeeting = descrMeeting,
                          dateCreateMeeting= DateTime.Now,
                          notificationMeeting = notify,
                          dateEventMeeting= dateTime,
                          statusMeeting = 1
                        
                        };

                        stsmeepupEntities.GetContext().meetings.Add(meeting);
                        stsmeepupEntities.GetContext().SaveChanges();
                        Console.WriteLine($"Событие {nameMeeting} создано.");

                        try
                        {
                            ChatId chatId = chat.chatId;
                            Console.WriteLine($"Отправка сообщения в чат {Convert.ToString(chat.chatId)}.");
                            await botClient.SendTextMessageAsync(chatId, $"Создана встреча! 😤\n\nНазвание встречи: {nameMeeting}.\nОписание: {descrMeeting}.\nДата проведения: {dateTime.ToString("dd.MM.yyyy HH:mm")}.😴", parseMode: ParseMode.Markdown);

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }


                    }
                    break;
                    



                default: Console.WriteLine($"Команды \"{command}\" не существует!\nСписок доступных команд: \"-help\"") ;
                    break;
            }

            await ConsoleAdmin();
        }


        private static async Task CheckNotify(object timerState) 
        {
            var botClient = new TelegramBotClient("6764016534:AAFaqr4u-pzFFa-Z_WgsmmL2Q2pmVcr5BGA");

            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday && DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                return;

            DateTime dateNow = DateTime.Now.Date;
            List<meetings> listMeeings = stsmeepupEntities.GetContext().meetings.Where(x => x.statusMeeting == 1 &&
                x.dateEventMeeting.Day == dateNow.Day &&
                x.dateEventMeeting.Month == dateNow.Month &&
                x.dateEventMeeting.Year == dateNow.Year).ToList();

           

                if (listMeeings != null)
                {
                    foreach (var item in listMeeings)
                    {
                        TimeSpan timeMeeting = item.dateEventMeeting.TimeOfDay;
                        TimeSpan timeNow = DateTime.Now.AddMinutes((double)item.notificationMeeting).TimeOfDay;
                        if (item.dateEventMeeting.Date == DateTime.Now.Date && timeMeeting.Hours == timeNow.Hours && timeMeeting.Minutes == timeNow.Minutes) 
                        {
                            var chat = stsmeepupEntities.GetContext().chats.FirstOrDefault(x => x.id_chat == item.id_chat);
                            if (chat != null)
                            {
                                await botClient.SendTextMessageAsync(chat.chatId, $"Уведомление ⚠️.\n\nЧерез {item.notificationMeeting} минут(-ы), начнется встреча {item.titleMeeting}. Дата проведения {item.dateEventMeeting}\n\nЧто бы узнать больше подробностей введите команду \"/meetings\". 👋", parseMode: ParseMode.Markdown);
                                Console.WriteLine($"{DateTime.Now.TimeOfDay}| Отправлено уведомление в чат");
                            }

                        }
                       
                    }
                }
            


        }

      
        static async Task Main(string[] args)
        {
            bot.StartReceiving(HandleUpdateAsync, HandleErrorAsync);
            

            Console.WriteLine($"{DateTime.Now.TimeOfDay}| Bot start");

           

            var timerState = new TimerState { Counter = 0 };
            Timer timer;
            timer = new Timer(
                callback: async state => await CheckNotify(state),
                state: timerState,
                dueTime: 10000,
                period: 60000);
            await ConsoleAdmin();
        }

        class TimerState
        {
            public int Counter;
        }
    }
}
