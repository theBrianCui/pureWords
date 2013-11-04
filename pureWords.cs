//Import various C# things.
using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

//Import Procon things.
using PRoCon.Core;
using PRoCon.Core.Plugin;
using PRoCon.Core.Players;

namespace PRoConEvents
{
    public class pureWords : PRoConPluginAPI, IPRoConPluginInterface
    {

        //--------------------------------------
        //Class level variables.
        //--------------------------------------

        private bool pluginEnabled = false;
        private string keywordListString = "";
        private string[] keywordArray;
        private List<string> triggerArray = new List<string>();
        private List<string> responseArray = new List<string>();

        private string kickMessage = "";
        private string chatMessage = "";

        private string debugLevelString = "1";
        private int debugLevel = 1;

        private string logName = "";
        //private StreamWriter log = File.AppendText("pureWordsLog.txt");

        public pureWords()
        {
            //triggerArray[0] = "";
            //responseArray[0] = "";
        }

        public void processChat(string speaker, string message)
        {
            if (pluginEnabled)
            {
                toConsole(2, speaker + " just said: \"" + message + "\"");
                if (containsBadWords(message))
                {
                    toConsole(1, "Kicking " + speaker + " with message \"" + kickMessage + "\"");
                    kickPlayer(speaker);
                    toLog("[ACTION] " + speaker + " was kicked for saying \"" + message + "\"");
                    if (!String.IsNullOrEmpty(chatMessage))
                    {
                        String chatThis = chatMessage.Replace("[player]", speaker);
                        toConsole(2, "Chat: \"" + chatThis + "\"");
                        toChat(chatThis);
                    }
                }

            }
        }

        //--------------------------------------
        //Description settings
        //--------------------------------------

        public string GetPluginName()
        {
            return "pureWords";
        }

        public string GetPluginVersion()
        {
            return "0.7.4";
        }

        public string GetPluginAuthor()
        {
            return "Analytalica";
        }

        public string GetPluginWebsite()
        {
            return "purebattlefield.org";
        }

        public string GetPluginDescription()
        {
            return @"<p>pureWords is a word filter plugin that monitors server chat. 
It features a configurable 'bad word' detector that kicks players for 
saying certain words in the in-game chat (whether it be global, team, 
or squad), and customizable chat triggers that respond to player 
inquiries such as '!help' or '!info'. Timestamped kick actions by 
pureWords can be logged into a local text file.
</p>
<p>This plugin was developed by analytalica for PURE Battlefield.</p>
<p><big><b>Initial Setup:</b></big><br>
</p>
<ol>
  <li>Set the bad word list by separating individual keywords by
commas. For example, if I wanted to filter out 'bathtub',
'porch', and 'bottle', I would enter:<br>
    <i>bathtub,porch,bottle</i></li>
  <li>Set a kick message. This is seen by the kicked player in
Battlelog.</li>
  <li>Set an in-game warning message. This is seen by all other
players in the server. To mention the player's name, type [player] and
it will be replaced. For example, if a player named 'Draeger' was
kicked by pureWords, setting the message to<br>
    <i>[player] was kicked by pureWords</i><br>
would show up as<br>
    <i>Draeger was kicked by pureWords</i><br>
in the in-game chat. This feature can be disabled by leaving the field
blank.</li>
  <li>To enable logging, configure a file name (preferrably one
that ends in .txt) and relative path. For example,<br>
    <i>Logs/pureWords.txt<br>
    </i>will
write to a file 'pureWords.txt' in the 'Logs' folder.</li>
  <li>Set the debug level to '0' (suppresses all messages), '1'
(recommended), or '2' (useful for debugging).</li>
</ol>
<p>pureWords is case insensitive and matches whole words only (ignoring any punctuation),
e.g. a player will not be kicked for 'ass' if he says 'assassin'.
In the bad word list, leading and trailing spaces (as well as line breaks) are automatically removed,
so it is fine to use <i>bathtub   ,   porch ,bottle </i> in place of <i>bathtub,porch,bottle</i>.</p>


";
        }

        //--------------------------------------
        //Helper Functions
        //--------------------------------------

        public void toChat(String message)
        {
            this.ExecuteCommand("procon.protected.send", "admin.say", message, "all");
        }

        public void toConsole(int msgLevel, String message)
        {
            //a message with msgLevel 1 is more important than 2
            if (debugLevel >= msgLevel)
            {
                this.ExecuteCommand("procon.protected.pluginconsole.write", "pureWords: " + message);
            }
        }

        public void toLog(String logText)
        {
            if (!String.IsNullOrEmpty(logName))
            {
                using (StreamWriter writeFile = new StreamWriter(logName, true))
                {
                    writeFile.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " " + logText);
                }
                this.toConsole(2, "An event has been logged to " + logName + ".");
            }
        }
        //--------------------------------------
        //Handy pureWords Methods
        //--------------------------------------

        public Boolean containsBadWords(String chatMessage)
        {
            foreach (string kw in keywordArray)
            {
                if (Regex.IsMatch(chatMessage, "\\b" + kw + "\\b", RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public void kickPlayer(String playerName)
        {
            this.ExecuteCommand("procon.protected.send", "admin.kickPlayer", playerName, this.kickMessage);
        }

        //--------------------------------------
        //These methods run when Procon does what's on the label.

        public void OnPluginLoaded(string strHostName, string strPort, string strPRoConVersion)
        {
            // Depending on your plugin you will need different types of events. See other plugins for examples.
            this.RegisterEvents(this.GetType().Name, "OnPluginLoaded", "OnGlobalChat", "OnTeamChat", "OnSquadChat");
            //triggerArray
        }

        public override void OnGlobalChat(string speaker, string message)
        {
            processChat(speaker, message);
        }

        public override void OnTeamChat(string speaker, string message, int teamId)
        {
            processChat(speaker, message);
        }

        public override void OnSquadChat(string speaker, string message, int teamId, int squadId)
        {
            processChat(speaker, message);
        }

        public void OnPluginEnable()
        {
            this.pluginEnabled = true;
            this.toConsole(1, "pureWords Enabled!");
            string stringKeywordList = "";
            foreach (string keyword in keywordArray)
            {
                stringKeywordList += (keyword + ", ");
            }
            this.toConsole(2, "Keyword List: " + stringKeywordList);
            toLog("[STATUS] pureWords Enabled");
        }

        public void OnPluginDisable()
        {
            this.pluginEnabled = false;
            toLog("[STATUS] pureWords Disabled");
            this.toConsole(1, "pureWords Disabled!");
        }

        public List<CPluginVariable> GetDisplayPluginVariables()
        {
            List<CPluginVariable> lstReturn = new List<CPluginVariable>();
            lstReturn.Add(new CPluginVariable("Filter Settings|Bad Word List", typeof(string), keywordListString));
            lstReturn.Add(new CPluginVariable("Filter Settings|Kick Message", typeof(string), kickMessage));
            lstReturn.Add(new CPluginVariable("Filter Settings|Chat Message", typeof(string), chatMessage));
            toConsole(2, "Begin initializing trigger array");
            for (int i = 0; i <= triggerArray.Count; i++)
            {
                toConsole(2, "Initializing " + i);
                lstReturn.Add(new CPluginVariable("Trigger Settings|Command " + i, typeof(string), triggerArray[i]));
            }
            toConsole(2, "Done initializing trigger array");
            lstReturn.Add(new CPluginVariable("Other|Log Path", typeof(string), logName));
            lstReturn.Add(new CPluginVariable("Other|Debug Level", typeof(string), debugLevelString));
            return lstReturn;
        }

        public List<CPluginVariable> GetPluginVariables()
        {
            return GetDisplayPluginVariables();
        }

        public void SetPluginVariable(String strVariable, String strValue)
        {
            if (Regex.Match(strVariable, @"Bad Word List").Success)
            {
                //keywordListString = strValue;
                string replaceWith = "";
                keywordListString = strValue.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith);
                keywordArray = keywordListString.Split(',');
                for (int i = 0; i < keywordArray.Length; i++)
                {
                    keywordArray[i] = keywordArray[i].Trim();
                }
            }
            else if (Regex.Match(strVariable, @"Kick Message").Success)
            {
                kickMessage = strValue;
            }
            else if (Regex.Match(strVariable, @"Chat Message").Success)
            {
                chatMessage = strValue;
            }
            else if (Regex.Match(strVariable, @"Log Path").Success)
            {
                logName = strValue;
                //StreamWriter log = File.AppendText(logName);
            }
            else if (Regex.Match(strVariable, @"Command").Success)
            {
                string replaceWith = "";
                string commandValueString = strVariable[strVariable.Length - 1].ToString();
                triggerArray[Int32.Parse(commandValueString)] = strVariable.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith).Trim();
            }
            else if (Regex.Match(strVariable, @"Debug Level").Success)
            {
                debugLevelString = strValue;
                try
                {
                    debugLevel = Int32.Parse(debugLevelString);
                }
                catch (Exception z)
                {
                    toConsole(1, "Invalid debug level! Choose 0, 1, or 2 only.");
                    debugLevel = 1;
                    debugLevelString = "1";
                }
            }
        }

    }
}