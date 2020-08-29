using MediatR;
using SmartMirror.Extensions;
using System.Collections.Generic;

namespace SmartMirror.Data.Speech
{
    public class AddListEntry : INotification
    {
        public AddListEntry(IDictionary<string, object> entities)
        {
            entities.TryGetValueAsStringArray("ToDo.TaskContent", out string[] taskContent);
            ItemNames = taskContent;
        }

        public string[] ItemNames { get; }

        public string Details { get; }
    }
}