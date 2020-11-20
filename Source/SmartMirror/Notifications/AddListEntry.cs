﻿using SmartMirror.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SmartMirror.Notifications
{
    public class AddListEntry
    {
        public AddListEntry(IDictionary<string, object> entities)
        {
            entities.TryGetValueAsStringArray("Bring.Items", out string[] taskContent);
            entities.TryGetValueAsStringArray("Bring.ListType", out string[] listType);
            ItemNames = taskContent;
            ListType = listType.FirstOrDefault();
        }

        public string[] ItemNames { get; }

        public string Details { get; }

        public string ListType { get; }
    }
}