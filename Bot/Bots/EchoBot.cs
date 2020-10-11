// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Data.Sqlite;
using Module;



namespace Microsoft.BotBuilderSamples.Bots
{
   public  struct User
    {
        public string nickname;
        public int rate;
    }
    public class EchoBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            string id = turnContext.Activity.From.Id;
            string nickname = turnContext.Activity.From.Name;

            var reply = MessageFactory.Text("");



            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction() { Title = "Задание", Type = ActionTypes.ImBack, Value = "Задание"},
                    new CardAction() { Title = "Факты", Type = ActionTypes.ImBack, Value = "Факты" },
                    new CardAction() { Title = "Профиль", Type = ActionTypes.ImBack, Value = "Профиль" },
                    new CardAction() { Title = "Рейтинг", Type = ActionTypes.ImBack, Value = "Рейтинг" },
                },
            };

            bool flag = true;
            string ansforEq = "";
            switch (turnContext.Activity.Text)
            {
                case "/start":
                    // reply = MessageFactory.Text("Приветсвую, это чат-бот, с помощью которого ты можешь подтнять свои знания по разным предметам");
                    addUser(id, nickname);
                    reply = MessageFactory.Text(id + "\t" + nickname);
                    break;
                case "Задание":
                    string[] equationTask = Equation.SolveEquation(3, 4);
                    ansforEq = equationTask[1];
                    reply = MessageFactory.Text(equationTask[0] + " " + equationTask[1]);
                    reply.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                        {
                            new CardAction() { Title = equationTask[2], Type = ActionTypes.ImBack, Value = "0"},
                            new CardAction() { Title = equationTask[3], Type = ActionTypes.ImBack, Value = "1" },
                            new CardAction() { Title = equationTask[4], Type = ActionTypes.ImBack, Value = "2" },
                            new CardAction() { Title = equationTask[5], Type = ActionTypes.ImBack, Value = "3" },
                        },
                    };

                    flag = false;
                    break;
                case "0":
                case "1":
                case "2":
                case "3":
                    if(ansforEq.Equals(turnContext.Activity.Text))
                        reply = MessageFactory.Text("GG");
                    else
                        reply = MessageFactory.Text("FF");
                    break;
                case "Факты":
                    string fact = getFact();
                    reply = MessageFactory.Text(fact);
                    break;
                case "Профиль":
                    string rate = getRate(id);
                    reply = MessageFactory.Text("Имя:\t" + nickname +  "\tРейтинг:\t" + rate);
                    break;

                case "Рейтинг":
                    List<User> listUser = new List<User>();
                    listUser = getRatingOfUsers();
                    string ansRate = "";
                    foreach (User i in listUser)
                    {
                        ansRate += "Имя:\t" + i.nickname + "\tРейтинг:\t" + i.rate + "\r\n";
                    }
                    reply = MessageFactory.Text(ansRate);
                    break;
                    
                default:

                    break;
            }

            if (flag)
            {
                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = new List<CardAction>()
                {
                    new CardAction() { Title = "Задание", Type = ActionTypes.ImBack, Value = "Задание"},
                    new CardAction() { Title = "Факты", Type = ActionTypes.ImBack, Value = "Факты" },
                    new CardAction() { Title = "Профиль", Type = ActionTypes.ImBack, Value = "Профиль" },
                    new CardAction() { Title = "Рейтинг", Type = ActionTypes.ImBack, Value = "Рейтинг" },
                },
                };
            }
            else
                flag = true;
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }
       

        protected List<User> getRatingOfUsers()
        {
            List<User> rateArray = new List<User>();
            using (var connection = new SqliteConnection("Data Source=wwwroot/Database.sqlite"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                 SELECT *
                 FROM Users
                 ORDER BY rate DESC
                 ";
                //command.ExecuteNonQuery();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User user = new User();
                        user.nickname = reader.GetString(1);
                        user.rate = reader.GetInt32(2);
                        rateArray.Add(user);
                    }
                }
            }

            return rateArray;
        }
        protected string getFact()
        {
            string fact = " ";
            using (var connection = new SqliteConnection("Data Source=wwwroot/Database.sqlite"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                 SELECT text
                 FROM Facts
                 ORDER BY RANDOM() 
                 ";
                //command.ExecuteNonQuery();
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    fact = reader.GetString(0);

                }
            }
            return fact;
        }
        protected void addUser(string id, string nickname)
        {
            using (var connection = new SqliteConnection("Data Source=wwwroot/Database.sqlite"))
            {
                
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                 INSERT OR IGNORE INTO 'Users'('id', 'nickname', 'rate') VALUES (@id, @nickname, 0)
                 ";
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@nickname", nickname);
                command.ExecuteNonQuery();
                //System.Console.WriteLine("по идее добавили");
            }
        }

        protected string getRate(string id)
        {
            string rate = " ";
            using (var connection = new SqliteConnection("Data Source=wwwroot/Database.sqlite"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                 SELECT rate 
                 FROM Users
                 WHERE id = @id 
                 ";
                command.Parameters.AddWithValue("@id", id);
                //command.ExecuteNonQuery();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                       rate = reader.GetString(0);
                        
                    }
                }
            }
            return rate;
        }
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            /*var welcomeText = "Доброго времени суток, выберите сложность задачи, которую вы хотите решить";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }*/

            string id = turnContext.Activity.Recipient.Id;
            string nickname = turnContext.Activity.Recipient.Name;
            addUser(id, nickname);
            //TODO insert new user
            //System.Console.WriteLine("привет пользователь");
            //Попросим ввести ник, так будет понятнее смотреть рейтинг
            //тут текст первой задачки со всей херней...
            //потом выводим кнопки с ответами, один правильный
            //так повторяем еще раз(взависимости от того, сколько мы хотим начальных тестовых задачек)
            var reply = MessageFactory.Text("Приветсвую, это чат-бот, с помощью которого ты можешь подтнять свои знания по разным предметам");
         
            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction() { Title = "Задание", Type = ActionTypes.ImBack, Value = "Задание"},
                    new CardAction() { Title = "Факты", Type = ActionTypes.ImBack, Value = "Факты" },
                    new CardAction() { Title = "Профиль", Type = ActionTypes.ImBack, Value = "Профиль" },
                    new CardAction() { Title = "Рейтинг", Type = ActionTypes.ImBack, Value = "Рейтинг" },
                },
            };
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }
        private static async Task SendSuggestedActionsAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("Доброго времени суток, выберите сложность задачи, которую вы хотите решить");
            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction() { Title = "1", Type = ActionTypes.ImBack, Value = "1"},
                    new CardAction() { Title = "2", Type = ActionTypes.ImBack, Value = "2" },
                    new CardAction() { Title = "3", Type = ActionTypes.ImBack, Value = "3" },
                },
            };
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }
    }
}


