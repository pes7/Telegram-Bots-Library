# TelegramBots
This Bot Creating Libruary created with aim of doing creating Telegram Bot process more easely.<br>
It gives you abillity to easy add your commands and etc. to Bot and don't thinking about how it works)<br>
If you want to controll all aspects of bot you can inherit <b>BotBase</b> or <b>BotInterface</b> and use all pleasure of my realisations and add you own changes to it.<br>
In Addition, in project you can find GuchiBot - this is bot that have all examples of creating your own bot in my Bot Creating Libruary.
# Creating a Bot
## Creating 
``` c#
BotBase Bot = new BotBase("466088141:AAHIcb1aG8F6P5YQSgcQlqaKJBD9vlLuMAw",
    modules: new List<ModuleInterface> {
        new SaveLoadModule(60, LikePath, this)
        new LikeDislikeComponent()
    }
);
```
## Add some Commands in different ways
``` c#
Bot.Commands.Add(new LikeDislikeComponent().Command);
Bot.Commands.Add(new SynkCommand(new WebmModule().WebmFuncForBot, new List<string>()
{
    "/sendrandwebm@guchimuchibot",
    "/sendrandwebm"
}));
Bot.Commands.Add(new SynkCommand((Telegram.Bot.Types.Message ms, BotInteface bot, List<ArgC> args)=>
{
    string message = "";
    ArgC ag = args.Find(fs => fs.Name == "id");
    ArgC text = args.Find(fs => fs.Name == "text");
    if (ag != null && text != null)
    {
        message = $"@{ag.Arg} {text.Arg}";
    }
    bot.Client.SendTextMessageAsync(ms.Chat.Id,message);
}, new List<string>()
{
    "/testparam"
}));
```
# Creator:
Nazar Ukolov - pes7
