using System;
using System.Windows.Data;
using DoorPrize.Generic;

namespace DoorPrize.GUI
{
    public class EnumToStringConverter : AbstractConverter, IValueConverter
    {
        private Type enumType;

        public EnumToStringConverter()
        {
        }

        public EnumToStringConverter(Type enumType)
        {
            EnumType = enumType;
        }

        public Type EnumType
        {
            get
            {
                return enumType;
            }
            set
            {
                if (enumType == value)
                    return;

                if (value != null)
                {
                    var isEnum = (Nullable.GetUnderlyingType(value) ?? value).IsEnum;

                    if (!isEnum)
                    {
                        throw new ArgumentException(
                            "The \"EnumType\" property must be set to an Enum type!");
                    }
                }

                enumType = value;
            }
        }

        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            return EnumHelper.GetDescription(EnumType, value);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new InvalidOperationException(
                "The \"ConvertBack\" method is inoperative!");
        }
    }
}
