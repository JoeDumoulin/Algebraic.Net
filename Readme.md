## Algebra.NET ##

This library supports the creation and use of algebraic structures in Microsoft's C# language.

###Monoid###
The most basic structure in the current implementation is a monoid.  A monoid is a set of objects for which a binary operation exists and the set contains an identity element over the binary operation.

The implementations of these objects is generic, allowing one to apply the same algebraic structures to sets with very different properties.  For example, the library includes monoid specializations for `integer` and `double` types (with multiplication and addition as binary operations) as well as `string` and `IEnumerable<T>` generic containers.  In the later two specializations the binary operation is concatenation.

Also included is a specialization for single parameter functions of `T` where the binary operation is function composition, and a permutation class that also uses composition as its operation.


###Group###
Some of the monoids mentioned above (integer and permutation at least) are also groups.  Though the group specialization of these structures is not in the library, it would be quite easy to create the group object by simply adding a function to describe the inverse operation for the group.  

For example, one can create a group on additive integers by defining a delegate for the inverse of an element for the additive integer operation:

`(x) => -x` 

Groups are monoids with the added feature that each element in the adjoining set has an inverse in the group.