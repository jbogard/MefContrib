﻿namespace MefContrib.Hosting.Conventions.Configuration
{
    /// <summary>
    /// Defines the functionality of a convention registry for conventions implementing the <see cref="IPartConvention"/> interface.
    /// </summary>
    public interface IPartRegistry : IHideObjectMembers
    {
        /// <summary>
        /// Creates a convention builde for <see cref="PartConvention"/> conventions.
        /// </summary>
        /// <returns>A <see cref="IPartConventionBuilder{TPartConvention}"/> instance for the <see cref="PartConvention"/> type.</returns>
        IPartConventionBuilder<PartConvention> Part();

        /// <summary>
        /// Create a convention builder for the <typeparamref name="TConvention"/> convention type.
        /// </summary>
        /// <typeparam name="TConvention">The type of a class which implements the <see cref="IPartConvention"/> interface.</typeparam>
        /// <returns>A <see cref="IPartConventionBuilder{TPartConvention}"/> instance for the part convention type specified by the <typeparamref name="TConvention"/> type parameter.</returns>
        IPartConventionBuilder<TConvention> Part<TConvention>() where TConvention : IPartConvention, new();
    }
}