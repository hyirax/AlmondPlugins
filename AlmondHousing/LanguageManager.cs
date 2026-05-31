using System;
using System.Collections.Generic;
using System.IO;
using AlmondHousing.Util;
using Newtonsoft.Json;

namespace AlmondHousing
{
    public class LanguageManager
    {
        private Dictionary<string, Dictionary<string, string>> _allTranslations = new();
        private Dictionary<string, string> _currentTranslations = new();
        public string CurrentLanguage { get; private set; } = "zh";

        public void LoadAllLanguages(string pluginDirectory)
        {
            try
            {
                string json = EmbeddedResource.ReadAllText("AlmondHousing.loc.json",
                    Path.Combine(pluginDirectory, "loc.json"));
                if (!string.IsNullOrEmpty(json))
                {
                    _allTranslations = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json) ?? new();
                }
            }
            catch { }
        }

        public void SetLanguage(string langCode)
        {
            CurrentLanguage = langCode;
            if (_allTranslations.ContainsKey(langCode))
            {
                _currentTranslations = _allTranslations[langCode];
            }
            else
            {
                _allTranslations.TryGetValue("en", out _currentTranslations);
            }
        }

        public string GetText(string originalText)
        {
            if (_currentTranslations != null && _currentTranslations.ContainsKey(originalText))
            {
                return _currentTranslations[originalText];
            }
            return originalText;
        }
    }
}