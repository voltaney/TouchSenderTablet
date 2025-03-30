﻿using Microsoft.Windows.ApplicationModel.Resources;

namespace TouchSenderTablet.GUI.Helpers;

public static class ResourceExtensions
{
    private static readonly ResourceLoader s_resourceLoader = new();

    public static string GetLocalized(this string resourceKey) => s_resourceLoader.GetString(resourceKey);
}
