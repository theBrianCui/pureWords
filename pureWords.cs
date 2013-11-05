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

        private string kickMessage = "";
        private string chatMessage = "";

        //Operation Stupid Trigger Variables is a Go
        //I spent many hours trying to get an array to work with the display
        //variables in Procon. They didn't. This was my last resort.

        private string trigger1 = "";
        private string response1 = "";
        private string trigger2 = "";
        private string response2 = "";
        private string trigger3 = "";
        private string response3 = "";
        private string trigger4 = "";
        private string response4 = "";
        private string trigger5 = "";
        private string response5 = "";
        private string trigger6 = "";
        private string response6 = "";
        private string trigger7 = "";
        private string response7 = "";
        private string trigger8 = "";
        private string response8 = "";
        private string trigger9 = "";
        private string response9 = "";
        private string trigger10 = "";
        private string response10 = "";
        private string trigger11 = "";
        private string response11 = "";
        private string trigger12 = "";
        private string response12 = "";
        private string trigger13 = "";
        private string response13 = "";
        private string trigger14 = "";
        private string response14 = "";
        private string trigger15 = "";
        private string response15 = "";
        private string trigger16 = "";
        private string response16 = "";
        private string trigger17 = "";
        private string response17 = "";
        private string trigger18 = "";
        private string response18 = "";
        private string trigger19 = "";
        private string response19 = "";
        private string trigger20 = "";
        private string response20 = "";

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
            return "0.7.8";
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
            lstReturn.Add(new CPluginVariable("Main Settings|Log Path", typeof(string), logName));
            lstReturn.Add(new CPluginVariable("Main Settings|Debug Level", typeof(string), debugLevelString));
            lstReturn.Add(new CPluginVariable("Filter Settings|Bad Word List", typeof(string), keywordListString));
            lstReturn.Add(new CPluginVariable("Filter Settings|Kick Message", typeof(string), kickMessage));
            lstReturn.Add(new CPluginVariable("Filter Settings|Chat Message", typeof(string), chatMessage));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Trigger 1", typeof(string), trigger1));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Response 1", typeof(string), response1));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Trigger 2", typeof(string), trigger2));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Response 2", typeof(string), response2));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Trigger 3", typeof(string), trigger3));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Response 3", typeof(string), response3));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Trigger 4", typeof(string), trigger4));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Response 4", typeof(string), response4));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Trigger 5", typeof(string), trigger5));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Response 5", typeof(string), response5));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Trigger 6", typeof(string), trigger6));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Response 6", typeof(string), response6));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Trigger 7", typeof(string), trigger7));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Response 7", typeof(string), response7));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Trigger 8", typeof(string), trigger8));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Response 8", typeof(string), response8));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Trigger 9", typeof(string), trigger9));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Response 9", typeof(string), response9));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Trigger 10", typeof(string), trigger10));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Response 10", typeof(string), response10));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Trigger 11", typeof(string), trigger11));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Response 11", typeof(string), response11));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Trigger 12", typeof(string), trigger12));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Response 12", typeof(string), response12));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Trigger 13", typeof(string), trigger13));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Response 13", typeof(string), response13));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Trigger 14", typeof(string), trigger14));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Response 14", typeof(string), response14));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Trigger 15", typeof(string), trigger15));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Response 15", typeof(string), response15));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Trigger 16", typeof(string), trigger16));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Response 16", typeof(string), response16));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Trigger 17", typeof(string), trigger17));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Response 17", typeof(string), response17));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Trigger 18", typeof(string), trigger18));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Response 18", typeof(string), response18));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Trigger 19", typeof(string), trigger19));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Response 19", typeof(string), response19));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Trigger 20", typeof(string), trigger20));
            lstReturn.Add(new CPluginVariable("Trigger Settings|Response 20", typeof(string), response20));
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
                //Don't ask.
            else if (Regex.Match(strVariable, @"Trigger 1").Success) { trigger1 = strValue; }
            else if (Regex.Match(strVariable, @"Trigger 2").Success) { trigger2 = strValue; }
            else if (Regex.Match(strVariable, @"Trigger 3").Success) { trigger3 = strValue; }
            else if (Regex.Match(strVariable, @"Trigger 4").Success) { trigger4 = strValue; }
            else if (Regex.Match(strVariable, @"Trigger 5").Success) { trigger5 = strValue; }
            else if (Regex.Match(strVariable, @"Trigger 6").Success) { trigger6 = strValue; }
            else if (Regex.Match(strVariable, @"Trigger 7").Success) { trigger7 = strValue; }
            else if (Regex.Match(strVariable, @"Trigger 8").Success) { trigger8 = strValue; }
            else if (Regex.Match(strVariable, @"Trigger 9").Success) { trigger9 = strValue; }
            else if (Regex.Match(strVariable, @"Trigger 10").Success) { trigger10 = strValue; }
            else if (Regex.Match(strVariable, @"Trigger 11").Success) { trigger11 = strValue; }
            else if (Regex.Match(strVariable, @"Trigger 12").Success) { trigger12 = strValue; }
            else if (Regex.Match(strVariable, @"Trigger 13").Success) { trigger13 = strValue; }
            else if (Regex.Match(strVariable, @"Trigger 14").Success) { trigger14 = strValue; }
            else if (Regex.Match(strVariable, @"Trigger 15").Success) { trigger15 = strValue; }
            else if (Regex.Match(strVariable, @"Trigger 16").Success) { trigger16 = strValue; }
            else if (Regex.Match(strVariable, @"Trigger 17").Success) { trigger17 = strValue; }
            else if (Regex.Match(strVariable, @"Trigger 18").Success) { trigger18 = strValue; }
            else if (Regex.Match(strVariable, @"Trigger 19").Success) { trigger19 = strValue; }
            else if (Regex.Match(strVariable, @"Trigger 20").Success) { trigger20 = strValue; }
            else if (Regex.Match(strVariable, @"Response 1").Success) { response1 = strValue; }
            else if (Regex.Match(strVariable, @"Response 2").Success) { response2 = strValue; }
            else if (Regex.Match(strVariable, @"Response 3").Success) { response3 = strValue; }
            else if (Regex.Match(strVariable, @"Response 4").Success) { response4 = strValue; }
            else if (Regex.Match(strVariable, @"Response 5").Success) { response5 = strValue; }
            else if (Regex.Match(strVariable, @"Response 6").Success) { response6 = strValue; }
            else if (Regex.Match(strVariable, @"Response 7").Success) { response7 = strValue; }
            else if (Regex.Match(strVariable, @"Response 8").Success) { response8 = strValue; }
            else if (Regex.Match(strVariable, @"Response 9").Success) { response9 = strValue; }
            else if (Regex.Match(strVariable, @"Response 10").Success) { response10 = strValue; }
            else if (Regex.Match(strVariable, @"Response 11").Success) { response11 = strValue; }
            else if (Regex.Match(strVariable, @"Response 12").Success) { response12 = strValue; }
            else if (Regex.Match(strVariable, @"Response 13").Success) { response13 = strValue; }
            else if (Regex.Match(strVariable, @"Response 14").Success) { response14 = strValue; }
            else if (Regex.Match(strVariable, @"Response 15").Success) { response15 = strValue; }
            else if (Regex.Match(strVariable, @"Response 16").Success) { response16 = strValue; }
            else if (Regex.Match(strVariable, @"Response 17").Success) { response17 = strValue; }
            else if (Regex.Match(strVariable, @"Response 18").Success) { response18 = strValue; }
            else if (Regex.Match(strVariable, @"Response 19").Success) { response19 = strValue; }
            else if (Regex.Match(strVariable, @"Response 20").Success) { response20 = strValue; }

            else if (Regex.Match(strVariable, @"Log Path").Success)
            {
                logName = strValue;
                //StreamWriter log = File.AppendText(logName);
            }
            /*else if (Regex.Match(strVariable, @"Command").Success)
            {
                string replaceWith = "";
                string commandValueString = strVariable[strVariable.Length - 1].ToString();
                triggerArray[Int32.Parse(commandValueString)] = strVariable.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith).Trim();
            }*/
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