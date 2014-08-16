using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Numerics;

namespace NumericTests
{
  public static class IEnumerableExtensionMethods
  {
    // hey linq! thanks for the monads.

    public static int Add(this IEnumerable<int> vals)
    {
      return vals.MonoidOp(AdditiveMonoid.IntAdditionMonoid);
    }
    public static double Add(this IEnumerable<double> vals)
    {
      return vals.MonoidOp(AdditiveMonoid.DoubleAdditionMonoid);
    }

    public static int Mult(this IEnumerable<int> vals)
    {
      return vals.MonoidOp(MultiplicativeMonoid.IntMultiplicationMonoid);
    }
    public static double Mult(this IEnumerable<double> vals)
    {
      return vals.MonoidOp(MultiplicativeMonoid.DoubleMultiplicationMonoid);
    }

    public static string Append(this IEnumerable<string> vals)
    {
      return vals.MonoidOp(new Monoid<string>(string.Empty, (x, y) => x + y));
    }
  }

  [TestFixture]
  public class MonoidTests
  {
    // enumerable operation tests
    [Test]
    public void monoid_string_addition_is_concatenation()
    {
      var list = new List<string>() { "abc", "def" };
      Assert.That(list.Append(), Is.EqualTo("abcdef"));
    }

    [Test]
    public void add_list_of_integers()
    {
      var list = new List<int>() { 1, 2, 3, 4, 5, 6 };
      Assert.That(list.Add(), Is.EqualTo(21));
    }
    [Test]
    public void add_list_of_doubles()
    {
      var list = new List<double>() { 1, 2, 3, 4, 5, 6 };
      Assert.That(list.Add(), Is.EqualTo(21));
    }

    [Test]
    public void mult_list_of_int()
    {
      var list = new List<int>() { 1, 2, 3, 4, 5, 6 };
      Assert.That(list.Mult(), Is.EqualTo(720));
    }

    [Test]
    public void mult_list_of_double()
    {
      var list = new List<double>() { 1, 2, 3, 4, 5, 6 };
      Assert.That(list.Mult(), Is.EqualTo(720));
    }

    [Test]
    public void additive_integer_monoid_adds_integers()
    {
      int a = 3;
      int b = 5;
      var m = AdditiveMonoid.IntAdditionMonoid;

      Assert.That(m.Op(a, b), Is.EqualTo(8));
    }

    [Test]
    public void additive_double_monoid_adds_integers()
    {
      double a = 3.0;
      double b = 0.5;
      var m = AdditiveMonoid.DoubleAdditionMonoid;

      Assert.That(m.Op(a, b), Is.EqualTo(3.5));
    }

    [Test]
    public void multiplicative_integer_monoid_adds_integers()
    {
      int a = 3;
      int b = 5;
      var m = MultiplicativeMonoid.IntMultiplicationMonoid;

      Assert.That(m.Op(a, b), Is.EqualTo(15));
    }

    [Test]
    public void multiplicative_double_monoid_adds_integers()
    {
      double a = 3.0;
      double b = 0.5;
      var m = MultiplicativeMonoid.DoubleMultiplicationMonoid;

      Assert.That(m.Op(a, b), Is.EqualTo(1.5));
    }

    [Test]
    public void composition_monoid_composes_functions()
    {
      Func<int, int> f = (a) => a + 1;
      Func<int, int> g = (a) => a * a;
      var m = FuctionCompositionMonoid<int>.CompositionMonoid;
      Func<int, int> o = m.Op(f, g);

      Func<int, int> p = m.Op(g, f);

      Assert.That(o(1), Is.EqualTo(2));
      Assert.That(p(1), Is.EqualTo(4));
    }

    [Test]
    public void permute_monoid_permutes_arrays()
    {
      int[] a = new int[] { 1, 3, 2, 5, 4, 0 };
      int[] b = new int[] { 5, 4, 2, 1, 3, 0 };

      var m = PermutationCompositionMonoid.PermutationIndexMonoidGenerator(6).Op(a,b);

      Assert.That(m.SequenceEqual(new List<int>() { 4, 1, 2, 0, 3, 5 }));
    }

    [Test]
    public void generate_permute_monoid_returns_Unit()
    {
      var p = PermutationCompositionMonoid.PermutationIndexMonoidGenerator(6);
      Assert.That(p.Unit.SequenceEqual(new List<int>() { 0, 1, 2, 3, 4, 5 }));
    }

    [Test]
    public void permute_with_inverse_returns_identity()
    {
      int[] a = new int[] { 1, 3, 2, 5, 4, 0 };
      var p = PermutationCompositionMonoid.PermutationIndexMonoidGenerator(6);
      var a2 = a;
      while (!p.Op(a2, a).SequenceEqual(p.Unit))
      {
        a2 = p.Op(a2, a).ToArray();
      }

      Assert.That(p.Op(a2, a).SequenceEqual(p.Unit));
    }
  }
}
