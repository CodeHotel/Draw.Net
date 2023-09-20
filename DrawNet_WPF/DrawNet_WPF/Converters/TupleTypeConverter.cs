using System;
using System.ComponentModel;
using System.Globalization;

namespace DrawNet_WPF.Converters
{
    class TupleTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string str)
            {
                string[] parts = str.Split(',');

                if (parts.Length == 2 && int.TryParse(parts[0], out int item1) && int.TryParse(parts[1], out int item2))
                {
                    return Tuple.Create(item1, item2);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
