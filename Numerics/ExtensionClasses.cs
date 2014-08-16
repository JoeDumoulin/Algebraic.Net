using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Numerics
{
  public static class ExtensionClasses
  {
    public static T MonoidOp<T>(this IEnumerable<T> summables, Monoid<T> mon)
    {
      return summables.Aggregate(mon.Unit, mon.Op);
    }

    public static T InnerProduct<T>(this IEnumerable<T> left, IEnumerable<T> right
      , Monoid<T> sum, Monoid<T> product)
    {
      return left.Zip(right, (x, y) => product.Op(x, y)).MonoidOp(sum);
    }

    public static T InnerProduct<T>(this IEnumerable<T> left, IEnumerable<T> right
      , SemiRing<T> ring)
    {
      if (left == null) throw new ArgumentNullException("left");
      if (right == null) throw new ArgumentNullException("right");
      if (ring == null) throw new ArgumentNullException("ring");

      return left.Zip(right, (x, y) => ring.Product.Op(x, y)).MonoidOp(ring.Add);
    }
    /// <summary>
    /// Permute the elements of the source list using the indexes in the permuteindexes list.
    /// </summary>
    /// <typeparam name="T">The type of the contained object</typeparam>
    /// <param name="source">Input list</param>
    /// <param name="permuteIndexes">An ordered list of indexes of the source list to permute</param>
    /// <returns>the permuted list of values from the source list</returns>
    public static IEnumerable<T> Permute<T>(this IEnumerable<T> source, IEnumerable<int> permuteIndexes)
    {
      if (source == null) throw new ArgumentNullException("source");
      if (permuteIndexes == null) throw new ArgumentNullException("permuteIndexes");
      if (permuteIndexes.Count() < 2) throw new ArgumentOutOfRangeException("permuteIndexes", @"the Count of 'permuteIndexes' must be greater than 2");

      var work = source.ToArray();
      var size = work.Count();
      int cell = permuteIndexes.First();

      foreach (var index in permuteIndexes)
      {
        T temp = work[cell];
        work[cell] = work[index];
        work[index] = temp;
        cell = index;
      }
      return work;
    }
  }

}
