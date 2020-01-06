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
            //It.IsAny for parameter substitution, seen below
            //To work, uncomment lines 41-43 in LoanApplicationProcessor.cs

            //mockIdentityVerifier.Setup(x => x.Validate(
            //    It.IsAny<string>(),
            //    It.IsAny<int>(),
            //    It.IsAny<string>()))
            //    .Returns(true);

            //But in this case, since we know what parameters we are setting up our LoanApplication
            //with, it would be better to just use them in this case
            //mockIdentityVerifier.Setup(x => x.Validate(
            //        "Sarah",
            //        25,
            //        "133 Pluralsight Dr., Draper, Utah"))
            //    .Returns(true);

            //like above, but using an out variable instead
            //To work, uncomment lines 47-56 in LoanApplicationProcessor.cs

            bool isValidOutValue = true;

            mockIdentityVerifier.Setup(x => x.Validate(
                "Sarah",
                25,
                "133 Pluralsight Dr., Draper, Utah",
                out isValidOutValue));


            //Like above, but using ref. This does not work!
            //mockIdentityVerifier
            //    .Setup(x => x.Validate("Sarah",
            //        25,
            //        "133 Pluralsight Drive, Draper, Utah",
            //        ref It.Ref<IdentityVerificationStatus>.IsAny))
            //    .Callback(new ValidateCallback(
            //        (string applicantName,
            //               int applicantAge,
            //               string applicantAddress,
            //               ref IdentityVerificationStatus status) =>
            //                status = new IdentityVerificationStatus(true)));



            var mockCreditScorer = new Mock<ICreditScorer>();

            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object, mockCreditScorer.Object);

            sut.Process((application));

            Assert.That(application.GetIsAccepted(), Is.True);
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
