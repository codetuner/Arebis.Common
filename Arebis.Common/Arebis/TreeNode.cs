using System.Collections.Generic;
using System.Linq;
using System;

namespace Arebis
{
	/// <summary>
	/// Represents a node in a tree structure and helps
	/// constructing and navigating trees based on paths.
	/// </summary>
	public class TreeNode<T>
	{
		/// <summary>
		/// Constructs the root node of a tree.
		/// </summary>
		public TreeNode(T item)
			: this(null, item)
		{ }

		/// <summary>
		/// Constructs a node in a tree.
		/// </summary>
		public TreeNode(TreeNode<T> parent, T item)
		{
			this.Parent = parent;
			this.Item = item;
			this.Children = new HashSet<TreeNode<T>>();
		}

		/// <summary>
		/// The item held by the node.
		/// </summary>
		public T Item { get; set; }

		/// <summary>
		/// The parent node. Null for the root node.
		/// </summary>
		public TreeNode<T> Parent { get; set; }

		/// <summary>
		/// The children nodes of the current node.
		/// </summary>
		public HashSet<TreeNode<T>> Children { get; set; }

		/// <summary>
		/// Adds the given child provided it is not yet a child
		/// of this node.
		/// </summary>
		/// <returns>True if the child was added, false if it was already present.</returns>
		public bool AddChild(T childItem)
		{
			if (Object.ReferenceEquals(childItem, null))
				throw new ArgumentNullException("childItem");

			if (this.Children.Where(p => p.Item.Equals(childItem)).Count() == 0)
			{
				this.Children.Add(new TreeNode<T>(this, childItem));
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Adds a path of items to the current node, extending
		/// the tree if needed.
		/// </summary>
		public void AddPath(IEnumerable<T> path)
		{
			// Start at the current node:
			TreeNode<T> node = this;
			bool extendingTree = false;

			// Walk path:
			foreach (T item in path)
			{
				TreeNode<T> itemNode = null;

				// Look for a matching node in the children of the parent node:
				if (!extendingTree)
					itemNode = node.Children.Where(p => p.Item.Equals(item)).FirstOrDefault();

				// Extend the tree with a new node if none found:
				if (itemNode == null)
				{
					itemNode = new TreeNode<T>(node, item);
					node.Children.Add(itemNode);
					extendingTree = true;
				}

				// Hop to child node for next iteration:
				node = itemNode;
			}
		}

		/// <summary>
		/// Returns a list of item paths.
		/// </summary>
		/// <param name="includeRoot">Whether root node is to be included in paths.</param>
		/// <param name="leafPathsOnly">If true, only paths ending on leafs
		/// are returned, if false, intermediate paths are also included.</param>
		/// <returns></returns>
		[Obsolete("Not yet implemented.", true)]
		public IList<IList<T>> ToPaths(bool includeRoot, bool leafPathsOnly)
		{
			throw new NotImplementedException();
		}
	}
}
