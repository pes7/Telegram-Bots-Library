[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/pes7/TelegramBots/blob/master/LICENSE)
# TelegramBots
This <b>Library</b> of creating <b>Telegram Bots</b> provides you with easy way of developing your own Bot.<br>
You can easy add your commands, modules, etc. to Bot and don't thinking about how it works)<br>
You can create your own Modules and Commands from IModule and ISynkCommand and then add it in bot<br>
If you want to controll all aspects of bot you can inherit <b>BotBase</b> or <b>IBot</b> and use all pleasure of my realisations and add you own changes to it.<br>
In Addition, in project you can find <b>GuchiBot</b> - this is bot that have all examples of creating your own bot.<br>
# What this Library Can???
### Modules
This is main and independent part of Bot. What is it? It is a class that iclude some local storge and list of commands that u can include to bot easely.
### Async Threads
Yes, of corse this bot works async so u can add him on a lot of groups and don't worry about laggyng
### Simple Commands Add
This library provide u with ISynkCommand and SynkCommands interface and class, so if u wan't to add ur command u don't won't to think how it do, u just inherit this class or interface and add command to bot. Example in bottom. In addition you can controll Access to commands
### Named Input
What it is? It is a type of input commands to bot, for using it u need to give bot information of his short name like "guchi" and when u will create commands u need to give them names like "picture" after that u can invoce it by input in chat something like that "guchi picture". Very simple to use, isn't it?
### Optimisation
A huge work i did for optimisation bot for much commands, so you will never disappoint on it (0_O) ;)
### If u have some PROBLEMS with library
You can find me on discord https://discord.gg/VfhRvdW and ask about something
### Yes of corse I have problemm with discribing library and with good Documenty on it. If u can help write me pls. All examples you can find on <b>GuchiBot</b>
# Structure
### Pes7BotCrator - main Library
### GuchiBot - example
# Main:
## Creating a Bot:
``` c#
Bot = new Bot(
                key: "YOUR_KEY",
                name: "guchimuchibot",
                webmdir: "G:/WebServers/home/apirrrsseer.ru/www/List_down/video",
                gachiimage: "C:/Users/user/Desktop/GachiArch",
                modules: new List<IModule> {
                    new _2chModule(),
                    new SaveLoadModule(60,120),
                    new LikeDislikeModule("./like.bot"),
                    new VoteModule("./votes.bot","./voteslike.bot"),
                    new AnistarModule(),
                    new Statistic(),
                    new TRM()
                }
            );
```
## Main Interfaces:
``` c#
    public interface IBot
    {
        string Key { get; set; }
        string Name { get; set; }
        string NameString { get; set; }
        string UserNameOfCreator { get; set; }
        Random Rand { get; set; }
        Telegram.Bot.TelegramBotClient Client { get; set; }
        Thread TimeSynk { get; set; }
        WebHook WebHook { get; }
        List<Message> MessagesLast { get; set; }
        List<dynamic> MessagesQueue { get; set; }
        List<Command> ActionCommands { get; set; }
        List<UserM> ActiveUsers { get; set; }
        List<Exception> Exceptions { get; set; }
        List<IModule> Modules { get; set; }
        GList<SynkCommand> SynkCommands { get; set; }
        T GetModule<T>() where T : IModule;
        Action OnWebHoockUpdated { get; set; }
        int RunTime { get; set; }
        string TimeToString(int i);
        void setModulesParent();
    }
```
``` c#
    public interface IModule
    {
        string Name { get; set; }
        IBot Parent { get; set; }
        System.Type Type { get; set; }
        List<Thread> MainThreads { get; set; }
        void Start();
        void AbortThread();
    }
```
``` c#
    public interface ISynkCommand
    {
        TypeOfCommand Type { get; set; }
        TypeOfAccess TypeOfAccess { get; set; }
        List<string> CommandLine { get; set; }
        string CommandName { get; set; }
        Delegate doFunc { get; set; }
        string Description { get; set; }
    }
```
## Inherit of BotBase or IBot
You can Inherit BotBase to create your own Bot with indevidual parameters.
``` c#
public class Bot : BotBase
    {
        public List<dynamic> LastWebms { get; set; }

        public string WebmDir { get; set; }
        public string GachiImage { get; set; }
        public string PreViewDir { get; set; }

        public Bot(string key, string name, string webmdir = null, string gachiimage = null, string preViewDir = null, List<IModule> modules = null) :
            base(key, name, modules)
        {
            LastWebms = new List<dynamic>();
            WebmDir = webmdir;
            GachiImage = gachiimage;
            PreViewDir = preViewDir;
            GenerePreViewDir();
        }
    ...
```
# Commands:
## Creating new Commands:
We have Interface ISynkCommand and class SynkCommand.
We have some base types of Command:
``` c#
    public enum TypeOfCommand {
        Standart,
        Query,
        InlineQuery,
        AllwaysInWebHook,
        Service,
        Photo
    }
```
We have some base Type of Access to Commands:
```
    public enum TypeOfAccess{
        Public,
        Hide,
        Admin,
        Named
    }
```
### Standart
This is command that provide you with trigger of simple text message to bot or in chat with bot. Standart command creates with <b>Action<Message, IBot, List<ArgC>></b> parameter.<br>
### Photo
As a message message but it contains Image
### Query 
This is command that calls when bot reacive Query from buttons and etc. Query command creates with <b>Action<CallbackQuery, IBot></b> parameter.</br>
### InlineQuery
This command calls when reacive Query from inline buttons and etc. InlineQuery command creates with <b>Action<InlineQuery, IBot></b> parameter.</br>
### AllwaysInWebHook
This type of command uses allways when any Update entere in WebHook. AllwaysInWebHook command creates with <b>Action<Update, IBot, List<ArgC>></b> parameter.<br>
### Service
This type of command uses for Service Messages from Telegram. Service command creates with <b>Action<Update, IBot></b> parameter.<br>
## How to add commands to Bot
We have many different types of this action but we can select 3 mait types:
#### From Module
``` c#
    Bot.SynkCommands.Add(Bot.GetModule<LikeDislikeModule>().Command);
```
#### From object of class or class:
``` c#
   Bot.SynkCommands.Add(new SynkCommand(bt.ArgMessage, new List<string>()
    {
        "/testparam"
    },descr:"Will post message to @id and with text. Params: `-id` `-text`"));
```
#### From lambda function
``` c#
    Bot.SynkCommands.Add(new SynkCommand((Telegram.Bot.Types.Message ms, IBot parent, List<ArgC> args)=> {
                parent.Client.SendTextMessageAsync(ms.Chat.Id,"Слушаюсь, уже сплю...");
                Bot.GetModule<SaveLoadModule>().saveIt();
                Bot.Dispose();
                Application.Exit();
            }, new List<string>()
            {
                "_"
            },commandName:"спать", access:TypeOfAccess.Named, descr: "Бот ложиться спать."));
````
# Modules:
  ## Aim of Modules
  Module - this is individual structure of life in bot. Every module have acces to his parent - bot and their own space of memory. You can have access to every modul in Bot by use ```Bot.GetModule<ModuleName>()```.
  ## Create
  ``` c#
       public class SaveLoadModule : Module
      {
          public SaveLoadModule(int interV, int backupIntervalV) : base("SaveLoadModule", typeof(SaveLoadModule))
          {
              ...
          }
      }
  ```
  ## Add in Bot
  You can do it when you create bot.
  ``` c#
      modules: new List<IModule> {
                      new _2chModule(),
                      new SaveLoadModule(60,120),
                      new LikeDislikeModule("./like.bot"),
                      new VoteModule("./votes.bot","./voteslike.bot"),
                      new AnistarModule(),
                      new Statistic(),
                      new TRM()
                  }
  ```
# Commands:
  ## Creating:
  ``` c#
    public class Help : SynkCommand
      {
          public BotBase Parent { get; set; }
          public Help(BotBase bot) : base(Act, new List<string>() { "/help" },descr:"List of commands.") { Parent = bot; }
          public static void Act(Message re, IBot Parent, List<ArgC> args)
          {
              Parent.Client.SendTextMessageAsync(re.Chat.Id,$"This bot[{Parent.Name}] was created with pes7's Bot Creator.");
              string coms = "";
              foreach(SynkCommand sn in Parent.SynkCommands.Where(fn=>fn.Type==TypeOfCommand.Standart && fn.CommandLine.First() != "Default"))
              {
                  if(sn.Description != null)
                      coms += $"\n{sn.CommandLine.First()} - {sn.Description}";
                  else
                      coms += $"\n{sn.CommandLine.First()}";
              }
              Parent.Client.SendTextMessageAsync(re.Chat.Id, $"Commands: {coms}");
          }
      }
  ```
  ## Adding in Bot:
  ``` c#
    Bot.SynkCommands.Add(new Pes7BotCrator.Commands.Help(Bot));
  ```
# Built With:
<a href = "https://github.com/TelegramBots/telegram.bot">.NET Client for Telegram Bot API</a><br>
<a href = "https://github.com/zzzprojects/html-agility-pack">Html Agility Pack</a>
# Creator:
Nazar Ukolov - pes7
