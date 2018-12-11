using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCAPP.Types
{
    public class Error
    {
        public string ErrorMessage { get; set; }
        public Error InnerError { get; set; }

        public Error() { }

        public Error(string error)
        {
            if (error == null || error.Trim() == "")
                ErrorMessage = "Unknown Error";
            ErrorMessage = error;
        }

        public Error(Exception e)
        {
            ErrorMessage = GetErrorMessage(e);
        }

        string GetErrorMessage(Exception e)
        {
            var error = e.Message;
            if (error == null || error.Trim() == "")
                error = "Unknown Error";

            if (e.InnerException != null)
                return error + " " + GetErrorMessage(e.InnerException);

            return error;
        }
    }
}