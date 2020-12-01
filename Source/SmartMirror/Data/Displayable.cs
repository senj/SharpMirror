using System;

namespace SmartMirror.Data
{
    public abstract class Displayable
    {
        public event Action OnChange;
        public bool Enabled;
        public bool ShowDetails;
        
        protected void RaiseOnChangeEvent()
        {
            OnChange?.Invoke();
        }

        public void SetEnabled(bool enabled)
        {
            Enabled = enabled;
            OnChange?.Invoke();
        }

        public void SetShowDetails(bool showDetails)
        {
            ShowDetails = showDetails;
            OnChange?.Invoke();
        }
    }
}