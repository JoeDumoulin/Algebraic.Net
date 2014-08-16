using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinearAlgebra;
using Numerics;
using NUnit.Framework;


namespace VectorTest
{
  [TestFixture]
  public class VectorTests
  {
    [Test]
    public void add_two_vectors_generates_new_vector()
    {
      var v1 = new Vector<int>(new int[] { 1, 2, 3 }, Orientation.Row
        , AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid);
      var v2 = new Vector<int>(new int[] { 1, 2, 3 }, Orientation.Row
        , AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid);
      Assert.That(v1, Is.EqualTo(v2));
    }

    [Test]
    public void equal_vectors_have_equal_hashcodes()
    {
      var v1 = new Vector<int>(new int[] { 1, 2, 3 }, Orientation.Row
        , AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid);
      var v2 = new Vector<int>(new int[] { 1, 2, 3 }, Orientation.Row
        , AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid);

      Assert.IsTrue(v1 == v2);
      Assert.That(v1.GetHashCode(), Is.EqualTo(v2.GetHashCode()));
    }

    [Test]
    public void vector_addition_produces_a_new_vector()
    {
      var v1 = new Vector<int>(new int[] { 1, 2, 3 }, Orientation.Row
        , AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid);
      var v2 = new Vector<int>(v1);

      var expected = new Vector<int>(new int[] { 2, 4, 6 }, Orientation.Row
        , AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid);

      var result = v1 + v2;
      Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void adding_unequal_vectors_is_an_error()
    {
      var v1 = new Vector<int>(new int[] { 1, 2, 3 }, Orientation.Row
        , AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid);
      var v2 = new Vector<int>(new int[] { 1, 2, 3, 4 }, Orientation.Row
        , AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid);

      var ex = Assert.Throws<ArgumentException>(() => new Vector<int>(v1 + v2));
      Assert.That(ex.Message, Is.EqualTo("Vector<T>.+ - vectors must have equal length"));
    }

    [Test]
    public void adding_null_vectors_is_an_error()
    {
      var v1 = new Vector<int>(new int[] { 1, 2, 3 },  Orientation.Row
        , AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid);
      Vector<int> v2 = null;

      var ex = Assert.Throws<ArgumentException>(() => new Vector<int>(v1 + v2));
      Assert.That(ex.Message, Is.EqualTo("Vector<T>.+ - null not allowed as an argument"));

       ex = Assert.Throws<ArgumentException>(() => new Vector<int>(v2 + v1));
      Assert.That(ex.Message, Is.EqualTo("Vector<T>.+ - null not allowed as an argument"));
    }

    [Test]
    public void adding_different_orientations_vectors_is_an_error()
    {
      var v1 = new Vector<int>(new int[] { 1, 2, 3 }, Orientation.Row
        , AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid);
      var v2 = new Vector<int>(new int[] { 1, 2, 3 }, Orientation.Column
        , AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid);

      var ex = Assert.Throws<ArgumentException>(() => new Vector<int>(v1 + v2));
      Assert.That(ex.Message, Is.EqualTo("Vector<T>.+ - vectors must have the same orientation for addition"));
    }

    [Test]
    public void vector_multiplication_produces_a_new_vector()
    {
      var v1 = new Vector<int>(new int[] { 1, 2, 3 }, Orientation.Row
        , AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid);

      var scalar = 2;

      var expected = new Vector<int>(new int[] { 2, 4, 6 }, Orientation.Row
        , AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid);

      var result = v1 * scalar;
      Assert.That(result, Is.EqualTo(expected));

      result = scalar * v1;
      Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void vector_multiplication_by_unit_produces_equivalent_vector()
    {
      var v1 = new Vector<int>(new int[] { 1, 2, 3 }, Orientation.Row
        , AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid);

      var scalar = MultiplicativeMonoid.IntMultiplicationMonoid.Unit;

      var expected = new Vector<int>(v1);

      var result = v1 * scalar;
      Assert.That(result, Is.EqualTo(expected));

      result = scalar * v1;
      Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void vector_multiply_with_null_vector_and_scalar_throws_error()
    {
      Vector<int> v1 = null;
      var scalar = 2;


      var ex = Assert.Throws<ArgumentException>(() => new Vector<int>(v1 * scalar));
      Assert.That(ex.Message, Is.EqualTo("Vector<T>.* - null not allowed as an argument"));
      ex = Assert.Throws<ArgumentException>(() => new Vector<int>(scalar * v1));
      Assert.That(ex.Message, Is.EqualTo("Vector<T>.* - null not allowed as an argument"));
    }

    [Test]
    public void vector_inner_product_produces_2norm()
    {
      var v1 = new Vector<int>(new int[] { 1, 2, 3 }, Orientation.Row
        , AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid);
      var v2 = new Vector<int>(new int[] { 1, 2, 3 }, Orientation.Column
        , AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid);

      Assert.That(v1.InnerProduct(v2), Is.EqualTo(14));
    }

    [Test]
    public void vector_created_with_semiring_inner_product_produces_2norm()
    {
      var v1 = new Vector<int>(new int[] { 1, 2, 3 }, Orientation.Row
        , new SemiRing<int>(AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid));
      var v2 = new Vector<int>(new int[] { 1, 2, 3 }, Orientation.Column
        , new SemiRing<int>(AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid));

      Assert.That(v1.InnerProduct(v2), Is.EqualTo(14));
    }

    [Test]
    public void vector_inner_product_throws_error_when_orientation_is_the_same()
    {
      var v1 = new Vector<int>(new int[] { 1, 2, 3 }, Orientation.Row
        , AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid);
      var v2 = new Vector<int>(new int[] { 1, 2, 3 }, Orientation.Row
        , AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid);

      var ex = Assert.Throws<ArgumentException>(() => v1.InnerProduct(v2));
      Assert.That(ex.Message, Is.EqualTo("Vector<T>.InnerProduct - dot product requires a row vector followed by a column vector"));
    }

    [Test]
    public void vector_inner_product_throws_error_when_vectors_have_different_length()
    {
      var v1 = new Vector<int>(new int[] { 1, 2, 3 }, Orientation.Row
        , AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid);
      var v2 = new Vector<int>(new int[] { 1, 2, 3, 4 }, Orientation.Column
        , AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid);

      var ex = Assert.Throws<ArgumentException>(() => v1.InnerProduct(v2));
      Assert.That(ex.Message, Is.EqualTo("Vector<T>.InnerProduct - vectors must have equal length"));
    }

    public void vector_inner_product_throws_error_when_one_vector_is_null()
    {
      var v1 = new Vector<int>(new int[] { 1, 2, 3 }, Orientation.Row
        , AdditiveMonoid.IntAdditionMonoid, MultiplicativeMonoid.IntMultiplicationMonoid);
      Vector<int> v2 = null;

      var ex = Assert.Throws<ArgumentException>(() => new Vector<int>(v1 + v2));
      Assert.That(ex.Message, Is.EqualTo("Vector<T>.InnerProduct - null not allowed as an argument"));

      ex = Assert.Throws<ArgumentException>(() => new Vector<int>(v2 + v1));
      Assert.That(ex.Message, Is.EqualTo("Vector<T>.InnerProduct - null not allowed as an argument"));
    }
  }
}
