using CrossCutting.Core.Contract.Configuration.DataClasses;

namespace Logic.Business.TranslationManagement
{
    public class TranslationManagementConfiguration
    {
        [ConfigMap("Logic.Business.TranslationManagement", "TranslationSheetId")]
        public virtual string SheetId { get; set; }
    }
}