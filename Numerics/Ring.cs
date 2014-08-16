using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerics
{
  public class SemiRing<T>
  {
    public readonly Monoid<T> Add;
    public readonly Monoid<T> Product;

    public SemiRing(Monoid<T> add, Monoid<T> product)
    {
      Add = add;
      Product = product;
    }
  }


  //class Ring
  //{
  //}
}
