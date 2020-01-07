using System;
using Loans.Domain.Applications;
using NUnit.Framework;
using Moq;
using Moq.Protected;

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

            //Behavior-based testing
            //Here we can Verify the "Getter" property was accessed during this test
            mockCreditScorer.VerifyGet(x => x.ScoreResult.ScoreValue.Score, Times.Once);
            //And the set:
            mockCreditScorer.VerifySet(x => x.Count = 1);

            //Object-state testing
            Assert.That(application.GetIsAccepted(), Is.True);
            Assert.That(mockCreditScorer.Object.Count, Is.EqualTo(1));
        }

        [Test]
        public void InitializeIdentityVerifier()
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

            mockCreditScorer.SetupAllProperties();

            mockCreditScorer.Setup(x => x.ScoreResult.ScoreValue.Score).Returns(300);

            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object, mockCreditScorer.Object);

            sut.Process((application));

            //Behavior-based testing
            //Verify is a method from MOQ that in this case will tell us if the Initialize method
            //on the mock IIdentityVerifier we have below has been called.
            mockIdentityVerifier.Verify(x => x.Initialize());

            mockIdentityVerifier.Verify(x => x.Validate(It.IsAny<string>(),
                                                        It.IsAny<int>(),
                                                        It.IsAny<string>()));
            //Here we can be very strict about what we expect to happen. After Verifying every
            //call that happens during testing, we can Verify that NOTHING else occurred.
            mockIdentityVerifier.VerifyNoOtherCalls();
        }

        [Test]
        public void CalculateScore()
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

            mockCreditScorer.SetupAllProperties();

            mockCreditScorer.Setup(x => x.ScoreResult.ScoreValue.Score).Returns(300);

            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object, mockCreditScorer.Object);

            sut.Process((application));

            //Behavior-based testing
            //As seen in previous examples but deleted now, we can use MOQ's It.IsAny<> to test cases where we want to check
            //if parameters were sent but don't particularly care what was sent
            mockCreditScorer.Verify(x => x.CalculateScore(It.IsAny<string>(), It.IsAny<string>()));

            //Here we are Verifying that CalculateScore was called with specific parameters, and ensuring that call
            //happened only once.
            mockCreditScorer.Verify(x => x.CalculateScore(
                    "Sarah",
                    "133 Pluralsight Dr., Draper, Utah"),
                Times.Once);
        }

        [Test]
        public void DeclineWhenCreditScoreError()
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

            mockCreditScorer.Setup(x => x.ScoreResult.ScoreValue.Score).Returns(300);

            //Here we can test if an error gets thrown. We can specify type and even customize the error
            //message, seen in the commented out portion below
            mockCreditScorer.Setup(x => x.CalculateScore(It.IsAny<string>(), It.IsAny<string>()))
                .Throws<InvalidOperationException>();
                //.Throws(new InvalidOperationException("Test Exception"));

            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object, mockCreditScorer.Object);

            sut.Process((application));

            Assert.That(application.GetIsAccepted(), Is.False);
        }

        [Test]
        public void AcceptUsingPartialMock()
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

            var mockIdentityVerifier = new Mock<IdentityVerifierServiceGateway>();

            //Here we can mock Protected class members by bringing in the Moq.Protected namespace (via using statement)
            //And we have to explicitly name the protected member we wish to mock. 
            mockIdentityVerifier.Protected().Setup<bool>("CallService",
                                                         "Sarah",
                                                         25,
                                                         "133 Pluralsight Dr., Draper, Utah")
                                .Returns(true);
            //If we want to be able to get intellisense
            //for protected members instead of explicitly stating them like below, we need to create an interface that defines those
            //members we wish to mock and use slightly different syntax, shown here:
            //mockIdentityVerifier.Protected()
            //    .As<IIdentityVerifierServiceGatewayProtectedMembers>()
            //    .Setup(x => x.CallService(It.IsAny<string>(),
            //                              It.IsAny<int>(),
            //                              It.IsAny<string>()))
            //    .Returns(true);


            var expectedTime = new DateTime(2000, 1, 1);
            //Here we can explicitly set up non-deterministic behavior to test
            mockIdentityVerifier.Protected().Setup<DateTime>("GetCurrentTime")
                                .Returns(expectedTime);

            var mockCreditScorer = new Mock<ICreditScorer>();

            mockCreditScorer.Setup(x => x.ScoreResult.ScoreValue.Score).Returns(300);

            var sut = new LoanApplicationProcessor(mockIdentityVerifier.Object, mockCreditScorer.Object);

            sut.Process((application));

            Assert.That(application.GetIsAccepted(), Is.True);
            Assert.That(mockIdentityVerifier.Object.LastCheckTime, Is.EqualTo(expectedTime));
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
