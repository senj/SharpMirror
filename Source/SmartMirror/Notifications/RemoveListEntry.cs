using SmartMirror.Extensions;
using System.Collections.Generic;

namespace SmartMirror.Notifications
{
    public class RemoveListEntry
    {
        public RemoveListEntry(IDictionary<string, object> entities)
        {
            entities.TryGetValueAsStringArray("ToDo.TaskContent", out string[] taskContent);
            ItemNames = taskContent;
        }

        public string[] ItemNames { get; }
    }
}