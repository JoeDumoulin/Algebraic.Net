using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Numerics;

namespace NumericTests
{
  [TestFixture]
  public class GroupTests
  {
    [Test]
    public void group_inverse_times_element_equals_unit()
    {
      var a = new Monoid<double>(0.0, (x, y) => x + y);
      var g = new Group<double>(a, (x) => -x);

      Assert.That(g.Monoid.Op(4.0,g.Inverse(4.0)), Is.EqualTo(g.Monoid.Unit));
    }
  }
}
