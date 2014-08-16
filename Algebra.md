# Simple Numerical and Linear Algebraic Types in C# #
This code is the result of some experiments to produce some generic numerical types using C#. The results may be imperfect, but I've found them interesting and useful.

## Starting at the End: Vector Spaces ##
When I started this code my goal was to get a working generic vector.  The reason for this was some work on multivariate probability distribution calcs I wanted to perform in C#, but that's a different post.

The first important thing was to be able to produce an Inner product.  The InnerProduct should perform this operation:

>     [Test]
>     public void vector_inner_product_produces_squared_2norm()
>     {
>       var v1 = new Vector<int>(new int[] { 1, 2, 3 });      
>       var v2 = new Vector<int>(new int[] { 1, 2, 3 }); 
> 
>       Assert.That(v1.InnerProduct(v2), Is.EqualTo(14));
>     }
> 

An `integer` Vector object will perform the usual multiplication and addition, as will a Vector of `double`.  But simply defining a Vector<T> class and including an inner product method will not get us very far.  The most basic code will require you to eventually write something like, given `T a` and `T b`:

> T c = a + b;
>
> T d = a * b;

This code doesn't compile in C# because the type `T` does not necessarily support the `+` or `*` operations.

So how can we get the behavior we want in a generic way?  That is, without writing the same code in different places for integers and doubles? 

There are many answers to this question in the context of C#.  The path I took to this code revealed the details of the algebra of vectors and made them explicit in the implementation.

## What Is a Vector? ##
The key to the vector inner product solution described here is to understand the common structure of the objects we are calling vectors.

Vectors are objects that participate in an structure called a *Vector Space*.  Some will know this idea from the contexts of linear algebra, engineering, or computer graphics for example.

At the risk of sounding circular,we can formally define a vector space as a set of objects called vectors and a field that acts on the vectors with the following rule:

*Let V be a vector space, u and v be vectors in V, and let x and y be* **scalar** 
*(members of the Field in V). Then ux + vy is a vector in V.* 

So vectors can be multiplied by scalars and can be added together to produce new vectors.

## Rings, Semirings, and Monoids ##
Fields are relatively complex objects that are not needed for dot product capability. In fact we can use simpler objects.  

We can in fact simplify the Field to the structure of an object called a **Ring**.  Without being too mathematically precise, a ring is a set of objects over which there is defined addition and multiplication and where a distributive law holds.

We can further relax our requirements on the structures by changing from a ring to a semiring, dropping the requirement for addition have an inverse.  Then we are able to define vectors within a relatively simple pair of structures called **Monoids**.

Monoids turn out to be very instersting and flexible structures.  Monoids are sets over which is defined a single operation (like addition or multiplication) and containing a special element called the identity or *unit*.

More precisely, A monoid is:

	- A set S
	- A binary operation +:S X S -> S
	- An element i in S such that for any x in S i+x = x+i = x.  
	for regular addition, i looks very much like zero (0).

We can define a monoid easily in C\# as:

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
        
With the definition of a monoid, we can produce a semiring as follows:

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
    
Now we can define our 'vector' object (which is currently not capable of division):

    public class Vector<T> : IEquatable<Vector<T>>, IEnumerable<T>
    {
      private T[] _data;  
      public readonly SemiRing<T> Ring;
      public readonly Orientation Direction;

	etc...

The code for vectors includes a few more details than this, but with the semiring we can now define a working generic Inner Product function suitable for our test.

Inner Product can be written as an extension to `Vector<T>`:

      public static T InnerProduct<T>(this Vector<T> v1, Vector<T> v2)
      {
        return v1.InnerProduct(v2, v2.Ring);
      }

And Inner Product is defined for stream containers as well:

    public static T InnerProduct<T>(this IEnumerable<T> left, IEnumerable<T> right
      , SemiRing<T> ring)
    {
      return left.Zip(right, (x, y) => ring.Product.Op(x, y)).MonoidOp(ring.Add);
    }

Now we need to update the test a little.  We need to add the proper syntax for creating the vector objects to reference the semiring. 

    [Test]
    public void vector_created_with_semiring_inner_product_produces_2norm()
    {
      var v1 = new Vector<int>(new int[] { 1, 2, 3 }, Orientation.Row
        , new SemiRing<int>(AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid));
      var v2 = new Vector<int>(new int[] { 1, 2, 3 }, Orientation.Column
        , new SemiRing<int>(AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid));

      Assert.That(v1.InnerProduct(v2), Is.EqualTo(14));
    }

We've also added the specific monoid implementations for integer multiplicative and integer additive monoids.  These require tests too:

    [Test]
    public void additive_integer_monoid_adds_integers()
    {
      int a = 3;
      int b = 5;
      var m = AdditiveMonoid.IntAdditionMonoid;

      Assert.That(m.Op(a, b), Is.EqualTo(8));
    }

and:

    [Test]
    public void multiplicative_integer_monoid_adds_integers()
    {
      int a = 3;
      int b = 5;
      var m = MultiplicativeMonoid.IntMultiplicationMonoid;

      Assert.That(m.Op(a, b), Is.EqualTo(15));
    }

To support these tests, we create some static instance of Monoid to support the integer type operations:

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

This seems like a lot of work to get two numbers added or multiplied.  Take a look at Monoid.cs and MonoidTests.cs to see the interesting and fascinating types of generalizations made possible using monoids.

With these final changes our InnerProduct test will run and we now have created a generic inner product in C#.  

