using System;

namespace Helium
{
    /// <summary>
    /// Error provided by the Helium SDK.
    /// </summary>
    public class HeliumError
    {
        /// <summary>
        /// Associated error code.
        /// </summary>
        public HeliumErrorCode ErrorCode;
        
        /// <summary>
        /// Error description, if any.
        /// </summary>
        public string ErrorDescription;

        public HeliumError(HeliumErrorCode code)
        {
            ErrorCode = code;
        }
        
        public HeliumError(HeliumErrorCode code, string description)
        {
            ErrorCode = code;
            ErrorDescription = description;
        }

        public override string ToString()
        {
            return $"{ErrorCode} {ErrorDescription}";
        }
        
        /// <summary>
        /// Creates a HeliumError based off an integer value.
        /// </summary>
        /// <param name="errorObj">error code.</param>
        /// <returns></returns>
        private static HeliumError ErrorFromInt(object errorObj)
        {
            int error;
            try
            {
                error = Convert.ToInt32(errorObj);
            }
            catch
            {
                return new HeliumError(HeliumErrorCode.Unknown);
            }

            if (error == -1)
                return null;

            if (error < 0 || error > (int)HeliumErrorCode.Unknown)
                return new HeliumError(HeliumErrorCode.Unknown, null);
            return new HeliumError((HeliumErrorCode)error, null);
        }

        /// <summary>
        /// Creates a HeliumError based off a code and description.
        /// </summary>
        /// <param name="errorObj">error code.</param>
        /// <param name="errString">error description.</param>
        /// <returns></returns>
        public static HeliumError ErrorFromIntString(object errorObj, string errString)
        {
            var e = ErrorFromInt(errorObj);
            if (e != null)
                e.ErrorDescription = errString;
            return e;
        }
    }
}