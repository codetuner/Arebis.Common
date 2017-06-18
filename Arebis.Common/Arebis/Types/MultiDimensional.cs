using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis.Types
{
    /// <summary>
    /// A multi-dimensional array-like object.
    /// </summary>
    /// <typeparam name="TElement">Element type.</typeparam>
    public class MultiDimensional<TElement>
    {
        private int[] flatteningFactors;
        private TElement[] values;

        /// <summary>
        /// Creates a multi-dimensional array-like object with the given dimensions.
        /// </summary>
        public MultiDimensional(params int[] dimensions)
        {
            this.Initialize(dimensions);
        }

        /// <summary>
        /// Recalculate flattening factors and returns the flattened number of elements.
        /// </summary>
        protected int Initialize(int[] dimensions)
        {
            // Store dimensions:
            this.Dimensions = dimensions.ToList().AsReadOnly();

            // Recalculate flattening factors:
            var dimcount = dimensions.Length;
            this.flatteningFactors = new int[dimcount];
            var flatteningFactor = 1;
            for (int i = 0; i < dimcount; i++)
            {
                this.flatteningFactors[i] = flatteningFactor;
                flatteningFactor *= dimensions[i];
            }

            // Allocate a new values array:
            this.values = new TElement[flatteningFactor];

            // Return the flattened numbers of elements:
            return flatteningFactor;
        }

        /// <summary>
        /// Dimensions definition.
        /// </summary>
        public ReadOnlyCollection<int> Dimensions { get; private set; }

        /// <summary>
        /// Actual value at the given dimension indices.
        /// </summary>
        public TElement this[params int[] indices]
        {
            get
            {
                return this.values[FlattenIndices(indices, true)];
            }
            set
            {
                this.values[FlattenIndices(indices, true)] = value;
            }
        }

        /// <summary>
        /// Clears or fills the multidimensional.
        /// </summary>
        /// <param name="value">Value to fill the multidimensional with.</param>
        public void Clear(TElement value = default(TElement))
        {
            for (int i = 0; i < this.values.Length; i++)
                this.values[i] = value;
        }

        /// <summary>
        /// Resizes the MultiDimension keeping values on the common 'surface'.
        /// Note that you can change any dimension index, but you cannot change the number of dimensions.
        /// </summary>
        public void Resize(params int[] newDimensions)
        {
            var olddimens = this.Dimensions;
            var oldvalues = this.values;

            if (this.Dimensions.Count == newDimensions.Length)
            {
                var aNotLastHasChanged = false;
                for (int i = 0; i < newDimensions.Length - 1; i++)
                {
                    if (this.Dimensions[i] != newDimensions[i])
                    {
                        aNotLastHasChanged = true;
                        break;
                    }
                }

                if (aNotLastHasChanged)
                {
                    // Not only the last index has changed; any index can have changed:

                    // Create a target instance as helper:
                    var target = new MultiDimensional<TElement>(newDimensions);

                    // Datermine common surface:
                    var commonSurface = new int[newDimensions.Length];
                    for (int i = 0; i < newDimensions.Length; i++)
                    {
                        commonSurface[i] = Math.Min(olddimens[i], newDimensions[i]);
                    }

                    // Copy common surface values:
                    foreach (var index in MultiCounter.Enumerate(commonSurface))
                    {
                        target[index] = this[index];
                    }

                    // Substitute current instance by target:
                    this.Dimensions = newDimensions.ToList().AsReadOnly();
                    this.flatteningFactors = target.flatteningFactors;
                    this.values = target.values;

                }
                else if (this.Dimensions[newDimensions.Length - 1] != newDimensions[newDimensions.Length - 1])
                {
                    // Only the last index changed.
                    this.Initialize(newDimensions);
                    Array.Copy(oldvalues, this.values, Math.Min(oldvalues.Length, this.values.Length));
                }
                else
                {
                    // All dimensions (last AND notlasts) are equal, nothing to do...
                }
            }
            else
            {
                // The number of dimensions has changed:
                throw new NotSupportedException("Cannot resize a MultiDimensional to a different number of dimensions.");
            }
        }

        protected int FlattenIndices(int[] indices, bool safe)
        {
            if (safe)
            {
                if (indices.Length != this.Dimensions.Count) throw new ArgumentException("Number of dimensions do not match.");
                for (int i = 0; i < this.Dimensions.Count; i++)
                {
                    if (indices[i] < 0 || indices[i] >= this.Dimensions[i])
                        throw new IndexOutOfRangeException(String.Format("Index for dimension {0} is {1} while should be in range [0,{2}].", i, indices[i], this.Dimensions[i] - 1));
                }
            }

            var index = 0;
            for (int i = 0; i < this.Dimensions.Count; i++)
            {
                index += this.flatteningFactors[i] * indices[i];
            }

            return index;
        }
    }
}
