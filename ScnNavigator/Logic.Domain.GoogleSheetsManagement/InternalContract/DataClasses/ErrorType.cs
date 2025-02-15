using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.GoogleSheetsManagement.InternalContract.DataClasses
{
    internal enum ErrorType
    {
        /// <summary>
        /// Default error value.
        /// </summary>
        /// <remarks>Do not use.</remarks>
        ERROR_TYPE_UNSPECIFIED,

        /// <summary>
        /// Corresponds to #ERROR.
        /// </summary>
        ERROR,

        /// <summary>
        /// Corresponds to #NULL.
        /// </summary>
        NULL_VALUE,

        /// <summary>
        /// Corresponds to #DIV/0.
        /// </summary>
        DIVIDE_BY_ZERO,

        /// <summary>
        /// Corresponds to #VALUE.
        /// </summary>
        VALUE,

        /// <summary>
        /// Corresponds to #REF.
        /// </summary>
        REF,

        /// <summary>
        /// Corresponds to #NAME?.
        /// </summary>
        NAME,

        /// <summary>
        /// Corresponds to #NUM.
        /// </summary>
        NUM,

        /// <summary>
        /// Corresponds to #N/A.
        /// </summary>
        N_A,

        /// <summary>
        /// Describes a loading formula.
        /// </summary>
        LOADING
    }
}
