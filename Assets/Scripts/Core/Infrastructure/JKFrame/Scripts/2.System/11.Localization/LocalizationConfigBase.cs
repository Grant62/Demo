using System;
using System.Collections.Generic;
using JKFrame;
using Sirenix.OdinInspector;

public abstract class LocalizationConfigSuperBase : ConfigBase { }

public abstract class LocalizationConfigBase<LanguageType> : LocalizationConfigSuperBase where LanguageType : Enum
{
    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.CollapsedFoldout)]
    public Dictionary<string, Dictionary<LanguageType, LocalizationDataBase>> config = new();

    public T GetContent<T>(string key, LanguageType languageType) where T : LocalizationDataBase
    {
        LocalizationDataBase content = null;
        if (config.TryGetValue(key, out Dictionary<LanguageType, LocalizationDataBase> dic))
        {
            dic.TryGetValue(languageType, out content);
        }

        return (T)content;
    }
}