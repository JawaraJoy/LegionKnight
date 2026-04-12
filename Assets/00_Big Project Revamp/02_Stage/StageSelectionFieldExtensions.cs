using System;

namespace Rush
{
    public static class StageSelectionFieldExtensions
    {
        public static StageSelectionField Find(
            this StageSelectionField[] array,
            Func<StageSelectionField, bool> predicate)
        {
            if (array == null) return null;
            foreach (var item in array)
                if (predicate(item)) return item;
            return null;
        }
    }
}
