// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("Nothing done?");
           

            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction() { Title = "1", Type = ActionTypes.ImBack,Value = "1" },
                    new CardAction() { Title = "2", Type = ActionTypes.ImBack, Value = "2" },
                    new CardAction() { Title = "3", Type = ActionTypes.ImBack,Value = "3" },
                }
            };
            switch (turnContext.Activity.Text)
            {
                case "1":
                    break;
                default:
                    break;
            }
                await turnContext.SendActivityAsync(reply, cancellationToken);
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


