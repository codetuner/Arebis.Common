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
    public class MultiDemensional<TElement>
    {
        private int[] flatteningFactors;
        private int[] dimensions;
        private TElement[] values;

        public MultiDemensional(params int[] dimensions)
        {
            this.flatteningFactors = new int[dimensions.Length];
            var flatteningFactor = 1;
            for (int i = 0; i < dimensions.Length; i++)
            {
                this.flatteningFactors[i] = flatteningFactor;
                flatteningFactor *= dimensions[i];
            }

            this.values = new TElement[flatteningFactor];
            this.Dimensions = new List<int>(dimensions).AsReadOnly();
        }

        public ReadOnlyCollection<int> Dimensions { get; private set; }

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

        public void Resize(params int[] dimensions)
        {
            throw new NotImplementedException(); 
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
