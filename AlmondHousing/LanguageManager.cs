using System;
using System.Collections.Generic;
using System.IO;
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
            string filePath = Path.Combine(pluginDirectory, "loc.json");
            if (File.Exists(filePath))
            {
                try
                {
                    string json = File.ReadAllText(filePath);
                    _allTranslations = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json) ?? new();
                }
                catch { } // 遇到错误静默忽略即可
            }
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