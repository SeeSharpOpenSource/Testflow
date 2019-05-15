using System.Collections.Generic;

namespace Testflow.Usr.I18nUtil
{
    internal class I18NOptionComparer : IEqualityComparer<I18NOption>
    {
        public bool Equals(I18NOption elem1, I18NOption elem2)
        {
            return elem1.FirstLanguageFile.Equals(elem2.FirstLanguageFile) && elem1.SecondLanguageFile.Equals(elem2.SecondLanguageFile) &&
                elem1.FirstLanguage.Equals(elem2.FirstLanguage) && elem1.SecondLanguage.Equals(elem2.SecondLanguage);
        }

        public int GetHashCode(I18NOption option)
        {
            return option.ToString().GetHashCode();
        }
    }
}