using CrossCutting.Core.Contract.Configuration.DataClasses;
using Logic.Domain.GoogleSheetsManagement.Contract.DataClasses;

namespace Logic.Domain.GoogleSheetsManagement
{
    public class GoogleSheetsManagementConfiguration
    {
        [ConfigMap("Logic.Domain.GoogleSheetsManagement", "FlowType")]
        public virtual CodeFlowType FlowType { get; set; } = CodeFlowType.OAuth2;

        [ConfigMap("Logic.Domain.GoogleSheetsManagement", "Scope")]
        public virtual Scope Scope { get; set; } = Scope.Write;

        [ConfigMap("Logic.Domain.GoogleSheetsManagement.ApiKey", "ApiKey")]
        public virtual string ApiKey { get; set; }

        [ConfigMap("Logic.Domain.GoogleSheetsManagement.OAuth2", "ClientId")]
        public virtual string ClientId { get; set; }

        [ConfigMap("Logic.Domain.GoogleSheetsManagement.OAuth2", "ClientSecret")]
        public virtual string ClientSecret { get; set; }
    }
}