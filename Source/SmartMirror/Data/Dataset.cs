using System.Collections.Generic;

namespace SmartMirror.Data
{
    public class Dataset<T>
    {
        public string id { get; set; }

        public string label { get; set; }

        public List<T> data { get; set; }

        public string[] backgroundColor { get; set; }

        public string[] borderColor { get; set; }
                    
        public int borderWidth { get; set; }

        public bool fill { get; set; }
    }
}
