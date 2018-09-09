using TaxCalculator.BusinessLogic.Common;

namespace TaxCalculator.BusinessLogic
{
    public class TaxCalculatorInputDataContract
    {
        public decimal Income { get; set; }
        public decimal Outcome { get; set; }
        public decimal Salaries { get; set; }
        
        public ECompanyType CompanyType { get; set; }
    }
}