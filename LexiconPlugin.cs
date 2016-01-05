using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace Lexicon_Plugin
{
    [ApiVersion(1, 22)]
    public class LexiconPlugin : TerrariaPlugin
    {
        public class Config
        {
            public string[] LexiconedGroups;

            public bool ReplaceWithHurrDurr = true;

            public int HurrDurrChancePercentage = 10;

            public bool InsertRandomTypos = true;

            public int TypoChancePercentage = 15;

            public bool SayRandomPhrases = true;

            public int RandomPhrasesChancePercentage = 10;

            public string[] RandomPhrases;

            public bool ReplaceWords = true;

            public Dictionary<string, string> LexiconWords = new Dictionary<string, string>();

            public void Initialize()
            {
                Action<string, string> action = delegate (string s1, string s2)
                {
                    if (!this.LexiconWords.ContainsKey(s1))
                    {
                        this.LexiconWords.Add(s1.ToLower(), s2);
                    }
                };
                action("I", "me");
                action("Are", "r");
                action("stupid", "stoopid");
                action("you", "u");
                action("wtf", "OHH MY GOD");
                action("what", "wot");
                action("what's", "wots");
                action("what's", "wots");
                action("whats", "wots");
                action("hey", "haai");
                action("terraria", "minecraft");
                action("server", "surver");
                action("hello", "hullo");
                action("who", "wo");
                action("is", "iz");
                action("hows", "howiz");
                action("everything", "evryting");
                action("going", "gowin'");
                action("it", "dat");
                action("mate", "mayte");
                action("friend", "frend");
                action("where", "were");
                action("your", "ur");
                action("my", "ma");
                action("that", "dat");
                action("bro", "browski");
                action("do", "doo");
                action("omg", "WHAT THE FUCK");
                action("wrong", "right");
                action("happening", "happeniiin");
                action("didn't", "did");
                action("say", "sey");
                action("didnt", "did");
                action("thanks", "thenks");
                action("lol", "holy shit that was so fucking funny");
                action("dick", "8=========D");
                action("fag", "banana");
                action("faggot", "banana");
                action("bullshit", "butterflies and rainbows");
                action("shit", "poopy face tomato nose");
                action("just", "jusstt");
                action("happen", "heppen");
                this.RandomPhrases = new string[]
                {
                    "I like pie",
                    "Can I please have admin?",
                    "Sometimes I like to play with myself",
                    "I ate a rock and pooped out butterflies",
                    "I am actually a unicorn",
                    "I like boys",
                    "I like eating my hair",
                    "Sometimes when i'm alone I like to dress up in brown, lay on the floor and pretend i'm a potato",
                    "I eat babies",
                    "My penis is on fire",
                    "I like farting",
                    "Sometimes, I dream about cheese...",
                    "A balloon just flew out my ass",
                    "I'm a princess from Mars, and I eat rocks and poop trees",
                    "I love the smell of a baby's burning flesh in the morning"
                };
                this.LexiconedGroups = new string[]
                {
                    "guest",
                    "default"
                };
            }
        }

        public static LexiconPlugin.Config config = new LexiconPlugin.Config();

        public static bool[] BeingLexiconed = new bool[255];

        public override Version Version
        {
            get
            {
                return new Version(1, 0);

            }
        }

        public override string Author
        {
            get
            {
                return "Jewsus";
            }
        }

        public override string Name
        {
            get
            {
                return "Lexiconify";
            }
        }

        public override string Description
        {
            get
            {
                return "Lexicons the crap out of people";
            }
        }

        public LexiconPlugin(Main game)
            : base(game)
        {
            Order = 1;
        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("wordreplacement.lexiconify", new CommandDelegate(this.Lexiconify), new string[]
            {
                "lexiconify, lex"
            }));
            Commands.ChatCommands.Add(new Command("wordreplacement.reload", new CommandDelegate(this.Reload_Config), new string[]
            {
                "lexreload"
            }));
            ServerApi.Hooks.ServerChat.Register(this, new HookHandler<ServerChatEventArgs>(this.OnChat));
            LexiconPlugin.ReadConfig();
        }

        private void OnChat(ServerChatEventArgs e)
        {
            TSPlayer tSPlayer = TShock.Players[e.Who];
            if (!e.Text.StartsWith(TShock.Config.CommandSpecifier) && !tSPlayer.mute)
            {
                bool flag = false;
                for (int i = 0; i < LexiconPlugin.config.LexiconedGroups.Length; i++)
                {
                    if (LexiconPlugin.config.LexiconedGroups[i] == tSPlayer.Group.Name)
                    {
                        flag = true;
                    }
                }
                if ((tSPlayer.Group.HasPermission("wordreplacement.permalexiconify") && !tSPlayer.Group.HasPermission("*")) || LexiconPlugin.BeingLexiconed[tSPlayer.Index] || flag)
                {
                    e.Handled = true;
                    string text = LexiconClass.Lexiconify(e.Text);
                    if (!TShock.Config.EnableChatAboveHeads)
                    {
                        string text2 = string.Format(TShock.Config.ChatFormat, new object[]
                        {
                            tSPlayer.Group.Name,
                            tSPlayer.Group.Prefix,
                            tSPlayer.Name,
                            tSPlayer.Group.Suffix,
                            text
                        });
                        TShock.Utils.Broadcast(text2, tSPlayer.Group.R, tSPlayer.Group.G, tSPlayer.Group.B);
                    }
                    else
                    {
                        Player player = Main.player[e.Who];
                        string name = player.name;
                        player.name = string.Format(TShock.Config.ChatAboveHeadsFormat, new object[]
                        {
                            tSPlayer.Group.Name,
                            tSPlayer.Group.Prefix,
                            tSPlayer.Name,
                            tSPlayer.Group.Suffix
                        });
                        NetMessage.SendData(4, -1, -1, player.name, e.Who, 0f, 0f, 0f, 0);
                        player.name = name;
                        string text2 = text;
                        NetMessage.SendData(25, -1, e.Who, text2, e.Who, (float)tSPlayer.Group.R, (float)tSPlayer.Group.G, (float)tSPlayer.Group.B, 0);
                        NetMessage.SendData(4, -1, -1, name, e.Who, 0f, 0f, 0f, 0);
                        string text3 = string.Format("<{0}> {1}", string.Format(TShock.Config.ChatAboveHeadsFormat, new object[]
                        {
                            tSPlayer.Group.Name,
                            tSPlayer.Group.Prefix,
                            tSPlayer.Name,
                            tSPlayer.Group.Suffix
                        }), text2);
                        tSPlayer.SendMessage(text3, tSPlayer.Group.R, tSPlayer.Group.G, tSPlayer.Group.B);
                        TSPlayer.Server.SendMessage(text3, tSPlayer.Group.R, tSPlayer.Group.G, tSPlayer.Group.B);
                        Log.Info(string.Format("Broadcast: {0}", text3));
                    }
                }
            }
        }

        private void Lexiconify(CommandArgs args)
        {
            if (args.Parameters.Count != 1)
            {
                args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /lexiconify [player], /lex [player]");
            }
            else
            {
                List<TSPlayer> list = TShock.Utils.FindPlayer(args.Parameters[0]);
                if (list.Count == 0)
                {
                    args.Player.SendErrorMessage("Invalid player!");
                }
                else if (list.Count > 1)
                {
                    args.Player.SendErrorMessage(string.Format("More than one ({0}) player matched!", args.Parameters.Count));
                }
                else
                {
                    TSPlayer tSPlayer = list[0];
                    LexiconPlugin.BeingLexiconed[tSPlayer.Index] = !LexiconPlugin.BeingLexiconed[tSPlayer.Index];
                    args.Player.SendInfoMessage(string.Format("{0} is currently {1} words replaced", tSPlayer.Name, LexiconPlugin.BeingLexiconed[tSPlayer.Index] ? "having" : "not having"));
                }
            }
        }

        private static void CreateConfig()
        {
            string path = Path.Combine(TShock.SavePath, "LexiconConfig.json");
            try
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        LexiconPlugin.config = new LexiconPlugin.Config();
                        LexiconPlugin.config.Initialize();
                        string value = JsonConvert.SerializeObject(LexiconPlugin.config);
                        streamWriter.Write(value);
                    }
                    fileStream.Close();
                }
            }
            catch (Exception ex)
            {
                Log.ConsoleError(ex.Message);
                LexiconPlugin.config = new LexiconPlugin.Config();
            }
        }

        private static bool ReadConfig()
        {
            string path = Path.Combine(TShock.SavePath, "LexiconConfig.json");
            bool result;
            try
            {
                if (File.Exists(path))
                {
                    using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(fileStream))
                        {
                            string text = streamReader.ReadToEnd();
                            LexiconPlugin.config = JsonConvert.DeserializeObject<LexiconPlugin.Config>(text);
                            LexiconPlugin.config.HurrDurrChancePercentage = (int)MathHelper.Clamp((float)LexiconPlugin.config.HurrDurrChancePercentage, 0f, 100f);
                            LexiconPlugin.config.RandomPhrasesChancePercentage = (int)MathHelper.Clamp((float)LexiconPlugin.config.RandomPhrasesChancePercentage, 0f, 100f);
                            LexiconPlugin.config.TypoChancePercentage = (int)MathHelper.Clamp((float)LexiconPlugin.config.TypoChancePercentage, 0f, 100f);
                        }
                        fileStream.Close();
                    }
                    result = true;
                    return result;
                }
                Log.ConsoleError("LexiconPlugin config not found. Creating new one...");
                LexiconPlugin.CreateConfig();
                result = false;
                return result;
            }
            catch (Exception ex)
            {
                Log.ConsoleError(ex.Message);
            }
            result = false;
            return result;
        }

        private void Reload_Config(CommandArgs args)
        {
            if (LexiconPlugin.ReadConfig())
            {
                args.Player.SendMessage("LexiconPlugin config reloaded sucessfully.", Color.Green);
            }
        }
    }
}
