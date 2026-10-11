using System.Collections.Generic;
using Lean.Localization;
using Source.Scripts.SaveSystem;

namespace Source.Scripts.Localization
{
    public class LanguageChanger
    {
        private const string Russian = "Russian";
        private const string English = "English";
        private const string Turkish = "Turkish";

        private readonly SaveData _saveData;
        private readonly Dictionary<LanguageType, string> _languages = new()
        {
            {LanguageType.English, English},
            {LanguageType.Turkish, Turkish},
            {LanguageType.Russian, Russian},
        };

        public LanguageChanger(SaveData saveData)
        {
            _saveData = saveData;
            SetLanguage(_saveData.SelectedLanguage);
        }

        public void SetLanguage(LanguageType type)
        {
            SetLanguage(_languages[type]);
        }

        private void SetLanguage(string language)
        {
            if (language == null)
                language = English;

            switch(language)
            {
                case English:
                case Russian:
                case Turkish:
                    LeanLocalization.SetCurrentLanguageAll(language);
                    LeanLocalization.UpdateTranslations();
                    _saveData.SelectedLanguage = language;
                    break;
                default:
                    LeanLocalization.SetCurrentLanguageAll(English);
                    LeanLocalization.UpdateTranslations();
                    _saveData.SelectedLanguage = English;
                    break;
            }
        }
    }
}