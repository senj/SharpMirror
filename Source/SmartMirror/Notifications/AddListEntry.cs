using SmartMirror.Extensions;
using System.Collections.Generic;

namespace SmartMirror.Notifications
{
    public class AddListEntry
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