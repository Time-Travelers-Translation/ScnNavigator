using CrossCutting.Core.Contract.Configuration.DataClasses;

namespace Logic.Business.TimeTravelersManagement
{
    public class TimeTravelersManagementConfiguration
    {
        [ConfigMap("Logic.Business.TimeTravelersManagement", "Platform")]
        public virtual string Platform { get; set; }

        [ConfigMap("Logic.Business.TimeTravelersManagement", "OriginalPath")]
        public virtual string OriginalPath { get; set; }

        [ConfigMap("Logic.Business.TimeTravelersManagement", "PatchPath")]
        public virtual string PatchPath { get; set; }

        [ConfigMap("Logic.Business.TimeTravelersManagement", "PatchMapPath")]
        public virtual string PatchMapPath { get; set; }
    }
}