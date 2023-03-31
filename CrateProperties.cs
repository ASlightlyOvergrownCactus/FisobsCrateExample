using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fisobs.Properties;

namespace TestMod
{
    sealed class CrateProperties : ItemProperties
    {
        public override void Throwable(Player player, ref bool throwable)
        => throwable = false;

        
        
    }
}
