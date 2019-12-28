using FoundryReports.Core.Products;

namespace FoundryReports.Core.Source.Prediction
{
    internal class MoldUsageCounter
    {
        public IMold ForMold { get; }

        public decimal MaxUsages { get; }

        public decimal CurrentUsages { get; set; }

        public bool DidExceedMax => CurrentUsages > MaxUsages;

        public MoldUsageCounter(IMold forMold)
        {
            ForMold = forMold;
            MaxUsages = forMold.MaxUsages;
            CurrentUsages = forMold.CurrentUsages;
        }
    }
}