﻿namespace MoneyFox.Common.Extensions;

using System.Reflection;
using System.Resources;
using Core.Common.Helpers;
using Core.Resources;

[ContentProperty("Text")]
public class TranslateExtension : IMarkupExtension
{
    private static readonly Lazy<ResourceManager> ResMgr = new(() => new(baseName: typeof(Strings).FullName, assembly: typeof(Strings).GetTypeInfo().Assembly));

    public string? Text { get; set; }

    public object ProvideValue(IServiceProvider serviceProvider)
    {
        if (Text == null)
        {
            return string.Empty;
        }

        return ResMgr.Value.GetString(name: Text, culture: CultureHelper.CurrentCulture) ?? Text;
    }
}
