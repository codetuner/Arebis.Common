using Arebis.Runtime.Aspects;
using System;
using System.Configuration;
using System.Transactions;

namespace Arebis.Data.Transactions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TransactionalOperationAttribute : AdviceAttribute
    {
        private static TimeSpan DefaultTimeout = TimeSpan.Parse(ConfigurationManager.AppSettings["TransactionalOperationAttribute.DefaultTimeout"] ?? "00:01:00");

        public TransactionalOperationAttribute()
            : base(false)
        { }

        public override void BeforeCall(ICallContext callContext)
        {
            callContext.SetProperty("transactionScope", new TransactionScope(TransactionScopeOption.Required, DefaultTimeout));
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
