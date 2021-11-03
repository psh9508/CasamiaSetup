using CasamiaSetup.Communication.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasamiaSetup.Communication
{
    public class InqCasamiaUsedPosResponse : ResponseBase
    {
        public bool HasPos { get; set; }
    }
}
