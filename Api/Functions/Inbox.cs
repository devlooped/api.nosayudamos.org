﻿using System;
using System.Threading.Tasks;
using Merq;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using NosAyudamos.Events;
using Serilog;

namespace NosAyudamos.Functions
{
    /// <summary>
    /// Initial handler of uncategorized incoming messages from event grid 
    /// callbacks into our azure function. Made testable by implementing 
    /// <see cref="IEventHandler{TEvent}"/>.
    /// </summary>
    class Inbox : IEventHandler<MessageReceived>
    {
        readonly ILogger log;
        readonly ISerializer serializer;
        readonly IPersonRepository repository;
        readonly IEventStream events;

        public Inbox(ILogger log, ISerializer serializer, IPersonRepository repository, IEventStream events)
            => (this.log, this.serializer, this.repository, this.events) = (log, serializer, repository, events);

        [FunctionName("inbox")]
        public async Task RunAsync([EventGridTrigger] EventGridEvent e)
        {
            log.Information(e.Data.ToString());

            // TODO: validate Topic, Subject, EventType

            await HandleAsync(serializer.Deserialize<MessageReceived>(e.Data));
        }

        public async Task HandleAsync(MessageReceived e)
        {
            log.Verbose("{@Message:j}", e);

            // Performs minimal discovery of existing person id (if any)
            // and whether it's a text or image message.

            var person = await repository.FindAsync(e.From);
            var id = person?.NationalId;

            if (Uri.TryCreate(e.Body, UriKind.Absolute, out var uri))
            {
                events.Push(new ImageMessageReceived(e.From, e.To, uri) { PersonId = id, When = e.When });
            }
            else
            {
                events.Push(new TextMessageReceived(e.From, e.To, e.Body) { PersonId = id, When = e.When });
            }
        }
    }
}