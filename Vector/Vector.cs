using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Numerics;

namespace LinearAlgebra
{
    public enum Orientation
    {
      Row,
      Column
    }

    /// <summary>
    /// generic immutable vector
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Vector<T> : IEquatable<Vector<T>>, IEnumerable<T>
    {
      private T[] _data;
      public readonly SemiRing<T> Ring;
      public readonly Orientation Direction;

      public Vector(T[] data, Orientation direction, SemiRing<T> ring)
      {
        _data = data;
        Direction = direction;
        Ring = ring;
      }

      public Vector(T[] data,Orientation direction, Monoid<T> add, Monoid<T> mult)
      {
        _data = data;
        Direction = direction;
        Ring = new SemiRing<T>(add, mult);
      }

      public Vector(IEnumerable<T> data, Orientation direction, Monoid<T> add, Monoid<T> mult)
      {
        _data = data.ToArray();
        Direction = direction;
        Ring = new SemiRing<T>(add, mult);
      }

      public Vector(Vector<T> vector)
      {
        _data = vector._data;
        Direction = vector.Direction;
        Ring = vector.Ring;
      }

      public int Length
      {
        get {return _data.Length;}
        private set { }
      }

      public T[] Data
      {
        get { return _data; }
        private set { }
      }

      public T this[int index]
      {
        get
        {
          if (index < 0 || index >= Length)
            throw new ArgumentOutOfRangeException("Vector: indexer value out of range");
          return _data[index];
        }
      }

      #region Object overrides
      public override bool Equals(object obj)
      {
        return Equals(obj as Vector<T>);
      }

      public override int GetHashCode()
      {
        int hash = Direction.GetHashCode();
        if (_data != null)
        {
          hash += 17 * Length;
          foreach (T item in _data)
            hash += 17 * item.GetHashCode();
        }
        return hash;
      }

      public static bool operator ==(Vector<T> v1, Vector<T> v2)
      {
        if (Object.ReferenceEquals(v1, null))
        {
          if (Object.ReferenceEquals(v2, null))
            return true;
          return false;
        }
        return v1.Equals(v2);
      }

      public static bool operator !=(Vector<T> v1, Vector<T> v2)
      {
        return !(v1 == v2);
      }
      #endregion

      #region IEquatable<Vector<T>> Members

      public bool Equals(Vector<T> other)
      {
        if (Object.ReferenceEquals(other, null))
          return false; // not equal to null
        if (Object.ReferenceEquals(this, other))
          return true; // reference identity

        return Direction == other.Direction
          && _data.SequenceEqual(other._data);
      }

      #endregion

      #region IEnumerable<T> Members

      public IEnumerator<T> GetEnumerator()
      {
        return _data.ToList().GetEnumerator();
      }

      #endregion

      #region IEnumerable Members

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
        return this.GetEnumerator();
      }

      #endregion

      // operations
      public static Vector<T> operator +(Vector<T> left, Vector<T> right)
      {
        if (left == null || right == null)
          throw new ArgumentException("Vector<T>.+ - null not allowed as an argument");
        if (left.Length != right.Length)
          throw new ArgumentException("Vector<T>.+ - vectors must have equal length");
        if (left.Direction != right.Direction)
          throw new ArgumentException("Vector<T>.+ - vectors must have the same orientation for addition");

        T[] data = left._data.Zip(right._data, (x, y) => left.Ring.Add.Op(x, y)).ToArray();
        return new Vector<T>(data, left.Direction, left.Ring.Add, left.Ring.Product);
      }

      public static Vector<T> operator *(Vector<T> left, T right)
      {
        if (left == null || right == null)
          throw new ArgumentException("Vector<T>.* - null not allowed as an argument");

        T[] data = left.Select((x) => left.Ring.Product.Op(x, right)).ToArray();
        return new Vector<T>(data, left.Direction, left.Ring.Add, left.Ring.Product);
      }

      public static Vector<T> operator *(T left, Vector<T> right)
      {
        if (left == null || right == null)
          throw new ArgumentException("Vector<T>.* - null not allowed as an argument");

        T[] data = right.Select((x) => right.Ring.Product.Op(left, x)).ToArray();
        return new Vector<T>(data, right.Direction, right.Ring.Add, right.Ring.Product);
      }

    }

    public static class VectorExtensions
    {
      public static T InnerProduct<T>(this Vector<T> v1, Vector<T> v2)
      {
        if (v1 == null || v2 == null)
          throw new ArgumentException("Vector<T>.InnerProduct - null not allowed as an argument");
        if (v1.Length != v2.Length)
          throw new ArgumentException("Vector<T>.InnerProduct - vectors must have equal length");
        if (v1.Direction != Orientation.Row || v2.Direction != Orientation.Column)
          throw new ArgumentException("Vector<T>.InnerProduct - dot product requires a row vector followed by a column vector");

        return v1.InnerProduct(v2, v2.Ring);
      }

      public static Vector<T> Transpose<T>(this Vector<T> v)
      {
        return new Vector<T>(v.Data,
          v.Direction == Orientation.Column ? Orientation.Row : Orientation.Column,
          v.Ring);
      }
    }
}
