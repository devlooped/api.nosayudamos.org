﻿using System;
using System.Threading.Tasks;
using Merq;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using NosAyudamos.Events;
using NosAyudamos.Properties;
using Serilog;

namespace NosAyudamos.Functions
{
    class Text : IEventHandler<TextMessageReceived>
    {
        readonly ILogger log;
        readonly ISerializer serializer;
        readonly IPersonRepository repository;
        readonly ILanguageUnderstanding language;
        readonly IEventStream events;


        public Text(ILogger log, ISerializer serializer, IPersonRepository repository, ILanguageUnderstanding language, IEventStream events)
            => (this.log, this.serializer, this.repository, this.language, this.events) = (log, serializer, repository, language, events);

        [FunctionName("text")]
        public async Task RunAsync([EventGridTrigger] EventGridEvent e)
        {
            log.Information(e.Data.ToString());

            // TODO: validate Topic, Subject, EventType

            await HandleAsync(serializer.Deserialize<TextMessageReceived>(e.Data));
        }

        public async Task HandleAsync(TextMessageReceived message)
        {
            log.Verbose("{@Message:j}", message);

            // Person is still not registered, need to discover intent
            if (message.PersonId == null)
            {
                var intents = await language.GetIntentsAsync(message.Text);
                if (intents.ContainsKey("None"))
                {
                    // Can't figure out intent, ask specifically
                    events.Push(new DeadMessageReceived(message.From, message.To, message.Text) { When = message.When });
                    events.Push(new MessageSent(message.To, message.From, Strings.UI.UnknownIntent));
                }
                else if (intents.TryGetValue("help", out var helpIntent) &&
                    helpIntent.Score >= 0.85)
                {
                    // User wants to be a donee, we need the ID
                    events.Push(new MessageSent(message.To, message.From, Strings.UI.Donee.SendIdentifier));
                }
                else if (intents.TryGetValue("donate", out var donateIntent) &&
                    donateIntent.Score >= 0.85)
                {
                    events.Push(new MessageSent(message.To, message.From, Strings.UI.Donor.SendAmount));
                }
            }
            else
            {
                var person = await repository.GetAsync(message.PersonId);
                var intents = await language.GetIntentsAsync(message.Text);

                if (intents.ContainsKey("None"))
                {
                    if (person.Role == Role.Donee)
                        events.Push(new MessageSent(message.To, message.From, Strings.UI.UnknownIntent));
                    else
                        events.Push(new MessageSent(message.To, message.From, Strings.UI.Donor.SendAmount));
                }

                // TODO load worklow for person, run it.
            }
        }
    }
}
