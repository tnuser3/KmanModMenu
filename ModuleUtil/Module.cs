using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KmanModule
{
    public interface Module
    {
        void Init();
        void Update();
        void OnLoad();
        void OnUnload();
        void UnLoad();
    }
}
