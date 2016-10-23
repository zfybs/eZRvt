using System;
using DllActivator;

namespace DllActivator
{

    public class DllActivator_eZRvt : IDllActivator_RevitStd
    {
        public void ActivateReferences()
        {
            IDllActivator_std dat1;
            //
            dat1 = new DllActivator_std();
            dat1.ActivateReferences();
            //

            IDllActivator_RevitStd dat2;
            dat2 = new DllActivator_RevitStd();
            dat2.ActivateReferences();
            //
        }
    }
}