using Arebis.Logging;
using Arebis.Runtime.Aspects;
using System;
using System.Configuration;
using System.Transactions;

namespace Arebis.Data.Transactions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TransactionalOperationAttribute : AdviceAttribute
    {
        #region Static constructor reading appSettings

        static TransactionalOperationAttribute()
        {
            try
            {
                DefaultIsolationLevel = (IsolationLevel)Enum.Parse(typeof(IsolationLevel), ConfigurationManager.AppSettings["TransactionalOperationAttribute.DefaultIsolationLevel"] ?? "Serializable");
            }
            catch
            {
                System.Diagnostics.Trace.Write("Error decoding appSetting \"TransactionalOperationAttribute.DefaultIsolationLevel\". The value must match one of the System.Transactions.IsolationLevel enumeration values. Reverted to default value.");
                DefaultIsolationLevel = IsolationLevel.Serializable;
            }
            try
            {
                DefaultTimeoutMs = (long)TimeSpan.Parse(ConfigurationManager.AppSettings["TransactionalOperationAttribute.DefaultTimeout"] ?? "00:01:00").TotalMilliseconds;
            }
            catch
            {
                System.Diagnostics.Trace.Write("Error decoding appSetting \"TransactionalOperationAttribute.DefaultTimeout\". Value must a valid Timestamp representation. I.e: \"00:01:00\". Reverted to default value.");
                DefaultTimeoutMs = 60000;
            }
        }

        #endregion

        /// <summary>
        /// Default isolation level.
        /// Defaults to Serializable.
        /// Can be overriden with the AppSetting "TransactionalOperationAttribute.DefaultIsolationLevel".
        /// </summary>
        public static IsolationLevel DefaultIsolationLevel { get; set; }

        /// <summary>
        /// Default timeout in milliseconds.
        /// Defaults to 1 minute.
        /// Can be overriden with the AppSetting "TransactionalOperationAttribute.DefaultTimeout" which has a timespan as value.
        /// </summary>
        public static long DefaultTimeoutMs { get; set; }

        public TransactionalOperationAttribute()
            : this(TransactionScopeOption.Required)
        { }

        public TransactionalOperationAttribute(TransactionScopeOption option)
            : base(false)
        {
            this.Option = option;
            this.IsolationLevel = DefaultIsolationLevel;
            this.TimeoutMs = DefaultTimeoutMs;
        }

        /// <summary>
        /// TransactionScope option.
        /// Defaults to Required.
        /// </summary>
        public TransactionScopeOption Option { get; set; }

        /// <summary>
        /// Isolation level.
        /// Defaults to TransactionalOperationAttribute.DefaultIsolationLevel.
        /// </summary>
        public IsolationLevel IsolationLevel { get; set; }

        /// <summary>
        /// Timeout in milliseconds.
        /// Defaults to TransactionalOperationAttribute.DefaultTimeoutMs.
        /// </summary>
        public long TimeoutMs { get; set; }

        public override void BeforeCall(ICallContext callContext)
        {
            callContext.SetProperty("transactionScope", new TransactionScope(this.Option, new TransactionOptions() { IsolationLevel = this.IsolationLevel, Timeout = TimeSpan.FromMilliseconds(this.TimeoutMs) }));
        }

        public override void AfterCall(ICallContext callContext)
        {
            var transactionScope = callContext.GetProperty("transactionScope") as TransactionScope;
            if (transactionScope != null)
            {
                if (callContext.CallSucceeded)
                {
                    transactionScope.Complete();
                }
                transactionScope.Dispose();
            }
        }
    }
}
