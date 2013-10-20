namespace Arktos.WinBert.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Cci;

    /// <summary>
    /// Some simple operation extension methods.
    /// </summary>
    public static class IOperations
    {
        /// <summary>
        /// Is the target operation a store local?
        /// </summary>
        /// <param name="operation">
        /// The operation to test.
        /// </param>
        /// <returns>
        /// True if the target operation is a store local, false otherwise.
        /// </returns>
        public static bool IsStoreLocal(this IOperation operation)
        {
            var isStoreLocal = false;
            switch (operation.OperationCode)
            {
                case OperationCode.Stloc:
                case OperationCode.Stloc_0:
                case OperationCode.Stloc_2:
                case OperationCode.Stloc_3:
                case OperationCode.Stloc_S:
                    isStoreLocal = true;
                    break;
                default:
                    isStoreLocal = false;
                    break;
            }

            return isStoreLocal;
        }

        /// <summary>
        /// Is the target operation a call virtual?
        /// </summary>
        /// <param name="operation">
        /// The operation to test.
        /// </param>
        /// <returns>
        /// True if the target operation is a call virtual, false otherwise.
        /// </returns>
        public static bool IsCallVirt(this IOperation operation)
        {
            return operation.OperationCode == OperationCode.Callvirt;
        }

        /// <summary>
        /// Is the target operation a call virtual?
        /// </summary>
        /// <param name="operation">
        /// The operation to test.
        /// </param>
        /// <returns>
        /// True if the target operation is a call virtual, false otherwise.
        /// </returns>
        public static bool IsNewObj(this IOperation operation)
        {
            return operation.OperationCode == OperationCode.Newobj;
        }

        /// <summary>
        /// Is the target operation a ret? Note that this is not analogous to the "return" of a method, but
        /// instead maps to the IL instruction. All IL method bodies have a single return point, and this
        /// instruction is that return point.
        /// </summary>
        /// <param name="operation">
        /// The operation to test.
        /// </param>
        /// <returns>
        /// True if the target operation is a ret, false otherwise.
        /// </returns>
        public static bool IsRet(this IOperation operation)
        {
            return operation.OperationCode == OperationCode.Ret;
        }

        /// <summary>
        /// Is the target operation in the try block of the test?
        /// </summary>
        /// <param name="operation">
        /// The operation to test.
        /// </param>
        /// <param name="handlerBounds">
        /// The handler bounds to test against. Presumably a set of bounds that exist in the same method from
        /// whence the operation came.
        /// </param>
        /// <returns>
        /// True if the operation is in the test try block, false otherwise.
        /// </returns>
        public static bool IsOperationInTryBlock(this IOperation operation, IOperationExceptionInformation handlerBounds)
        {
            return operation.Offset >= handlerBounds.TryStartOffset &&
                operation.Offset <= handlerBounds.TryEndOffset;
        }

        /// <summary>
        /// Is the target operation in the catch block of the test?
        /// </summary>
        /// <param name="operation">
        /// The operation to test.
        /// </param>
        /// <param name="handlerBounds">
        /// The handler bounds to test against. Presumably a set of bounds that exist in the same method from
        /// whence the operation came.
        /// </param>
        /// <returns>
        /// True if the operation is in the test catch block, false otherwise.
        /// </returns>
        public static bool IsOperationInCatchBlock(this IOperation operation, IOperationExceptionInformation handlerBounds)
        {
            return operation.Offset >= handlerBounds.HandlerStartOffset &&
                operation.Offset <= handlerBounds.HandlerEndOffset;
        }
    }
}
