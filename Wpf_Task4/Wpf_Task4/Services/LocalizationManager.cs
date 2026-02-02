using System.Windows;

namespace Wpf_Task4.Services;

public static class LocalizationManager
{
    public static string GetString(string key)
    {
        if (Application.Current.Resources.Contains(key))
        {
            return Application.Current.Resources[key] as string ?? key;
        }
        return key;
    }
}