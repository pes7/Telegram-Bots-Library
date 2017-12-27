[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/pes7/TelegramBots/blob/master/LICENSE)
# TelegramBots
This <b>Library</b> of creating <b>Telegram Bots</b> provides you with easy way of developing your own Bot.<br>
You can easy add your commands, modules, etc. to Bot and don't thinking about how it works)<br>
You can create your own Modules and Commands from IModule and ISynkCommand and then add it in bot<br>
If you want to controll all aspects of bot you can inherit <b>BotBase</b> or <b>IBot</b> and use all pleasure of my realisations and add you own changes to it.<br>
In Addition, in project you can find <b>GuchiBot</b> - this is bot that have all examples of creating your own bot.
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
        List<SynkCommand> SynkCommands { get; set; }
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
        List<string> CommandLine { get; set; }
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
### Standart
This is command that provide you with trigger of simple text message to bot or in chat with bot. Standart command creates with <b>Action<Message, IBot, List<ArgC>></b> parameter.<br>
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
    Bot.SynkCommands.Add(new SynkCommand(async (Telegram.Bot.Types.Message ms, IBot parent, List<ArgC> args) =>
    {
        try
        {
            Stream st = System.IO.File.Open("./previews/14637724531700.webm.jpg", FileMode.Open);
            await parent.GetModule<TRM>().SendTimeRelayPhotoAsynkAsync(parent.MessagesLast.First().Chat.Id, new FileToSend("fl.jpg", st), 10, "KEKUS");
        }
        catch { await parent.GetModule<TRM>().SendTimeRelayMessageAsynkAsync(ms.Chat.Id,"Error to send simple .jpg",10); }
    }, new List<string>() { "/kek" }));
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
# Description of Existing Modules:
  ## 2chModule
  ### Aim:
  The main aim of this module is to parse 2ch.hk and select from there .webm and .mp4 files.
  ### Types:
  ThBoard<br>
  Webm
  ## LikeDislikeModule
  ### Aim:
  Aim of this module so simple to describe and so hard to creating... This module provides you with ability of Like and Dislike some posts in your channel, chat, etc. This module have his own Query command in it.
  ### Types:
  Likes
  ### Use:
  Adding module command to bot
  ``` c#
    Bot.SynkCommands.Add(Bot.GetModule<LikeDislikeModule>().Command);
  ```
  Adding buttons to post
  ``` c#
    var keys = LikeDislikeModule.getKeyBoard(webm.Path);
    await Parent.Client.SendPhotoAsync(Parent.MessagesLast.Last().Chat.Id, new FileToSend(webm.Thumbnail), webm.Path, false, 0, keys);
  ```
  ## TRM [Time Relay Module]
  ### Aim:
  The main aim of this module is available creating AutoDeliting and Delayed posts.
  ### Types:
  TimeRelayMessage
  ### Use:
  ``` c#
    Stream st = System.IO.File.Open("./previews/14637724531700.webm.jpg", FileMode.Open);
    await parent.GetModule<TRM>().SendTimeRelayPhotoAsynkAsync(parent.MessagesLast.First().Chat.Id, new FileToSend("fl.jpg", st), 10, "image");
  ```
  ## VoteModule
  ### Aim:
  Creating Polls and share it on other chats.
  ### Types:
  Likes<br>
  Opros<br>
  VoteMessage
  ### Use:
  In Developing....
  ## AnistarModule
  ### Aim:
  It module parse anistar.me and select todays anime ;)
  ## SaveLoadModule
  ### Aim:
  Automatic saving and backuping binary files.
  ### Use:
  Firs of all, you need create new Save function in your module:
  ``` c#
    public void Save()
      {
          if (LLikes.Count > 0)
          {
              List<Likes> ls = LLikes;
              SaveLoadModule.SaveSomething(ls, FileNameLikes);
          }
          if (Opros.Count > 0)
          {
              List<Opros> op = Opros;
              SaveLoadModule.SaveSomething(op, FileNameVotes);
          }
      }
  ```
  Add your save func in Module
  ``` c#
    Bot.GetModule<SaveLoadModule>().SaveActions.Add(Bot.GetModule<VoteModule>().Save);
  ```
  ## Statistic:
  ### Aim:
  Create statistic of chat and command usage, etc.
# Other Classes:
  ## ArgC
  This class needs to parse arguments of command.
  ``` c#
    public class ArgC
    {
        public string Name { get; set; }
        public string Arg { get; set; }
        public ArgC(string name = null, string arg = null)
        {
            Name = name;
            Arg = arg;
        }
        public static List<ArgC> getArgs(string message){
          ...
        }
    }
  ```
  ## UserM
  This class needs to save Online Users of chat and Bot.
  ``` c#
    public string PhotoPath { get; set; } = null;
      public int MessageCount { get; set; }
      public UserM(User us, int i = 0) : base()
      {
          this.Username = us.Username;
          this.LastName = us.LastName;
          this.Id = us.Id;
          this.FirstName = us.FirstName;
          MessageCount = i;
      }
      public async Task<bool> DownloadImageToDirectory(IBot Parent, bool isOverride = false){
        ...
      }
      public static string nameGet(User us){
        ...
      }
      public static string usernameGet(User us){
        ...
      }
    }
  ```
# Built With:
<a href = "https://github.com/TelegramBots/telegram.bot">.NET Client for Telegram Bot API</a><br>
<a href = "https://github.com/zzzprojects/html-agility-pack">Html Agility Pack</a>
# Creator:
Nazar Ukolov - pes7
