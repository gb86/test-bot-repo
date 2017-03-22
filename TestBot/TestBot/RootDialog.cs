using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace TestBot
{
    public class RootDialog : IDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(ProcessMessage);
        }

        public async Task ProcessMessage(IDialogContext context, IAwaitable<IMessageActivity> activity)
        {
            await context.PostAsync("Root dialog started!");
            context.Wait(ProcessMessage);
        }
    }
}