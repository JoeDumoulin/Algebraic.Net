using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numerics
{


  public class Group<T>
  {
    public readonly Monoid<T> Monoid;
    public readonly Func<T, T> Inverse;

    public Group(Monoid<T> monoid, Func<T, T> inverse)
    {
      Monoid = monoid;
      Inverse = inverse;
    }
  }

  // ordered group (Archimedean)

}
