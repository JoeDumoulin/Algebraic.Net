using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// this is my lightweight cheesy way of getting around 
//  not being able to infer type behavior
namespace Numerics
{
  public class Monoid<T>
  {
    public readonly  T Unit; // Unary Operator
    public readonly Func<T, T, T> Op; // Monoid Compositon Rule

    public Monoid(T unit, Func<T, T, T> op)
    {
      Unit = unit;
      Op = op;
    }
  }

  // Ordered monoid
  public class OrderedMonoid<T> : Monoid<T>
  {
    public readonly Func<T,T,bool> Less;

    public OrderedMonoid(T unit, Func<T, T, T> op, Func<T, T, bool> less)
      :base(unit, op)
    {
      Less = less;
    }
  }

  // Here are some important sets of objects that are monoids

  public static class AdditiveMonoid
  {
    // delegates for additive monoid addition
    public static readonly Monoid<int> IntAdditionMonoid =
      new Monoid<int>(0, (x, y) => x + y);
    public static readonly Monoid<double> DoubleAdditionMonoid =
      new Monoid<double>(0.0, (x, y) => x + y);
  }

  public static class MultiplicativeMonoid
  {
    // delegates for additive monoid addition
    public static readonly Monoid<int> IntMultiplicationMonoid =
      new Monoid<int>(1, (x, y) => x * y);
    public static readonly Monoid<double> DoubleMultiplicationMonoid =
      new Monoid<double>(1.0, (x, y) => x * y);
  }

  public static class ConcatenationMonoid<T>
  {
    public static readonly Monoid<string> StringConcatMoinoid =
      new Monoid<string>(string.Empty, (x, y) => x + y);

    // concat monoid for generic stream
    private static IEnumerable<T> Empty = new List<T>();
    public static readonly Monoid<IEnumerable<T>> EnumerableConcatMoinoid =
      new Monoid<IEnumerable<T>>(Empty, (x, y) => x.Concat(y));
  }

  public static class FuctionCompositionMonoid<T>
  {
    private static T Identity(T a)
    {
      return a;
    }

    private static Func<T, T> F(Func<T, T> f, Func<T, T> g)
    {
      return (a) => f(g(a));
    }

    public static readonly Monoid<Func<T,T>> CompositionMonoid =
      new Monoid<Func<T, T>>(Identity, (a, b) => F(a, b));
  }

  // Permutation Composition Monoid
  public static class PermutationCompositionMonoid
  {
    public static IEnumerable<int> UnitGenerator(int length)
    {
      if (length < 2) throw new ArgumentOutOfRangeException("ComposePermutationIndexes", @"permutation length must be greater than 2");
      
      return Enumerable.Range(0, length);
    }

    public static IEnumerable<int> ComposePermutationIndexes(IEnumerable<int> first, IEnumerable<int> second)
    {
      if (first == null) throw new ArgumentNullException("first");
      if (second == null) throw new ArgumentNullException("second");
      if (first.Count() != second.Count()) throw new ArgumentOutOfRangeException("ComposePermutationIndexes", @"the Count of the first and second permutations must be equal");
      if (first.Count() < 2) throw new ArgumentOutOfRangeException("ComposePermutationIndexes", @"the Count of 'indexes' must be greater than 2");

      int[] f = first.ToArray();
      int[] s = second.ToArray();

      int[] result = new int[f.Length];
      for (int i = 0; i < f.Length; i++)
      {
        result[i] = s[f[i]];
      }
      return result;
    }

    public static Monoid<IEnumerable<int>> PermutationIndexMonoidGenerator(int length)
    {
      return new Monoid<IEnumerable<int>>(UnitGenerator(length), (a, b) => ComposePermutationIndexes(a, b));
    }
  }
}
