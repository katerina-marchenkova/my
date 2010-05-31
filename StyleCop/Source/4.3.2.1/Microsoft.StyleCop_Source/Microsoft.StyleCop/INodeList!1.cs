namespace Microsoft.StyleCop
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification="The interface is implemented by lists.")]
    public interface INodeList<T> : IEnumerable<T>, IEnumerable where T: class
    {
        IEnumerable<T> ForwardIterator();
        IEnumerable<T> ForwardIterator(Microsoft.StyleCop.Node<T> start);
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification="The use of IEnumerable<T> is consistent with standards.")]
        IEnumerable<Microsoft.StyleCop.Node<T>> ForwardNodeIterator();
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification="The use of IEnumerable<T> is consistent with standards.")]
        IEnumerable<Microsoft.StyleCop.Node<T>> ForwardNodeIterator(Microsoft.StyleCop.Node<T> start);
        bool OutOfBounds(Microsoft.StyleCop.Node<T> node);
        IEnumerable<T> ReverseIterator();
        IEnumerable<T> ReverseIterator(Microsoft.StyleCop.Node<T> start);
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification="The use of IEnumerable<T> is consistent with standards.")]
        IEnumerable<Microsoft.StyleCop.Node<T>> ReverseNodeIterator();
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification="The use of IEnumerable<T> is consistent with standards.")]
        IEnumerable<Microsoft.StyleCop.Node<T>> ReverseNodeIterator(Microsoft.StyleCop.Node<T> start);

        Microsoft.StyleCop.Node<T> First { get; }

        Microsoft.StyleCop.Node<T> Last { get; }
    }
}

