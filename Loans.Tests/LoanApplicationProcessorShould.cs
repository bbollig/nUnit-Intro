using Loans.Domain.Applications;
using NUnit.Framework;
using Moq;

namespace Loans.Tests
{
    class LoanApplicationProcessorShould
    {
        /*
         * - Fakes, Dummies, Stubs and Mocks - 
         * Fakes: Working implementation, not suitable for production
         * Dummies: Passed around but never used, satisfy parameters
         * Stubs: Provide answers to calls, Property gets, Method returns
         * Mocks: Verify Interaction, Properties, Methods
         *
         * Test Double is a generic term referring to any of the above, where you replace a product object
         * for testing purposes
         */
        [Test]
        public void DeclineLowSalary()
        {
            LoanProduct product = new LoanProduct(99, "Loan", 5.25m);
            LoanAmount amount = new LoanAmount("USD", 200_000);

            var application = new LoanApplication(42, 
                product, 
                amount, 
                "Sarah", 
                25, 
                "133 Pluralsight Dr., Draper, Utah",
                64_999);

            var mockIdentityVerifier = new Mock<IIdentityVerifier>();
            var mockCreditScorer = new Mock<ICreditScorer>();

            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object, mockCreditScorer.Object);

            sut.Process((application));

            Assert.That(application.GetIsAccepted(), Is.False);
        }

        [Test]
        public void Accept()
        {
            LoanProduct product = new LoanProduct(99, "Loan", 5.25m);
            LoanAmount amount = new LoanAmount("USD", 200_000);

            var application = new LoanApplication(
                42,
                product,
                amount,
                "Sarah",
                25,
                "133 Pluralsight Dr., Draper, Utah",
                65_000);

            var mockIdentityVerifier = new Mock<IIdentityVerifier>();
            mockIdentityVerifier.Setup(x => x.Validate(
                "Sarah",
                25,
                "133 Pluralsight Dr., Draper, Utah"))
                .Returns(true);


            var mockCreditScorer = new Mock<ICreditScorer>();

            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object, mockCreditScorer.Object);

            sut.Process((application));

            Assert.That(application.GetIsAccepted(), Is.True);
        }

    }
}
