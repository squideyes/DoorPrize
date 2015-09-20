using System;
using System.Windows.Markup;

namespace DoorPrize.GUI
{
    public abstract class AbstractConverter : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
