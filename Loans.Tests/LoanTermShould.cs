using Loans.Domain.Applications;
using NUnit.Framework;

namespace Loans.Tests
{
    [TestFixture]
    public class LoanTermShould
    {
        [Test]
        public void ReturnTermInMonths()
        {
            //Constraint method of assertion is the new feature
            //Assert.That(*test result, *constraint instance);
            //vs. Classic method of assertion which would look like this:
            //Assert.AreEqual(1, sut.Years);
            var sut = new LoanTerm(1);
            Assert.That(sut.ToMonths(), Is.EqualTo(12));
        }

        [Test]
        public void StoreYears()
        {
            var sut = new LoanTerm(1);
            Assert.That(sut.Years, Is.EqualTo(1));
        }
    }
}
