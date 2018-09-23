using System;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace Microsoft.Bot.Sample.LuisBot
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-luis
    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {
        //CONSTANTS
        //Entity
        public const string Entity_Address = "Events.Address"; 
        public const string Entity_Name = "Events.Name";
        public const string Entity_PlaceName = "Events.PlaceName";
        public const string Events_PlaceType = "Events.PlaceType";
        public const string Entity_Type = "Events.Type";
        
        //Intents
        public const string Intent_Book = "Events.Book";
        public const string Intent_None = "None";
        
        public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"], 
            ConfigurationManager.AppSettings["LuisAPIKey"], 
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {  
        }
        
        [LuisIntent(Intent_Book)]
        public async Task BookIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        } 

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        private async Task ShowLuisResult(IDialogContext context, LuisResult result) 
        {
            // get recognized entities
            string entities = this.BotEntityRecognition(result);
            
            // round number
            string roundedScore =  result.Intents[0].Score != null ? (Math.Round(result.Intents[0].Score.Value, 2).ToString()) : "0";
            
            await context.PostAsync($"**Query**: {result.Query}, **Intent**: {result.Intents[0].Intent}, **Score**: {roundedScore}. **Entities**: {entities}");
            context.Wait(MessageReceived);
        }
        
        // Entities found in result
        public string BotEntityRecognition(LuisResult result)
        {
            StringBuilder entityResults = new StringBuilder();
        
            if(result.Entities.Count>0)
            {
                foreach (EntityRecommendation item in result.Entities)
                {
                    entityResults.Append(item.Type + "=" + item.Entity + ",");
                }
                // remove last comma
                entityResults.Remove(entityResults.Length - 1, 1);
            }
        
            return entityResults.ToString();
        }
    }
}