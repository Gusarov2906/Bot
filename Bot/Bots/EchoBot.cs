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

    public struct TaskStruct
    {
        public string text;
        public string ans;
    }
    public class EchoBot : ActivityHandler
    {
        public static string ansforEq = "";
        public static string ansforTs = "";
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
            int rate = 0;
            switch (turnContext.Activity.Text)
            {
                case "/start":
                    // reply = MessageFactory.Text("Приветсвую, это чат-бот, с помощью которого ты можешь подтнять свои знания по разным предметам");
                    addUser(id, nickname);
                    reply = MessageFactory.Text(id + "\t" + nickname);
                    break;
                case "Задание":
                    int tempRand = (new Random()).Next() % 3;
                    rate = getRate(id);
                    if (tempRand != 2)
                    {
                        string[] equationTask = Equation.SolveEquation(rateToWeight(rate), 4);
                        ansforEq = equationTask[1];
                        reply = MessageFactory.Text(equationTask[0]);
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
                    }
                    else
                    {
                        TaskStruct taskStruct = getTask(rateToWeight(getRate(id)));
                        ansforTs = taskStruct.ans;
                        reply = MessageFactory.Text("Введите численный ответ(если вещественное число, то через точку)\r\n" + taskStruct.text + "\t");                    
                    }    

                    flag = false;
                    break;
                case "0":
                case "1":
                case "2":
                case "3":
                    if (ansforEq.Equals(turnContext.Activity.Text))
                    {
                        setRate(id, (getRate(id) + 100));
                        reply = MessageFactory.Text("GG");
                    }
                    else
                    {
                        int tempRate = getRate(id);
                        if (tempRate >= 100)
                            setRate(id, (getRate(id) - 100));
                        reply = MessageFactory.Text("FF");
                    }
                    break;



                case "Факты":
                    string fact = getFact();
                    reply = MessageFactory.Text(fact);
                    break;
                case "Профиль":
                     rate = getRate(id);
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
                    if ((turnContext.Activity.Text).Equals(ansforTs.ToString()))
                    {
                        setRate(id, (getRate(id) + 100));
                        reply = MessageFactory.Text("GG");
                    }
                    else
                    {
                        int tempRate = getRate(id);
                        if (tempRate >= 100)
                            setRate(id, (getRate(id) - 100));
                        reply = MessageFactory.Text("FF");
                    }
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
       
        protected int rateToWeight(int rate)
        {
            if (rate > 1000)
                return 5;
            return rate / 200 + 1;
           
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


        protected TaskStruct getTask(int weight)
        {
            List<TaskStruct> taskList = new List<TaskStruct>();
            TaskStruct taskstruct;
            using (var connection = new SqliteConnection("Data Source=wwwroot/Database.sqlite"))
            {              
               
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                 SELECT * FROM Tasks ORDER BY RANDOM()
                 ";
                //command.Parameters.AddWithValue("@weight", weight);
                //command.ExecuteNonQuery();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        taskstruct.text = reader.GetString(1);
                        taskstruct.ans = reader.GetString(2);
                        taskList.Add(taskstruct);
                        // taskstruct.ans = 0f;
                    }
                }
            }
            return taskList[0];
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

        protected void setRate(string id, int rate)
        {
            using (var connection = new SqliteConnection("Data Source=wwwroot/Database.sqlite"))
            {

                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                 UPDATE 'Users' SET rate = @rate WHERE id = @id
                 ";
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@rate", rate);
                command.ExecuteNonQuery();
                rate = getRate(id);
                //System.Console.WriteLine("по идее добавили");
            }
        }
        protected int getRate(string id)
        {
            int rate = 0;
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
                       rate = reader.GetInt32(0);
                        
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


