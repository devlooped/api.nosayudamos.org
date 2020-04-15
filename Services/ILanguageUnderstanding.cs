using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;

namespace NosAyudamos
{
    public interface ILanguageUnderstanding
    {
        Task<IEnumerable<string>> GetIntentsAsync(string text);
    }

    public class LanguageUnderstanding : ILanguageUnderstanding
    {
        private readonly IEnviroment enviroment;

        public LanguageUnderstanding(IEnviroment env)
        {
            enviroment = env;
        }

        public async Task<IEnumerable<string>> GetIntentsAsync(string text)
        {
            using var luisClient = CreateLuisClient();

            var requestOptions = new PredictionRequestOptions
            {
                PreferExternalEntities = true,
            };

            var predictionRequest = new PredictionRequest
            {
                Query = text,
                Options = requestOptions
            };

            var predictionResponse = await luisClient.Prediction.GetSlotPredictionAsync(
                Guid.Parse(enviroment.GetVariable("LuisAppId")),
                slotName: enviroment.GetVariable("LuisAppSlot"), 
                predictionRequest,
                verbose: true,
                showAllIntents: false,
                log: true).ConfigureAwait(false);

            return predictionResponse.Prediction.Intents.Keys;
        }

        private ILUISRuntimeClient CreateLuisClient()
        {
            var credentials = new ApiKeyServiceClientCredentials(
                enviroment.GetVariable("LuisSubscriptionKey"));

            return new LUISRuntimeClient(credentials, new System.Net.Http.DelegatingHandler[] { })
                {
                    Endpoint = enviroment.GetVariable("LuisEndpoint")
                };
        }
    }
}