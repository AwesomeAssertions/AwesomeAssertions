using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions.Formatting;

namespace AwesomeAssertions.Execution;

/// <summary>
/// Represents a collection of data items that are associated with an <see cref="AssertionScope"/>.
/// </summary>
internal class ContextDataDictionary
{
    private readonly List<DataItem> items = [];

    public IDictionary<string, object> GetReportable()
    {
        return items.Where(item => item.Reportable).ToDictionary(item => item.Key, item => (object)item);
    }

    public string AsStringOrDefault(string key) =>
        items.SingleOrDefault(i => i.Key == key)?.ToString();

    public void Add(ContextDataDictionary contextDataDictionary)
    {
        foreach (DataItem item in contextDataDictionary.items)
        {
            Add(item.Clone());
        }
    }

    public void Add(DataItem item)
    {
        int existingItemIndex = items.FindIndex(i => i.Key == item.Key);
        if (existingItemIndex >= 0)
        {
            items[existingItemIndex] = item;
        }
        else
        {
            items.Add(item);
        }
    }

    public class DataItem(string key, object value, bool reportable, bool requiresFormatting)
    {
        public string Key { get; } = key;

        public object Value { get; } = value;

        public bool Reportable { get; } = reportable;

        public bool RequiresFormatting { get; } = requiresFormatting;

        public DataItem Clone()
        {
            object clone = Value is ICloneable2 cloneable ? cloneable.Clone() : Value;
            return new DataItem(Key, clone, Reportable, RequiresFormatting);
        }

        public override string ToString()
        {
            if (RequiresFormatting)
            {
                return Formatter.ToString(Value);
            }

            return Value?.ToString();
        }
    }
}
