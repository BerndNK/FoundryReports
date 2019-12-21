using System;
using Newtonsoft.Json;

namespace FoundryReports.Core.Products
{
    [Serializable]
    internal class MoldRequirement : IMoldRequirement
    {
        [JsonIgnore]
        public IMold? MoldInstance { get; set; }

        public string MoldName { get; set; }

        public IMold Mold => MoldInstance ?? new MoldDummy {Name = MoldName};

        public decimal CastingCellAmount { get; set; }
        
        public MoldRequirement(IMold forMold, decimal castingCellAmount)
        {
            MoldInstance = forMold;
            CastingCellAmount = castingCellAmount;
        }

        public MoldRequirement()
        {
        }
    }
}