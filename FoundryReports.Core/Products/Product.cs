using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace FoundryReports.Core.Products
{
    [Serializable]
    internal class Product : IProduct
    {
        public string Name { get; set; } = "New Product";

        [JsonIgnore]
        public IEnumerable<IMoldRequirement> MoldRequirements => MoldRequirementList;

        public IList<MoldRequirement> MoldRequirementList = new List<MoldRequirement>();

        public IMoldRequirement AddMoldRequirement()
        {
            var newMoldRequirement = new MoldRequirement();
            MoldRequirementList.Add(newMoldRequirement);
            return newMoldRequirement;
        }

        public void Remove(IMoldRequirement moldRequirement)
        {
            var existingElement = MoldRequirementList.FirstOrDefault(r => ReferenceEquals(r, moldRequirement));
            if(existingElement != null)
            {
                MoldRequirementList.Remove(existingElement);
            }
        }
    }
}