using System;
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

        delegate void ValidateCallback(string applicantName,
                                       int applicantAge,
                                       string applicantAddress,
                                       ref IdentityVerificationStatus status);


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

            //In cases where we don't care what parameters are being sent, we can use MOQ's
            //It.IsAny for parameter substitution. But in this case, since we know what parameters
            // we are setting up our LoanApplication//with, it would be better to just use them in this case
            mockIdentityVerifier.Setup(x => x.Validate(
                    "Sarah",
                    25,
                    "133 Pluralsight Dr., Draper, Utah"))
                .Returns(true);

            var mockCreditScorer = new Mock<ICreditScorer>();
            //The following sets up the property hierarchy to return a valid credit score
            mockCreditScorer.Setup(x => x.ScoreResult.ScoreValue.Score).Returns(300);
            //The following tells MOQ to track changes to the Count property so that we can test
            //it in the second assertion
            mockCreditScorer.SetupProperty(x => x.Count);

            //You can also automatically configure all properties for an object using
            //mockCreditScorer.SetupAllProperties();
            //instead of the SetupProperty method.
            //However, the caveat here is that the above line would need to be placed above
            //the Setup method because having it below would REconfigure our explicitly set
            //ScoreResult.ScoreValue.Score of 300

            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object, mockCreditScorer.Object);

            sut.Process((application));

            Assert.That(application.GetIsAccepted(), Is.True);
            Assert.That(mockCreditScorer.Object.Count, Is.EqualTo(1));
        }

        [Test]
        public void NullReturnExample()
        {
            var mock = new Mock<IINullExample>();

            mock.Setup(x => x.SomeMethod())
                .Returns<string>(null);

            string mockReturnValue = mock.Object.SomeMethod();

            Assert.That(mockReturnValue, Is.Null);
        }
    }

    public interface IINullExample
    {
        string SomeMethod();
    }
}
