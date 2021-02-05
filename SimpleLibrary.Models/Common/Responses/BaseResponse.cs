using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLibrary.Models.Common.Responses
{
    public class BaseResponse
    {
        public string Message { set; get; }
        public bool Status { set; get; }

        public BaseResponse()
        {
            Message = "";
            Status = true;
        }

        public void SetSuccessStatsus()
        {
            this.Status = true;
        }

        public void SetFailedtatsus()
        {
            this.Status = false;
        }

        public void SetMessage(string message, int type)
        {
            switch (type)
            {
                case 1:
                    Message = "[ERROR] " + message;
                    break;
                case 2:
                    Message = "[EXCEPTION] " + message;
                    break;
                case 3:
                    Message = "[WARNING] " + message;
                    break;
                case 4:
                    Message = "[INFO] " + message;
                    break;
                case 5:
                    Message = "[SUCCESS] " + message;
                    break;
                default:
                    Message = "[UNDEFINED] " + message;
                    break;
            }
        }
    }
}
