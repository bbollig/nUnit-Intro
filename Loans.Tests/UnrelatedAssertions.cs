using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Loans.Tests
{
    [TestFixture]
    class UnrelatedAssertions
    {
        [Test]
        public void StringTestsExamples()
        {
            //Commenting out failing Assertions but leaving for examples
            string name = "Sarah";

            //Assert.That(name, Is.Empty);//fail
            
            Assert.That(name, Is.Not.Empty);
            
            Assert.That(name, Is.EqualTo("Sarah"));
            
            //Assert.That(name, Is.EqualTo("SARAH"));//fail
            
            Assert.That(name, Is.EqualTo("SARAH").IgnoreCase);
            
            Assert.That(name, Does.StartWith("Sa"));
            
            Assert.That(name, Does.EndWith("ah"));
            
            Assert.That(name, Does.Contain("ara"));
            
            Assert.That(name, Does.Not.Contain("Brian"));
            
            Assert.That(name, Does.StartWith("Sa")
                .And.EndWith("rah"));

            Assert.That(name, Does.StartWith("xyz")
                .Or.EndWith("rah"));
        }
    }
}
