using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartMirror.Data.Layout
{
    public class LayoutState
    {
        public event Action OnChange;
        private readonly Dictionary<Type, int> _components;

        public int LeftCount { get; private set; } = 4;
        public int MidCount { get; private set; } = 3;
        public int RightCount { get; private set; } = 10;

        public LayoutState()
        { 
            _components = new Dictionary<Type, int>();
        }

        public void AddComponents(List<Type> components)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (!_components.ContainsKey(components[i]))
                {
                    _components.Add(components[i], i);
                }
            }

            OnChange?.Invoke();
        }

        public void MoveComponentUp(Type component)
        {
            if (_components.ContainsKey(component))
            {
                _components[component] = _components[component] + 1;
            }

            OnChange?.Invoke();
        }

        public void MoveComponentDown(Type component)
        {
            if (_components.ContainsKey(component))
            {
                _components[component] = _components[component] - 1;
            }

            OnChange?.Invoke();
        }

        public void ResetComponent(Type component)
        {
            if (_components.ContainsKey(component))
            {
                _components[component] = 0;
            }

            OnChange?.Invoke();
        }

        public int GetComponentOrder(Type component)
{
            if (_components.ContainsKey(component))
{
                return _components[component];
            }

            return -1;
        }

        public IEnumerable<Type> GetOrderedComponents() => _components
            .OrderBy(p => p.Value)
            .Select(p => p.Key);

        public Dictionary<Type, int> GetComponents() => _components;

        public void RaiseLeftCount() 
        {
            LeftCount++;
            OnChange?.Invoke();
        }

        public void LowerLeftCount()
        {
            LeftCount--;
            OnChange?.Invoke();
        }

        public void RaiseMidCount()
        {
            MidCount++;
            OnChange?.Invoke();
        }

        public void LowerMidCount()
        {
            MidCount--;
            OnChange?.Invoke();
        }

        public void RaiseRightCount()
        {
            RightCount++;
            OnChange?.Invoke();
        }

        public void LowerRightCount()
        {
            RightCount--;
            OnChange?.Invoke();
        }
    }
}
