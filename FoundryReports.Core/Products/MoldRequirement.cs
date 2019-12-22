using System;
using Newtonsoft.Json;

namespace FoundryReports.Core.Products
{
    [Serializable]
    internal class MoldRequirement : IMoldRequirement
    {
        /// <summary>
        /// Only the name of the mold is used for serialization. A GUID would be better, however is omitted due to the scope of this project
        /// </summary>
        public string MoldName { get; set; } = string.Empty;

        private IMold? _mold;

        [JsonIgnore]
        public IMold Mold
        {
            get => _mold ?? new MoldDummy {Name = MoldName};
            set
            {
                _mold = value;
                MoldName = _mold.Name;
            }
        }

        public decimal UsageAmount { get; set; }
        
        public MoldRequirement(IMold forMold, decimal castingCellAmount)
        {
            _mold = forMold;
            UsageAmount = castingCellAmount;
        }

        public MoldRequirement()
        {
        }
    }
}