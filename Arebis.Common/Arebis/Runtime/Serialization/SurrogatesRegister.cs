using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Arebis.Runtime.Serialization
{
	/// <summary>
	/// Versionned register of serialization surrogates.
	/// </summary>
	public class SurrogatesRegister
	{
		private List<Registration> register = new List<Registration>();

		/// <summary>
		/// Registers a surrogate for a given type, from a given recording version on.
		/// </summary>
		public void RegisterSurrogate(double version, Type targetType, ISerializationSurrogate surrogate)
		{
			#region Check preconditions
			if (targetType == null) throw new ArgumentNullException("targetType");
			if (surrogate == null) throw new ArgumentNullException("surrogate");
			#endregion Check preconditions

			// Check that no surrogate conflicts:
			foreach (Registration reg in register)
			{
				if (reg.MatchesTypeAndVersion(targetType, version))
				{
					throw new InvalidOperationException("The SerializationSurrogate conflicts with an already registered surrogate for same version and target type.");
				}
			}
			// Register surrogate:
			register.Add(new Registration(version, targetType, surrogate));
		}

		/// <summary>
		/// Unregisters the surrogate for a given type, from a given recording version sion.
		/// </summary>
		public void UnregisterSurrogate(double version, Type targetType)
		{
			// Find surrogate and unregister it from the given version on:
			foreach (Registration reg in register)
			{
				if (reg.MatchesTypeAndVersion(targetType, version))
				{
					reg.SetEndVersion(version);
				}
			}
		}

		/// <summary>
		/// Lists all registrations in the SurrogatesRegister.
		/// </summary>
		/// <returns>List of Registration objects.</returns>
		public IList<Registration> ListRegistrations()
		{
			return new List<Registration>(register);
		}

		/// <summary>
		/// Builds an ISurrugateSelector filled with all SerializationSurrogates
		/// valid for the given version.
		/// </summary>
		public ISurrogateSelector GetSurrogateSelectorFor(double version)
		{
			SurrogateSelector ss = new SurrogateSelector();
			StreamingContext sc = new StreamingContext(StreamingContextStates.All);
			foreach (Registration reg in register)
			{
				if (reg.MatchesVersion(version))
				{
					ss.AddSurrogate(reg.TargetType, sc, reg.Surrogate);
				}
			}
			return ss;
		}

		/// <summary>
		/// Represents a registration in the SurrogatesRegister.
		/// </summary>
		public class Registration
		{
			private double startVersion;
			private double endVersion;
			private Type targetType;
			private ISerializationSurrogate surrogate;

			internal Registration(double startVersion, Type targetType, ISerializationSurrogate surrogate)
			{
				this.startVersion = startVersion;
				this.endVersion = Double.MaxValue;
				this.targetType = targetType;
				this.surrogate = surrogate;
			}

			/// <summary>
			/// Version from which of the registration is valid.
			/// </summary>
			public double StartVersion
			{
				get
				{
					return this.startVersion;
				}
			}

			/// <summary>
			/// Version from which of the registration is not valid anymore.
			/// </summary>
			public double EndVersion
			{
				get
				{
					return this.endVersion;
				}
			}

			/// <summary>
			/// The type targetted by the surrogate.
			/// </summary>
			public Type TargetType
			{
				get
				{
					return this.targetType;
				}
			}

			/// <summary>
			/// The serialization surrogate.
			/// </summary>
			public ISerializationSurrogate Surrogate
			{
				get
				{
					return this.surrogate;
				}
			}

			/// <summary>
			/// Whether the current recording is valid for the given argument(s).
			/// </summary>
			public bool MatchesTypeAndVersion(Type targetType, double version)
			{
				return this.MatchesVersion(version) && this.MatchesType(targetType);
			}

			/// <summary>
			/// Whether the current recording is valid for the given argument(s).
			/// </summary>
			public bool MatchesType(Type targetType)
			{
				return this.targetType.Equals(targetType);
			}

			/// <summary>
			/// Whether the current recording is valid for the given argument(s).
			/// </summary>
			public bool MatchesVersion(double version)
			{
				return (version >= this.startVersion) && (version < this.endVersion);
			}

			/// <summary>
			/// Sets the end version of the registration.
			/// </summary>
			internal void SetEndVersion(double endVersion)
			{
				this.endVersion = endVersion;
			}
		}
	}
}
