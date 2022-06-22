using System;

namespace Helium
{
    public class HeliumError
    {
        public HeliumErrorCode ErrorCode;
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
        
        public static HeliumError ErrorFromInt(object errorObj)
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

            switch (error)
            {
                case -1:
                    return null;
                case < 0:
                // out of bounds
                case > (int)HeliumErrorCode.Unknown:
                    return new HeliumError(HeliumErrorCode.Unknown);
                default:
                    return new HeliumError((HeliumErrorCode)error);
            }
        }

        public static HeliumError ErrorFromIntString(object errorObj, string errString)
        {
            var e = ErrorFromInt(errorObj);
            if (e != null)
                e.ErrorDescription = errString;
            return e;
        }
    }
}
