using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANCommon
{
    public interface IForm
    {
        void SetData(DataRow row);
        void GetData(DataRow row);
        bool ValidateData(DataRow row);
    }
}
