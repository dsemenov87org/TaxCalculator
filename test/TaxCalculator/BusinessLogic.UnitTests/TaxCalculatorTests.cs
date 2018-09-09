using System.Threading.Tasks;
using NUnit.Framework;
using TaxCalculator.BusinessLogic;
using TaxCalculator.BusinessLogic.Common;
using TaxCalculator.BusinessLogic.TaxSystems;

namespace TaxCalculator.Tests.BusinessLogic
{
    [TestFixture]
    public class TaxCalculatorTests
    {
        private readonly TaxCalculatorFactory _taxCalculatorFactory;

        private readonly SelfInsuranceFee _selfInsuranceFee;

        public TaxCalculatorTests()
        {
            var mockTaxAmountFetcher = new MockTaxAmountFetcher();

            _taxCalculatorFactory =
                new TaxCalculatorFactory(
                    mockTaxAmountFetcher, new MockTaxRateFetcher());

            _selfInsuranceFee =
                new SelfInsuranceFee(
                    mockTaxAmountFetcher.FetchTaxAmount(ETaxAmount.PfrSelfAmount).Result,
                    mockTaxAmountFetcher.FetchTaxAmount(ETaxAmount.FomsSelfAmount).Result);
        }

        [Test]
        public async Task ChooseIpUsnD_Then_CalculateWright()
        {
            // Assign
            var calculator =
                await _taxCalculatorFactory.CreateCalculator(
                    ECompanyType.IP,
                    EAccountTaxationSystem.Usn6);

            // Act
            var @params =
                new CustomerTaxParameters(
                    new RurMoney(2500000m),
                    RurMoney.Zero,
                    new RurMoney(30000m));

            var actual = await calculator(@params);

            // Assert
            var expected = new IndividualUsnDTaxAggregate(
                accruedSTS: new RurMoney(150000m),
                feeDeduction: new RurMoney(64729.83m), 
                feeTotal: new RurMoney(64729.83m),
                sts: new RurMoney(85270.17m),
                selfInsuranceFee: _selfInsuranceFee,
                employeeFee: new EmployeeInsuranceFee(
                    new RurMoney(7586.21m),
                    new RurMoney(1758.62m),
                    new RurMoney(1000m)),
                additionalFee: new RurMoney(22000m),
                employeeNdfl: new RurMoney(4483m),
                total: new RurMoney(154483m),
                burgen: 6.18m
                );

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task ChooseIpUsnDWithOutEmployees_Then_CalculateWright()
        {
            // Assign
            var calculator =
                await _taxCalculatorFactory.CreateCalculator(
                    ECompanyType.IP,
                    EAccountTaxationSystem.Usn6);

            // Act
            var @params =
                new CustomerTaxParameters(
                    new RurMoney(588000m),
                    RurMoney.Zero,
                    RurMoney.Zero);

            var actual = await calculator(@params);

            // Assert
            var expected = new IndividualUsnDTaxAggregate(
                accruedSTS: new RurMoney(35280m),
                feeDeduction: new RurMoney(35265m),
                feeTotal: new RurMoney(35265m),
                sts: new RurMoney(15m),
                selfInsuranceFee: _selfInsuranceFee,
                employeeFee: new EmployeeInsuranceFee(RurMoney.Zero, RurMoney.Zero, RurMoney.Zero),
                additionalFee: new RurMoney(2880m),
                employeeNdfl: RurMoney.Zero,
                total: new RurMoney(35280m),
                burgen: 6m
                );

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task ChooseIpUsnD_R_Then_CalculateWright()
        {
            // Assign
            var calculator =
                await _taxCalculatorFactory.CreateCalculator(
                    ECompanyType.IP,
                    EAccountTaxationSystem.Usn15);

            // Act
            var @params =
                new CustomerTaxParameters(
                    new RurMoney(1000000m),
                    new RurMoney(200000m),
                    new RurMoney(30000m));

            var actual = await calculator(@params);

            // Assert
            var expected = new IndividualUsnD_RTaxAggregate(
                taxExpenses: new RurMoney(277212.83m),
                sts: new RurMoney(108418.08m),
                selfInsuranceFee: _selfInsuranceFee,
                employeeFee: new EmployeeInsuranceFee(
                    new RurMoney(7586.21m),
                    new RurMoney(1758.62m),
                    new RurMoney(1000m)),
                additionalFee: new RurMoney(7000m),
                employeeNdfl: new RurMoney(4483m),
                feeTotal: new RurMoney(49729.83m),
                total: new RurMoney(162630.91m),
                burgen: 16.26m
                );

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async  Task ChooseIpUsnD_R_WithoutEmployees_Then_CalculateWright()
        {
            // Assign
            var calculator =
                await _taxCalculatorFactory.CreateCalculator(
                    ECompanyType.IP,
                    EAccountTaxationSystem.Usn15);

            // Act
            var @params =
                new CustomerTaxParameters(
                    new RurMoney(1000000m),
                    new RurMoney(200000m),
                    RurMoney.Zero);

            var actual = await calculator(@params);

            // Assert
            var expected = new IndividualUsnD_RTaxAggregate(
                taxExpenses: new RurMoney(232385m),
                sts: new RurMoney(115142.25m),
                selfInsuranceFee: _selfInsuranceFee,
                employeeFee: new EmployeeInsuranceFee(RurMoney.Zero, RurMoney.Zero, RurMoney.Zero),
                additionalFee: new RurMoney(7000m),
                employeeNdfl: RurMoney.Zero,
                feeTotal: new RurMoney(39385m),
                total: new RurMoney(154527.25m),
                burgen: 15.45m
                );

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task ChooseOrgUsnD_Then_CalculateWright()
        {
            // Assign
            var calculator =
                await _taxCalculatorFactory.CreateCalculator(
                    ECompanyType.OOO,
                    EAccountTaxationSystem.Usn6);

            // Act
            var @params =
                new CustomerTaxParameters(
                    new RurMoney(1000000m),
                    RurMoney.Zero,
                    new RurMoney(30000m));

            var actual = await calculator(@params);

            // Assert
            var expected = new OrgUsnDTaxAggregate(
                sts: new RurMoney(49655.17m),
                chargedTax: new RurMoney(60000m),
                feeDeduction: new RurMoney(10344.83m),
                employeeFee: new EmployeeInsuranceFee(
                    new RurMoney(7586.21m),
                    new RurMoney(1758.62m),
                    new RurMoney(1000m)),
                ndfl: new RurMoney(4483m),
                feeTotal: new RurMoney(10344.83m),
                total: new RurMoney(64483m),
                burgen: 6.45m
                );

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task ChooseOrgUsnD_WithoutEmployees_Then_CalculateWright()
        {
            // Assign
            var calculator =
                await _taxCalculatorFactory.CreateCalculator(
                    ECompanyType.OOO,
                    EAccountTaxationSystem.Usn6);

            // Act
            var @params =
                new CustomerTaxParameters(
                    new RurMoney(1000000m),
                    RurMoney.Zero,
                    RurMoney.Zero);

            var actual = await calculator(@params);

            // Assert
            var expected = new OrgUsnDTaxAggregate(
                sts: new RurMoney(60000m),
                chargedTax: new RurMoney(60000m),
                feeDeduction: RurMoney.Zero,
                employeeFee: new EmployeeInsuranceFee(RurMoney.Zero, RurMoney.Zero, RurMoney.Zero),
                ndfl: RurMoney.Zero,
                feeTotal: RurMoney.Zero,
                total: new RurMoney(60000m),
                burgen: 6m
                );

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task ChooseOrgUsnD_R_Then_CalculateWright()
        {
            // Assign
            var calculator =
                await _taxCalculatorFactory.CreateCalculator(
                    ECompanyType.OOO,
                    EAccountTaxationSystem.Usn15);

            // Act
            var @params =
                new CustomerTaxParameters(
                    new RurMoney(1000000m),
                    new RurMoney(200000m),
                    new RurMoney(30000m));

            var actual = await calculator(@params);

            // Assert
            var expected = new OrgUsnD_RTaxAggregate(
                taxExpenses: new RurMoney(244827.83m),
                sts: new RurMoney(113275.83m),
                employeeFee: new EmployeeInsuranceFee(
                    new RurMoney(7586.21m),
                    new RurMoney(1758.62m),
                    new RurMoney(1000m)),
                ndfl: new RurMoney(4483m),
                feeTotal: new RurMoney(10344.83m),
                total: new RurMoney(128103.66m),
                burgen: 12.81m
                );

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task ChooseOrgUsnD_R_WithoutEmployeesThen_CalculateWright()
        {
            // Assign
            var calculator =
                await _taxCalculatorFactory.CreateCalculator(
                    ECompanyType.OOO,
                    EAccountTaxationSystem.Usn15);

            // Act
            var @params =
                new CustomerTaxParameters(
                    new RurMoney(1000000m),
                    new RurMoney(200000m),
                    RurMoney.Zero);

            var actual = await calculator(@params);

            // Assert
            var expected = new OrgUsnD_RTaxAggregate(
                taxExpenses: new RurMoney(200000m),
                sts: new RurMoney(120000m),
                employeeFee: new EmployeeInsuranceFee(RurMoney.Zero, RurMoney.Zero, RurMoney.Zero),
                ndfl: RurMoney.Zero,
                feeTotal: RurMoney.Zero,
                total: new RurMoney(120000m),
                burgen: 12m
                );

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task ChooseIpOsn_Then_CalculateWright()
        {
            // Assign
            var calculator =
                await _taxCalculatorFactory.CreateCalculator(
                    ECompanyType.IP,
                    EAccountTaxationSystem.Osn);

            // Act
            var @params =
                new CustomerTaxParameters(
                    new RurMoney(1000000m),
                    new RurMoney(100000m),
                    new RurMoney(30000m));

            var actual = await calculator(@params);

            // Assert
            var expected = new IndividualOsnTaxAggregate(
                pit: new RurMoney(89115m),
                nds: new RurMoney(137288.13m),
                selfInsuranceFee: _selfInsuranceFee,
                additionalFee: new RurMoney(7000m),
                feeTotal: new RurMoney(49729.83m),
                employeeFee: new EmployeeInsuranceFee(
                    new RurMoney(7586.21m),
                    new RurMoney(1758.62m),
                    new RurMoney(1000m)),
                employeeNdfl: new RurMoney(4483m),
                total: new RurMoney(276132.96m),
                burgen: 27.61m
                );

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task ChooseIpOsnWithoutEmployees_Then_CalculateWright()
        {
            // Assign
            var calculator =
                await _taxCalculatorFactory.CreateCalculator(
                    ECompanyType.IP,
                    EAccountTaxationSystem.Osn);

            // Act
            var @params =
                new CustomerTaxParameters(
                    new RurMoney(1000000m),
                    new RurMoney(100000m),
                    RurMoney.Zero);

            var actual = await calculator(@params);

            // Assert
            var expected = new IndividualOsnTaxAggregate(
                pit: new RurMoney(94942m),
                nds: new RurMoney(137288.13m),
                selfInsuranceFee: _selfInsuranceFee,
                additionalFee: new RurMoney(7000m),
                feeTotal: new RurMoney(39385m),
                employeeFee: new EmployeeInsuranceFee(RurMoney.Zero, RurMoney.Zero, RurMoney.Zero),
                employeeNdfl: RurMoney.Zero,
                total: new RurMoney(271615.13m),
                burgen: 27.16m
                );

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task ChooseOrgOsn_Then_CalculateWright()
        {
            // Assign
            var calculator =
                await _taxCalculatorFactory.CreateCalculator(
                    ECompanyType.OOO,
                    EAccountTaxationSystem.Osn);

            // Act
            var @params =
                new CustomerTaxParameters(
                    new RurMoney(1000000m),
                    new RurMoney(100000m),
                    new RurMoney(30000m));

            var actual = await calculator(@params);

            // Assert
            var feeTotal = new RurMoney(10344.83m);

            var profitTax = new RurMoney(143576.81m);

            var nds = new RurMoney(137288.13m);
            
            var expected = new OrgOsnTaxAggregate(
                profitTax,
                nds,
                feeTotal,
                employeeFee: new EmployeeInsuranceFee(
                    new RurMoney(7586.21m),
                    new RurMoney(1758.62m),
                    new RurMoney(1000m)),
                ndfl: new RurMoney(4483m),
                total: new RurMoney(295692.77m),
                burgen: 29.57m
                );

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task ChooseOrgOsnWithoutEmployee_Then_CalculateWright()
        {
            // Assign
            var calculator =
                await _taxCalculatorFactory.CreateCalculator(
                    ECompanyType.OOO,
                    EAccountTaxationSystem.Osn);

            // Act
            var @params =
                new CustomerTaxParameters(
                    new RurMoney(1000000m),
                    new RurMoney(100000m),
                    RurMoney.Zero);

            var actual = await calculator(@params);

            // Assert
            var feeTotal = RurMoney.Zero;

            var profitTax = new RurMoney(152542.37m);

            var nds = new RurMoney(137288.13m);

            var expected = new OrgOsnTaxAggregate(
                profitTax,
                nds,
                feeTotal,
                employeeFee: new EmployeeInsuranceFee(RurMoney.Zero, RurMoney.Zero, RurMoney.Zero),
                ndfl: RurMoney.Zero,
                total: new RurMoney(289830.5m),
                burgen: 28.98m
                );

            Assert.AreEqual(expected, actual);
        }
    }
}
