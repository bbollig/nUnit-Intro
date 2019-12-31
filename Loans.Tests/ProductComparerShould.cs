using Loans.Domain.Applications;
using NUnit.Framework;
using System.Collections.Generic;

namespace Loans.Tests
{
    [ProductComparison]
    public class ProductComparerShould
    {
        private List<LoanProduct> products;
        private ProductComparer sut;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            //In cases where creating a resource takes a large amount of time and we do not want to
            //repeat it for every test, we can use OneTimeSetUp
            //Simulate long setup init time for this list of products
            //We assume that this list will not be modified by any tests
            //as this will potentially break other tests (i.e. break test isolation)
            products = new List<LoanProduct>
            {
                new LoanProduct(1, "a", 1),
                new LoanProduct(2, "b", 2),
                new LoanProduct(3, "c", 3),
            };
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            //Run after lsat test in this test class (fixture) executes
            //e.g. disposing of shared expensive setup performed in OneTimeSetUp

            //products.Dispose(); e.g. if products implemented IDisposable
        }

        //Runs BEFORE EACH test executes
        [SetUp]
        public void SetUp()
        {
            sut = new ProductComparer(new LoanAmount("USD", 200_000m), products);
        }

        //Runs AFTER EACH test executes
        [TearDown]
        public void TearDown()
        {
            //sut.Dispose();
        }

        [Test]
        public void ReturnCorrectNumberOfComparisons()
        {
            List<MonthlyRepaymentComparison> comparisons =
                sut.CompareMonthlyRepayments(new LoanTerm(30));

            //Has constraint helper class has a number of convenience methods for working with collections
            Assert.That(comparisons, Has.Exactly(3).Items);
        }

        [Test]
        public void NotReturnDuplicateComparisons()
        {
            List<MonthlyRepaymentComparison> comparisons =
                sut.CompareMonthlyRepayments(new LoanTerm(30));

            Assert.That(comparisons, Is.Unique);
        }

        [Test]
        public void ReturnComparisonForFirstProduct()
        {
            List<MonthlyRepaymentComparison> comparisons =
                sut.CompareMonthlyRepayments(new LoanTerm(30));

            //Need to know the expected monthly repayment (third parameter)
            var expectedProduct =  new MonthlyRepaymentComparison("a", 1, 643.28m);

            Assert.That(comparisons, Does.Contain(expectedProduct));
        }

        [Test]
        public void ReturnComparisonForFirstProduct_WithPartialKnownExpectedValues()
        {
            List<MonthlyRepaymentComparison> comparisons =
                sut.CompareMonthlyRepayments(new LoanTerm(30));

            //TypeSafe way of specifying conditions:
            Assert.That(comparisons, Has.Exactly(1)
                                                 .Matches<MonthlyRepaymentComparison>(
                                                     item => item.ProductName == "a" &&
                                                             item.InterestRate == 1 &&
                                                             item.MonthlyRepayment > 0));
        }
    }
}
