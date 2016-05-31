using System;

namespace Arebis.Pdf.Common
{
    /// <summary>
    /// A PdfObject reference consisting of the ObjectId and GenerationId.
    /// </summary>
    [Serializable]
    public struct PdfObjectRef
    {
        private int generationId;
        private int objectId;

        public PdfObjectRef(int generationId, int objectId)
        {
            this.generationId = generationId;
            this.objectId = objectId;
        }

        public int GenerationId { get { return generationId; } }

        public int ObjectId { get { return objectId; } }

        public override string ToString()
        {
            return ObjectId + " " + GenerationId + " R";
        }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(this, obj))
            {
                return true;
            }
            else if (obj is PdfObjectRef)
            {
                var tobj = (PdfObjectRef)obj;
                return (this.ObjectId == tobj.ObjectId) && (this.GenerationId == tobj.GenerationId);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return 9677 ^ GenerationId ^ (ObjectId << 5);
        }

        /// <summary>
        /// Returns a proposed name for this reference.
        /// </summary>
        public string ToDefaultName()
        {
            if (GenerationId == 0)
                return "/Obj" + this.ObjectId;
            else
                return "/Obj" + this.ObjectId + "G" + this.GenerationId;
        }
    }
}
